using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitManager.Data;
using FitManager.Models;

namespace FitManager.Controllers
{
    public class ClanarinaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClanarinaController(ApplicationDbContext context)
        {
            _context = context;
        }


        [Authorize]
        public async Task<IActionResult> MojaClanarina()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var now = DateTime.UtcNow;

            var aktivnaClanarina = await _context.Clanarine
                .Include(c => c.TipClanarine)
                .Where(c => c.ClanId == userId
                         && c.Status == StatusClanarine.AKTIVNA
                         && c.DatumIsteka >= now)
                .OrderByDescending(c => c.DatumPocetka)
                .FirstOrDefaultAsync();

            if (aktivnaClanarina != null)
            {
                return View("MojaClanarina", aktivnaClanarina);
            }

            var tipovi = await _context.TipoviClanarine.ToListAsync();
            ViewBag.Tipovi = tipovi;
            return View("MojaClanarina");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kupi(int tipClanarineId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var tip = await _context.TipoviClanarine.FindAsync(tipClanarineId);
            if (tip == null)
                return NotFound();

            var now = DateTime.UtcNow;

            var clanarina = new Clanarina
            {
                ClanId = userId,
                TipClanarineId = tipClanarineId,
                DatumPocetka = now,
                DatumIsteka = now.AddDays(tip.TrajanjeDana),
                Cijena = tip.Cijena,
                Status = StatusClanarine.AKTIVNA,
                ObavjestenjePoslano = false
            };

            _context.Add(clanarina);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MojaClanarina));
        }
        // GET: Clanarina
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Clanarine.Include(c => c.Clan).Include(c => c.TipClanarine);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clanarina/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clanarina = await _context.Clanarine
                .Include(c => c.Clan)
                .Include(c => c.TipClanarine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clanarina == null)
            {
                return NotFound();
            }

            return View(clanarina);
        }

        // GET: Clanarina/Create
        public IActionResult Create()
        {
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email");
            ViewData["TipClanarineId"] = new SelectList(_context.TipoviClanarine, "Id", "Naziv");
            return View();
        }

        // POST: Clanarina/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DatumPocetka,DatumIsteka,Cijena,Status,ObavjestenjePoslano,ClanId,TipClanarineId")] Clanarina clanarina)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clanarina);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", clanarina.ClanId);
            ViewData["TipClanarineId"] = new SelectList(_context.TipoviClanarine, "Id", "Naziv", clanarina.TipClanarineId);
            return View(clanarina);
        }

        // GET: Clanarina/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clanarina = await _context.Clanarine.FindAsync(id);
            if (clanarina == null)
            {
                return NotFound();
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", clanarina.ClanId);
            ViewData["TipClanarineId"] = new SelectList(_context.TipoviClanarine, "Id", "Naziv", clanarina.TipClanarineId);
            return View(clanarina);
        }

        // POST: Clanarina/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatumPocetka,DatumIsteka,Cijena,Status,ObavjestenjePoslano,ClanId,TipClanarineId")] Clanarina clanarina)
        {
            if (id != clanarina.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clanarina);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClanarinaExists(clanarina.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", clanarina.ClanId);
            ViewData["TipClanarineId"] = new SelectList(_context.TipoviClanarine, "Id", "Naziv", clanarina.TipClanarineId);
            return View(clanarina);
        }

        // GET: Clanarina/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clanarina = await _context.Clanarine
                .Include(c => c.Clan)
                .Include(c => c.TipClanarine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clanarina == null)
            {
                return NotFound();
            }

            return View(clanarina);
        }

        // POST: Clanarina/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clanarina = await _context.Clanarine.FindAsync(id);
            if (clanarina != null)
            {
                _context.Clanarine.Remove(clanarina);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClanarinaExists(int id)
        {
            return _context.Clanarine.Any(e => e.Id == id);
        }
    }
}
