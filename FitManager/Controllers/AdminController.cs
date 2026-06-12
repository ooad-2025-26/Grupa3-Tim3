using Microsoft.AspNetCore.Mvc;
using FitManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FitManager.Models;

namespace FitManager.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context) { _context = context; }

        public async Task<IActionResult> Index()
        {
            var usersCount = await _context.Korisnici.CountAsync();
            var treningCount = await _context.GrupniTreninzi.CountAsync();
            var rezervacijeCount = await _context.Rezervacije.CountAsync();
            var aktivneClanarineCount = await _context.Clanarine
                .CountAsync(c => c.Status == StatusClanarine.AKTIVNA);
            var tipoviCount = await _context.TipoviClanarine.CountAsync();
            var planoviCount = await _context.PlanoviTreninga.CountAsync();

            var najaktivniji = await _context.Rezervacije
                .Where(r => r.Status == StatusRezervacije.AKTIVNA)
                .GroupBy(r => r.Clan)
                .Select(g => new { Clan = g.Key, Broj = g.Count() })
                .OrderByDescending(x => x.Broj)
                .Take(5)
                .ToListAsync();

            ViewBag.Users = usersCount;
            ViewBag.Trainings = treningCount;
            ViewBag.Reservations = rezervacijeCount;
            ViewBag.AktivneClanarine = aktivneClanarineCount;
            ViewBag.Tipovi = tipoviCount;
            ViewBag.Planovi = planoviCount;
            ViewBag.Najaktivniji = najaktivniji;
            return View();
        }
    }
}
