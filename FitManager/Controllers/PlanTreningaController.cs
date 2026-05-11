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
    public class PlanTreningaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlanTreningaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PlanTreningas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PlanoviTreninga.Include(p => p.Clan);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PlanTreningas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planTreninga = await _context.PlanoviTreninga
                .Include(p => p.Clan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (planTreninga == null)
            {
                return NotFound();
            }

            return View(planTreninga);
        }

        // GET: PlanTreningas/Create
        public IActionResult Create()
        {
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email");
            return View();
        }

        // POST: PlanTreningas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FitnessCilj,Bmi,BmiKategorija,Intenzitet,SedmicniPlan,DatumKreiranja,ClanId")] PlanTreninga planTreninga)
        {
            if (ModelState.IsValid)
            {
                _context.Add(planTreninga);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", planTreninga.ClanId);
            return View(planTreninga);
        }

        // GET: PlanTreningas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planTreninga = await _context.PlanoviTreninga.FindAsync(id);
            if (planTreninga == null)
            {
                return NotFound();
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", planTreninga.ClanId);
            return View(planTreninga);
        }

        // POST: PlanTreningas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FitnessCilj,Bmi,BmiKategorija,Intenzitet,SedmicniPlan,DatumKreiranja,ClanId")] PlanTreninga planTreninga)
        {
            if (id != planTreninga.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planTreninga);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanTreningaExists(planTreninga.Id))
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
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", planTreninga.ClanId);
            return View(planTreninga);
        }

        // GET: PlanTreningas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planTreninga = await _context.PlanoviTreninga
                .Include(p => p.Clan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (planTreninga == null)
            {
                return NotFound();
            }

            return View(planTreninga);
        }

        // POST: PlanTreningas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planTreninga = await _context.PlanoviTreninga.FindAsync(id);
            if (planTreninga != null)
            {
                _context.PlanoviTreninga.Remove(planTreninga);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanTreningaExists(int id)
        {
            return _context.PlanoviTreninga.Any(e => e.Id == id);
        }
    }
}
