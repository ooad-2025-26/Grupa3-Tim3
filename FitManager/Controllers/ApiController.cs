using FitManager.Data;
using FitManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FitManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Korisnik> _userManager;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<Korisnik> _signInManager;

        public ApiController(
            ApplicationDbContext context,
            Microsoft.AspNetCore.Identity.UserManager<Korisnik> userManager,
            Microsoft.AspNetCore.Identity.SignInManager<Korisnik> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ────────────────────────────────────────────────────────────────────
        // PROFILE
        // ────────────────────────────────────────────────────────────────────

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(string id)
        {
            var user = await _context.Korisnici.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(new
            {
                id = user.Id,
                username = user.UserName,
                email = user.Email,
                ime = user.Ime,
                prezime = user.Prezime,
                telefon = user.PhoneNumber,
                datumRodjenja = user.DatumRodjenja
            });
        }

        public class ProfileUpdateDto
        {
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? CurrentPassword { get; set; }
            public string? NewPassword { get; set; }
            public string? ConfirmPassword { get; set; }
            public ProfilClana? Profil { get; set; }
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] ProfileUpdateDto payload)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var errors = new List<string>();

            if (!string.IsNullOrWhiteSpace(payload?.Username) && payload.Username != user.UserName)
            {
                var res = await _userManager.SetUserNameAsync(user, payload.Username);
                if (!res.Succeeded) errors.AddRange(res.Errors.Select(e => e.Description));
            }

            if (!string.IsNullOrWhiteSpace(payload?.Email) && payload.Email != user.Email)
            {
                if (string.IsNullOrWhiteSpace(payload.CurrentPassword))
                {
                    errors.Add("To change email, current password is required.");
                }
                else
                {
                    var passwordOk = await _userManager.CheckPasswordAsync(user, payload.CurrentPassword);
                    if (!passwordOk)
                    {
                        errors.Add("Current password is incorrect.");
                    }
                    else
                    {
                        var res = await _userManager.SetEmailAsync(user, payload.Email);
                        if (!res.Succeeded) errors.AddRange(res.Errors.Select(e => e.Description));
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(payload?.NewPassword) || !string.IsNullOrWhiteSpace(payload?.ConfirmPassword))
            {
                if (string.IsNullOrWhiteSpace(payload.NewPassword) || string.IsNullOrWhiteSpace(payload.ConfirmPassword))
                {
                    errors.Add("Both password fields must be filled to change password.");
                }
                else if (payload.NewPassword != payload.ConfirmPassword)
                {
                    errors.Add("New password and confirmation do not match.");
                }
                else
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var reset = await _userManager.ResetPasswordAsync(user, token, payload.NewPassword!);
                    if (!reset.Succeeded) errors.AddRange(reset.Errors.Select(e => e.Description));
                }
            }

            if (payload?.Profil != null) user.AzurirajProfil(payload.Profil);

            if (errors.Any()) return BadRequest(new { errors });

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(new { errors = updateResult.Errors.Select(e => e.Description).ToList() });

            var currentUserId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(currentUserId) && currentUserId == user.Id)
                await _signInManager.RefreshSignInAsync(user);

            return Ok(new { username = user.UserName, email = user.Email });
        }

        [HttpDelete("profile/{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var res = await _userManager.DeleteAsync(user);
            if (!res.Succeeded)
                return BadRequest(new { errors = res.Errors.Select(e => e.Description) });

            var currentUserId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(currentUserId) && currentUserId == id)
                await _signInManager.SignOutAsync();

            return NoContent();
        }

        // ────────────────────────────────────────────────────────────────────
        // MEMBERSHIP / QR
        // ────────────────────────────────────────────────────────────────────

        [HttpGet("membership/{userId}")]
        public async Task<IActionResult> GetMembership(string userId)
        {
            var now = DateTime.UtcNow;

            var clanarina = await _context.Clanarine
                .Include(c => c.TipClanarine)
                .Where(c =>
                    c.ClanId == userId &&
                    c.Status == StatusClanarine.AKTIVNA &&
                    c.DatumPocetka <= now &&
                    c.DatumIsteka >= now)
                .OrderByDescending(c => c.DatumPocetka)
                .FirstOrDefaultAsync();

            if (clanarina == null) return NotFound();

            var qr = await _context.QRKodovi
                .FirstOrDefaultAsync(q => q.ClanId == userId && q.Aktivan);

            string qrBase64 = string.Empty;
            if (qr != null)
            {
                using var generator = new QRCodeGenerator();
                using var data = generator.CreateQrCode(qr.Kod, QRCodeGenerator.ECCLevel.Q);
                using var png = new PngByteQRCode(data);
                qrBase64 = Convert.ToBase64String(png.GetGraphic(20));
            }

            return Ok(new
            {
                Tip = clanarina.TipClanarine.Naziv,
                DatumPocetka = clanarina.DatumPocetka,
                DatumIsteka = clanarina.DatumIsteka,
                Cijena = clanarina.Cijena,
                QrBase64 = qrBase64
            });
        }

        // ────────────────────────────────────────────────────────────────────
        // TRENINZI
        // ────────────────────────────────────────────────────────────────────

        /// <summary>
        /// GET /api/Api/trainings/date/2025-06-15
        /// Vraća sve grupne treninge za odabrani datum.
        /// Sve property u camelCase da JS može čitati bez konverzije.
        /// </summary>
        [HttpGet("trainings/date/{date}")]
        public async Task<IActionResult> GetTrainingsByDate(string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest(new { message = "Neispravan datum." });

            var start = parsedDate.Date;
            var end = start.AddDays(1);

            var list = await _context.GrupniTreninzi
                .Where(t => t.DatumVrijeme >= start && t.DatumVrijeme < end)
                .Select(t => new
                {
                    id = t.Id,
                    naziv = t.Naziv,
                    opis = t.Opis,
                    datumVrijeme = t.DatumVrijeme,
                    maksKapacitet = t.MaksKapacitet,
                    slobodnaMjesta = t.SlobodnaMjesta,
                    trajanje = t.Trajanje,
                    trenerEmail = t.Trener != null ? t.Trener.Email : null
                })
                .ToListAsync();

            return Ok(list);
        }

        // ────────────────────────────────────────────────────────────────────
        // REZERVACIJE
        // ────────────────────────────────────────────────────────────────────

        /// <summary>
        /// GET /api/Api/reservations/user/{userId}
        /// Vraća sve aktivne rezervacije za korisnika.
        /// camelCase property-i.
        /// </summary>
        [HttpGet("reservations/user/{userId}")]
        public async Task<IActionResult> GetUserReservations(string userId)
        {
            var list = await _context.Rezervacije
                .Include(r => r.GrupniTrening)
                .Where(r => r.ClanId == userId && r.Status == StatusRezervacije.AKTIVNA)
                .ToListAsync();

            return Ok(list.Select(r => new
            {
                id = r.Id,
                grupniTreningId = r.GrupniTreningId,
                naziv = r.GrupniTrening.Naziv,
                datumVrijeme = r.GrupniTrening.DatumVrijeme,
                datumKreiranja = r.DatumKreiranja
            }));
        }

        /// <summary>
        /// POST /api/Api/reserve
        /// Body: { "ClanId": "...", "GrupniTreningId": 5 }
        /// </summary>
        [HttpPost("reserve")]
        public async Task<IActionResult> CreateReservation([FromBody] ReserveDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ClanId) || dto.GrupniTreningId <= 0)
                return BadRequest(new { message = "Neispravni podaci." });

            var now = DateTime.UtcNow;

            // Provjera da trening postoji
            var trening = await _context.GrupniTreninzi.FindAsync(dto.GrupniTreningId);
            if (trening == null)
                return NotFound(new { message = "Trening nije pronađen." });

            // Provjera da nije u prošlosti
            if (trening.DatumVrijeme <= now)
                return BadRequest(new { message = "Ne možete rezervisati prošli termin." });

            // Duplikat rezervacije
            var exists = await _context.Rezervacije.AnyAsync(r =>
                r.ClanId == dto.ClanId &&
                r.GrupniTreningId == dto.GrupniTreningId &&
                r.Status == StatusRezervacije.AKTIVNA);
            if (exists)
                return BadRequest(new { message = "Već imate rezervaciju za ovaj termin." });

            // Aktivna članarina
            var hasMembership = await _context.Clanarine.AnyAsync(c =>
                c.ClanId == dto.ClanId &&
                c.Status == StatusClanarine.AKTIVNA &&
                c.DatumPocetka <= now &&
                c.DatumIsteka >= now);
            if (!hasMembership)
                return BadRequest(new { message = "Morate imati aktivnu članarinu da biste rezervisali." });

            // Max jedna rezervacija dnevno
            var dayStart = trening.DatumVrijeme.Date;
            var dayEnd = dayStart.AddDays(1);
            var hasSameDay = await _context.Rezervacije
                .Include(r => r.GrupniTrening)
                .AnyAsync(r =>
                    r.ClanId == dto.ClanId &&
                    r.Status == StatusRezervacije.AKTIVNA &&
                    r.GrupniTrening.DatumVrijeme >= dayStart &&
                    r.GrupniTrening.DatumVrijeme < dayEnd);
            if (hasSameDay)
                return BadRequest(new { message = "Možete imati samo jednu rezervaciju dnevno." });

            // Transakcija — čitaj sa lock-om da izbjegnemo race condition
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Ponovo dohvati trening unutar transakcije
                var treningLocked = await _context.GrupniTreninzi
                    .FirstOrDefaultAsync(t => t.Id == dto.GrupniTreningId);

                if (treningLocked == null)
                    return NotFound(new { message = "Trening nije pronađen." });

                if (!treningLocked.ImaSlobodnihMjesta())
                    return BadRequest(new { message = "Termin je pun." });

                treningLocked.RezervisiMjesto();
                _context.GrupniTreninzi.Update(treningLocked);

                var rezervacija = new Rezervacija
                {
                    ClanId = dto.ClanId,
                    GrupniTreningId = dto.GrupniTreningId,
                    DatumKreiranja = now,
                    Status = StatusRezervacije.AKTIVNA
                };
                _context.Rezervacije.Add(rezervacija);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new
                {
                    message = "Rezervacija uspješna.",
                    id = rezervacija.Id,
                    slobodnaMjesta = treningLocked.SlobodnaMjesta
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new { message = "Greška pri rezervaciji.", detail = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/Api/reservation/{id}
        /// Otkazuje rezervaciju i oslobađa mjesto.
        /// </summary>
        [HttpDelete("reservation/{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var rez = await _context.Rezervacije
                .Include(r => r.GrupniTrening)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rez == null) return NotFound();

            if (rez.Status != StatusRezervacije.AKTIVNA)
                return BadRequest(new { message = "Rezervacija nije aktivna." });

            rez.Otkazi(DateTime.UtcNow);

            if (rez.GrupniTrening != null)
            {
                rez.GrupniTrening.OslobodiMjesto();
                _context.GrupniTreninzi.Update(rez.GrupniTrening);
            }

            _context.Rezervacije.Update(rez);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ────────────────────────────────────────────────────────────────────
        // PLAN TRENINGA
        // ────────────────────────────────────────────────────────────────────

        public class TrainingPlanCreateDto
        {
            public int FitnessCilj { get; set; }
            public double Bmi { get; set; }
            public int BmiKategorija { get; set; }
            public int Intenzitet { get; set; }
            public string? SedmicniPlan { get; set; }
            public string? ClanId { get; set; }
        }

        [HttpPost("training-plan")]
        public async Task<IActionResult> CreateTrainingPlan([FromBody] TrainingPlanCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Podaci nisu poslani." });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) userId = dto.ClanId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Morate biti prijavljeni da biste kreirali plan." });

            var plan = new PlanTreninga
            {
                FitnessCilj = (FitnessCilj)dto.FitnessCilj,
                Bmi = (decimal)dto.Bmi,
                BmiKategorija = (BmiKategorija)dto.BmiKategorija,
                Intenzitet = (Intenzitet)dto.Intenzitet,
                DatumKreiranja = DateTime.UtcNow,
                ClanId = userId
            };

            var sb = new StringBuilder();
            sb.AppendLine($"Cilj: {plan.FitnessCilj}");
            sb.AppendLine($"BMI: {plan.Bmi} ({plan.BmiKategorija})");
            sb.AppendLine($"Intenzitet: {plan.Intenzitet}");
            sb.AppendLine();

            if (plan.FitnessCilj == FitnessCilj.MRSAVLJENJE)
            {
                sb.AppendLine("Ponedjeljak: Kardio 30-45 minuta");
                sb.AppendLine("Utorak: Trening snage - cijelo tijelo");
                sb.AppendLine("Srijeda: Aktivni odmor");
                sb.AppendLine("Četvrtak: HIIT trening");
                sb.AppendLine("Petak: Trening snage - noge i trup");
                sb.AppendLine("Subota: Duži kardio trening");
                sb.AppendLine("Nedjelja: Odmor");
            }
            else if (plan.FitnessCilj == FitnessCilj.JACANJE)
            {
                sb.AppendLine("Ponedjeljak: Prsa i triceps");
                sb.AppendLine("Utorak: Leđa i biceps");
                sb.AppendLine("Srijeda: Odmor");
                sb.AppendLine("Četvrtak: Noge");
                sb.AppendLine("Petak: Ramena i trup");
                sb.AppendLine("Subota: Lagana aktivnost");
                sb.AppendLine("Nedjelja: Odmor");
            }
            else
            {
                sb.AppendLine("Ponedjeljak: Intervalni trening");
                sb.AppendLine("Utorak: Trening snage za izdržljivost");
                sb.AppendLine("Srijeda: Umjereni kardio");
                sb.AppendLine("Četvrtak: Kombinovani trening");
                sb.AppendLine("Petak: Full-body circuit");
                sb.AppendLine("Subota: Aktivni odmor");
                sb.AppendLine("Nedjelja: Odmor");
            }

            plan.SedmicniPlan = sb.ToString();

            _context.PlanoviTreninga.Add(plan);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = plan.Id,
                fitnessCilj = plan.FitnessCilj.ToString(),
                bmi = plan.Bmi,
                bmiKategorija = plan.BmiKategorija.ToString(),
                intenzitet = plan.Intenzitet.ToString(),
                datumKreiranja = plan.DatumKreiranja,
                sedmicniPlan = plan.SedmicniPlan
            });
        }
    }

    // DTO za rezervaciju (umjesto da primamo cijeli model)
    public class ReserveDto
    {
        public string ClanId { get; set; } = string.Empty;
        public int GrupniTreningId { get; set; }
    }
}