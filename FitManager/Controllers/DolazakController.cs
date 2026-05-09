using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitManager.Data;
using FitManager.Models;

namespace FitManager.Controllers
{
    public class DolazakController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DolazakController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dolazak
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Dolasci.Include(d => d.Clan);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Dolazak/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dolazak = await _context.Dolasci
                .Include(d => d.Clan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dolazak == null)
            {
                return NotFound();
            }

            return View(dolazak);
        }

        // GET: Dolazak/Create
        public IActionResult Create()
        {
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email");
            return View();
        }

        // POST: Dolazak/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VrijemeDolaska,ClanId")] Dolazak dolazak)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dolazak);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", dolazak.ClanId);
            return View(dolazak);
        }

        // GET: Dolazak/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dolazak = await _context.Dolasci.FindAsync(id);
            if (dolazak == null)
            {
                return NotFound();
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", dolazak.ClanId);
            return View(dolazak);
        }

        // POST: Dolazak/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VrijemeDolaska,ClanId")] Dolazak dolazak)
        {
            if (id != dolazak.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dolazak);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DolazakExists(dolazak.Id))
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
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", dolazak.ClanId);
            return View(dolazak);
        }

        // GET: Dolazak/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dolazak = await _context.Dolasci
                .Include(d => d.Clan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dolazak == null)
            {
                return NotFound();
            }

            return View(dolazak);
        }

        // POST: Dolazak/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dolazak = await _context.Dolasci.FindAsync(id);
            if (dolazak != null)
            {
                _context.Dolasci.Remove(dolazak);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DolazakExists(int id)
        {
            return _context.Dolasci.Any(e => e.Id == id);
        }
    }
}
