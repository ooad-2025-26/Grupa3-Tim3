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
