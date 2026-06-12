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
    [Route("GrupniTrenings")]
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
            return View("~/Views/GrupniTrenings/Index.cshtml", await applicationDbContext.ToListAsync());
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

            return View("~/Views/GrupniTrenings/Details.cshtml", grupniTrening);
        }

        // GET: GrupniTrenings/Create
        [Authorize(Roles = "ADMIN,TRENER")]
        public IActionResult Create()
        {
            ViewBag.Tipovi = new List<SelectListItem>
            {
                new SelectListItem("Kardio", "KARDIO"),
                new SelectListItem("Snaga", "SNAGA"),
                new SelectListItem("Joga", "JOGA"),
                new SelectListItem("Pilates", "PILATES"),
            };
            return View("~/Views/GrupniTrenings/Create.cshtml", new GrupniTrening());
        }

        // POST: GrupniTrenings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN,TRENER")]
        public async Task<IActionResult> Create([Bind("Naziv,Opis,MaksKapacitet,DatumVrijeme,Trajanje,TipTreninga")] GrupniTrening grupniTrening)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                grupniTrening.TrenerId = userId;
                grupniTrening.SlobodnaMjesta = grupniTrening.MaksKapacitet;

                _context.Add(grupniTrening);
                await _context.SaveChangesAsync();

                if (User.IsInRole("TRENER"))
                    return RedirectToAction("Index", "Rezervacija");
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Tipovi = new List<SelectListItem>
            {
                new SelectListItem("Kardio", "KARDIO"),
                new SelectListItem("Snaga", "SNAGA"),
                new SelectListItem("Joga", "JOGA"),
                new SelectListItem("Pilates", "PILATES"),
            };
            return View("~/Views/GrupniTrenings/Create.cshtml", grupniTrening);
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
            return View("~/Views/GrupniTrenings/Edit.cshtml", grupniTrening);
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
            return View("~/Views/GrupniTrenings/Edit.cshtml", grupniTrening);
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

            return View("~/Views/GrupniTrenings/Delete.cshtml", grupniTrening);
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
