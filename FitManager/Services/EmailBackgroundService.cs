using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FitManager.Data;
using FitManager.Models;

namespace FitManager.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailBackgroundService> _logger;

        public EmailBackgroundService(IServiceProvider serviceProvider, ILogger<EmailBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EmailBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var now = DateTime.UtcNow;
                    var uskoroIsticu = await db.Clanarine
                        .Include(c => c.Clan)
                        .Include(c => c.TipClanarine)
                        .Where(c => c.Status == StatusClanarine.AKTIVNA
                                 && c.ObavjestenjePoslano == false
                                 && c.DatumIsteka >= now
                                 && c.DatumIsteka <= now.AddDays(7))
                        .ToListAsync(stoppingToken);

                    foreach (var clanarina in uskoroIsticu)
                    {
                        var postoji = await db.EmailObavjestenja
                            .AnyAsync(e => e.ClanarinaId == clanarina.Id, stoppingToken);
                        if (!postoji)
                        {
                            var poruka = $"Poštovani {clanarina.Clan?.Ime} {clanarina.Clan?.Prezime},<br><br>" +
                                         $"Vaša članarina ({clanarina.TipClanarine?.Naziv}) ističe {clanarina.DatumIsteka:dd.MM.yyyy}.<br>" +
                                         $"Molimo Vas da izvršite produženje kako biste nastavili koristiti naše usluge.<br><br>" +
                                         $"S poštovanjem,<br>FitManager tim.";

                            db.EmailObavjestenja.Add(new EmailObavjestenje
                            {
                                ClanarinaId = clanarina.Id,
                                Sadrzaj = poruka,
                                DatumSlanja = now,
                                Status = StatusObavjestenja.NA_CEKANJU,
                                PokusajSlanja = 0
                            });
                            clanarina.ObavjestenjePoslano = true;
                        }
                    }

                    if (uskoroIsticu.Any())
                        await db.SaveChangesAsync(stoppingToken);

                    var pending = await db.EmailObavjestenja
                        .Where(e => e.Status == StatusObavjestenja.NA_CEKANJU && e.PokusajSlanja < 5)
                        .OrderBy(e => e.DatumSlanja)
                        .ToListAsync(stoppingToken);

                    foreach (var item in pending)
                    {
                        try
                        {
                            await emailSvc.SendEmailAsync(item.Clanarina!.Clan!.Email, "Obavještenje", item.Sadrzaj);
                            item.OznaciPoslano();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Neuspjelo slanje emaila za obavjestenje id={Id}", item.Id);
                            item.OznaciGresku();
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška u EmailBackgroundService petlji.");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
