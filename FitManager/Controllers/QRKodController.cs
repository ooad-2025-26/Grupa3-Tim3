using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitManager.Data;
using FitManager.Models;

namespace FitManager.Controllers
{
    public class QRKodController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QRKodController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: QRKod
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.QRKodovi.Include(q => q.Clan);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: QRKod/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var qrKod = await _context.QRKodovi
                .Include(q => q.Clan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qrKod == null) return NotFound();

            return View(qrKod);
        }

        // GET: QRKod/Create
        public IActionResult Create()
        {
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email");
            return View();
        }

        // POST: QRKod/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Kod,DatumGenerisanja,Aktivan,ClanId")] QRKod qrKod)
        {
            if (ModelState.IsValid)
            {
                _context.Add(qrKod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", qrKod.ClanId);
            return View(qrKod);
        }

        // GET: QRKod/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var qrKod = await _context.QRKodovi.FindAsync(id);
            if (qrKod == null) return NotFound();
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", qrKod.ClanId);
            return View(qrKod);
        }

        // POST: QRKod/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Kod,DatumGenerisanja,Aktivan,ClanId")] QRKod qrKod)
        {
            if (id != qrKod.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(qrKod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.QRKodovi.Any(e => e.Id == qrKod.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", qrKod.ClanId);
            return View(qrKod);
        }

        // GET: QRKod/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var qrKod = await _context.QRKodovi
                .Include(q => q.Clan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qrKod == null) return NotFound();

            return View(qrKod);
        }

        // POST: QRKod/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var qrKod = await _context.QRKodovi.FindAsync(id);
            if (qrKod != null) _context.QRKodovi.Remove(qrKod);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
