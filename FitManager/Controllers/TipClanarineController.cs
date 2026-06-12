using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitManager.Data;
using FitManager.Models;

namespace FitManager.Controllers
{
    public class TipClanarineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipClanarineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipClanarine
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoviClanarine.ToListAsync());
        }

        // GET: TipClanarine/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tip = await _context.TipoviClanarine.FirstOrDefaultAsync(m => m.Id == id);
            if (tip == null) return NotFound();

            return View(tip);
        }

        // GET: TipClanarine/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipClanarine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,TrajanjeDana,Cijena")] TipClanarine tipClanarine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipClanarine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipClanarine);
        }

        // GET: TipClanarine/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tipClanarine = await _context.TipoviClanarine.FindAsync(id);
            if (tipClanarine == null) return NotFound();
            return View(tipClanarine);
        }

        // POST: TipClanarine/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,TrajanjeDana,Cijena")] TipClanarine tipClanarine)
        {
            if (id != tipClanarine.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipClanarine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TipoviClanarine.Any(e => e.Id == tipClanarine.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tipClanarine);
        }

        // GET: TipClanarine/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tip = await _context.TipoviClanarine.FirstOrDefaultAsync(m => m.Id == id);
            if (tip == null) return NotFound();

            return View(tip);
        }

        // POST: TipClanarine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tip = await _context.TipoviClanarine.FindAsync(id);
            if (tip != null) _context.TipoviClanarine.Remove(tip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
