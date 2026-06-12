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
    public class GrupniTreningController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GrupniTreningController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GrupniTrenings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GrupniTreninzi.Include(g => g.Trener);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: GrupniTrenings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupniTrening = await _context.GrupniTreninzi
                .Include(g => g.Trener)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (grupniTrening == null)
            {
                return NotFound();
            }

            return View(grupniTrening);
        }

        // GET: GrupniTrenings/Create
        public IActionResult Create()
        {
            ViewData["TrenerId"] = new SelectList(_context.Korisnici, "Id", "Email");
            return View();
        }

        // POST: GrupniTrenings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,Opis,MaksKapacitet,SlobodnaMjesta,DatumVrijeme,Trajanje,TipTreninga,TrenerId")] GrupniTrening grupniTrening)
        {
            if (ModelState.IsValid)
            {
                // Ensure initial available slots are consistent
                if (grupniTrening.SlobodnaMjesta <= 0)
                {
                    grupniTrening.SlobodnaMjesta = grupniTrening.MaksKapacitet;
                }
                if (grupniTrening.SlobodnaMjesta > grupniTrening.MaksKapacitet)
                {
                    grupniTrening.SlobodnaMjesta = grupniTrening.MaksKapacitet;
                }

                _context.Add(grupniTrening);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrenerId"] = new SelectList(_context.Korisnici, "Id", "Email", grupniTrening.TrenerId);
            return View(grupniTrening);
        }

        // GET: GrupniTrenings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupniTrening = await _context.GrupniTreninzi.FindAsync(id);
            if (grupniTrening == null)
            {
                return NotFound();
            }
            ViewData["TrenerId"] = new SelectList(_context.Korisnici, "Id", "Email", grupniTrening.TrenerId);
            return View(grupniTrening);
        }

        // POST: GrupniTrenings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,Opis,MaksKapacitet,SlobodnaMjesta,DatumVrijeme,Trajanje,TipTreninga,TrenerId")] GrupniTrening grupniTrening)
        {
            if (id != grupniTrening.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Normalize available slots
                    if (grupniTrening.SlobodnaMjesta < 0) grupniTrening.SlobodnaMjesta = 0;
                    if (grupniTrening.SlobodnaMjesta > grupniTrening.MaksKapacitet) grupniTrening.SlobodnaMjesta = grupniTrening.MaksKapacitet;

                    _context.Update(grupniTrening);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GrupniTreningExists(grupniTrening.Id))
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
            ViewData["TrenerId"] = new SelectList(_context.Korisnici, "Id", "Email", grupniTrening.TrenerId);
            return View(grupniTrening);
        }

        // GET: GrupniTrenings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupniTrening = await _context.GrupniTreninzi
                .Include(g => g.Trener)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (grupniTrening == null)
            {
                return NotFound();
            }

            return View(grupniTrening);
        }

        // POST: GrupniTrenings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grupniTrening = await _context.GrupniTreninzi.FindAsync(id);
            if (grupniTrening != null)
            {
                _context.GrupniTreninzi.Remove(grupniTrening);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GrupniTreningExists(int id)
        {
            return _context.GrupniTreninzi.Any(e => e.Id == id);
        }
    }
}
