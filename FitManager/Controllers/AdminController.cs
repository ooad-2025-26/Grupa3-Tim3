using Microsoft.AspNetCore.Mvc;
using FitManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FitManager.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context){ _context = context; }

        public async Task<IActionResult> Index()
        {
            var usersCount = await _context.Korisnici.CountAsync();
            var trainingsCount = await _context.GrupniTreninzi.CountAsync();
            var reservationsCount = await _context.Rezervacije.CountAsync();

            ViewBag.Users = usersCount;
            ViewBag.Trainings = trainingsCount;
            ViewBag.Reservations = reservationsCount;
            return View();
        }
    }
}
