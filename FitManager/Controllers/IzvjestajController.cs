using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitManager.Data;
using FitManager.Models;

namespace FitManager.Controllers
{
    public class IzvjestajController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IzvjestajController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Izvjestaj
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Izvjestaji.Include(i => i.Administrator);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Izvjestaj/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var izvjestaj = await _context.Izvjestaji
                .Include(i => i.Administrator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (izvjestaj == null) return NotFound();

            return View(izvjestaj);
        }

        // GET: Izvjestaj/Create
        public IActionResult Create()
        {
            ViewData["AdministratorId"] = new SelectList(_context.Korisnici, "Id", "Email");
            return View();
        }

        // POST: Izvjestaj/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TipIzvjestaja,DatumOd,DatumDo,DatumGenerisan,Sadrzaj,AdministratorId")] Izvjestaj izvjestaj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(izvjestaj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdministratorId"] = new SelectList(_context.Korisnici, "Id", "Email", izvjestaj.AdministratorId);
            return View(izvjestaj);
        }

        // GET: Izvjestaj/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var izvjestaj = await _context.Izvjestaji.FindAsync(id);
            if (izvjestaj == null) return NotFound();
            ViewData["AdministratorId"] = new SelectList(_context.Korisnici, "Id", "Email", izvjestaj.AdministratorId);
            return View(izvjestaj);
        }

        // POST: Izvjestaj/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TipIzvjestaja,DatumOd,DatumDo,DatumGenerisan,Sadrzaj,AdministratorId")] Izvjestaj izvjestaj)
        {
            if (id != izvjestaj.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(izvjestaj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Izvjestaji.Any(e => e.Id == izvjestaj.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdministratorId"] = new SelectList(_context.Korisnici, "Id", "Email", izvjestaj.AdministratorId);
            return View(izvjestaj);
        }

        // GET: Izvjestaj/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var izvjestaj = await _context.Izvjestaji
                .Include(i => i.Administrator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (izvjestaj == null) return NotFound();

            return View(izvjestaj);
        }

        // POST: Izvjestaj/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var izvjestaj = await _context.Izvjestaji.FindAsync(id);
            if (izvjestaj != null) _context.Izvjestaji.Remove(izvjestaj);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
