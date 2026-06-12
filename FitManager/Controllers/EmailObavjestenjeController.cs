using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitManager.Data;
using FitManager.Models;

namespace FitManager.Controllers
{
    public class EmailObavjestenjeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmailObavjestenjeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmailObavjestenje
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EmailObavjestenja.Include(e => e.Clanarina);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EmailObavjestenje/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var obavjestenje = await _context.EmailObavjestenja
                .Include(e => e.Clanarina)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obavjestenje == null) return NotFound();

            return View(obavjestenje);
        }

        // GET: EmailObavjestenje/Create
        public IActionResult Create()
        {
            ViewData["ClanarinaId"] = new SelectList(_context.Clanarine, "Id", "Id");
            return View();
        }

        // POST: EmailObavjestenje/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DatumSlanja,Status,Sadrzaj,PokusajSlanja,ClanarinaId")] EmailObavjestenje emailObavjestenje)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emailObavjestenje);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanarinaId"] = new SelectList(_context.Clanarine, "Id", "Id", emailObavjestenje.ClanarinaId);
            return View(emailObavjestenje);
        }

        // GET: EmailObavjestenje/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var obavjestenje = await _context.EmailObavjestenja.FindAsync(id);
            if (obavjestenje == null) return NotFound();
            ViewData["ClanarinaId"] = new SelectList(_context.Clanarine, "Id", "Id", obavjestenje.ClanarinaId);
            return View(obavjestenje);
        }

        // POST: EmailObavjestenje/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatumSlanja,Status,Sadrzaj,PokusajSlanja,ClanarinaId")] EmailObavjestenje emailObavjestenje)
        {
            if (id != emailObavjestenje.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emailObavjestenje);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.EmailObavjestenja.Any(e => e.Id == emailObavjestenje.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanarinaId"] = new SelectList(_context.Clanarine, "Id", "Id", emailObavjestenje.ClanarinaId);
            return View(emailObavjestenje);
        }

        // GET: EmailObavjestenje/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var obavjestenje = await _context.EmailObavjestenja
                .Include(e => e.Clanarina)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obavjestenje == null) return NotFound();

            return View(obavjestenje);
        }

        // POST: EmailObavjestenje/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obavjestenje = await _context.EmailObavjestenja.FindAsync(id);
            if (obavjestenje != null) _context.EmailObavjestenja.Remove(obavjestenje);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
