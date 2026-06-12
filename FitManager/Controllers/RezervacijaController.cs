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
    using Microsoft.AspNetCore.Authorization;

    public class RezervacijaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RezervacijaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rezervacija
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Index()
        {
            // CLAN users should see the calendar/client reservation UI (no list of all reservations)
            if (User.IsInRole("CLAN"))
            {
                return View();
            }

            // TRENER and ADMIN can view the list of reservations with attendee counts
            if (User.IsInRole("TRENER") || User.IsInRole("ADMIN"))
            {
                var treninzi = await _context.GrupniTreninzi
                    .Include(t => t.Trener)
                    .OrderByDescending(t => t.DatumVrijeme)
                    .ToListAsync();

                var rezervacije = await _context.Rezervacije
                    .Where(r => r.Status == StatusRezervacije.AKTIVNA)
                    .GroupBy(r => r.GrupniTreningId)
                    .ToDictionaryAsync(g => g.Key, g => g.Count());

                ViewBag.RezervacijeCount = rezervacije;
                return View("TrenerPregled", treninzi);
            }

            // other authenticated roles are forbidden from this page
            return Forbid();
        }

        // GET: Rezervacija/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rezervacija = await _context.Rezervacije
                .Include(r => r.Clan)
                .Include(r => r.GrupniTrening)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rezervacija == null)
            {
                return NotFound();
            }

            return View(rezervacija);
        }

        // GET: Rezervacija/Create
        [Authorize(Roles = "ADMIN")]
        public IActionResult Create()
        {
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email");
            ViewData["GrupniTreningId"] = new SelectList(_context.GrupniTreninzi, "Id", "Naziv");
            return View();
        }

        // POST: Rezervacija/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create([Bind("Id,ClanId,GrupniTreningId,DatumKreiranja,Status")] Rezervacija rezervacija)
        {
            if (ModelState.IsValid)
            {
                var trening = await _context.GrupniTreninzi.FindAsync(rezervacija.GrupniTreningId);
                if (trening == null)
                {
                    ModelState.AddModelError(string.Empty, "Odabrani trening ne postoji.");
                }
                else
                {
                    // check capacity
                    if (!trening.ImaSlobodnihMjesta())
                    {
                        ModelState.AddModelError(string.Empty, "Termin je pun.");
                    }
                    else
                    {
                        // ensure unique reservation for user+trening
                        var exists = await _context.Rezervacije.AnyAsync(r => r.ClanId == rezervacija.ClanId && r.GrupniTreningId == rezervacija.GrupniTreningId && r.Status == StatusRezervacije.AKTIVNA);
                        if (exists)
                        {
                            ModelState.AddModelError(string.Empty, "Već imate rezervaciju za ovaj termin.");
                        }
                        else
                        {
                                trening.RezervisiMjesto();
                                _context.Update(trening);
                                rezervacija.DatumKreiranja = DateTime.Now;
                                _context.Add(rezervacija);
                                await _context.SaveChangesAsync();
                                return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", rezervacija.ClanId);
            ViewData["GrupniTreningId"] = new SelectList(_context.GrupniTreninzi, "Id", "Naziv", rezervacija.GrupniTreningId);
            return View(rezervacija);
        }

        // GET: Rezervacija/Edit/5
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rezervacija = await _context.Rezervacije.FindAsync(id);
            if (rezervacija == null)
            {
                return NotFound();
            }
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", rezervacija.ClanId);
            ViewData["GrupniTreningId"] = new SelectList(_context.GrupniTreninzi, "Id", "Naziv", rezervacija.GrupniTreningId);
            return View(rezervacija);
        }

        // POST: Rezervacija/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClanId,GrupniTreningId,DatumKreiranja,Status")] Rezervacija rezervacija)
        {
            if (id != rezervacija.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rezervacija);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RezervacijaExists(rezervacija.Id))
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
            ViewData["ClanId"] = new SelectList(_context.Korisnici, "Id", "Email", rezervacija.ClanId);
            ViewData["GrupniTreningId"] = new SelectList(_context.GrupniTreninzi, "Id", "Naziv", rezervacija.GrupniTreningId);
            return View(rezervacija);
        }

        // GET: Rezervacija/Delete/5
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rezervacija = await _context.Rezervacije
                .Include(r => r.Clan)
                .Include(r => r.GrupniTrening)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rezervacija == null)
            {
                return NotFound();
            }

            return View(rezervacija);
        }

        // POST: Rezervacija/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")] // only admin can delete via MVC
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rezervacija = await _context.Rezervacije.FindAsync(id);
            if (rezervacija != null)
            {
                _context.Rezervacije.Remove(rezervacija);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RezervacijaExists(int id)
        {
            return _context.Rezervacije.Any(e => e.Id == id);
        }
    }
}
