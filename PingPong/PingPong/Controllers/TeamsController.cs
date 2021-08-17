﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PingPong.Models;

namespace PingPong.Controllers
{
    public class TeamsController : Controller
    {
        private readonly PingPongContext _context;

        public TeamsController(PingPongContext context)
        {
            _context = context;
        }

        // GET: Teams
        [Route("teams", Name = "teams")]
        public async Task<IActionResult> Index()
        {
            var pingPongContext = _context.Teams.Include(t => t.PlayerANavigation).Include(t => t.PlayerBNavigation);
            return View(await pingPongContext.ToListAsync());
        }

        // GET: Teams/Details/5
        [Route("teams/{id:}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.PlayerANavigation)
                .Include(t => t.PlayerBNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            var teamGames = await _context.Games
                .Where(g => (g.TeamA == id || g.TeamB == id)).ToListAsync();

            int teamWins = teamGames.Count(g => g.Victor == id);

            int teamLosses = teamGames.Count(g => g.Victor != id);

            var teamWinRatio = new { percentage = (teamWins / teamLosses), reducedTotal = $"{reduceFraction(teamWins, teamLosses)}", teamWins = teamWins, teamLosses = teamLosses };

            ViewBag.teamWinRatio = teamWinRatio;
            ViewBag.teamGames = teamGames;

            static string reduceFraction(int x, int y)
            {
                int d;
                d = __gcd(x, y);

                x = x / d;
                y = y / d;

            return $"{x}:{y}";
            }

            static int __gcd(int a, int b)
            {
                if (b == 0)
                    return a;
                return __gcd(b, a % b);

            }



            return View(team);
        }

        // GET: Teams/Create
        [Route("teams/create")]
        public IActionResult Create()
        {
            ViewData["PlayerA"] = new SelectList(_context.Players, "Id", "FirstName");
            ViewData["PlayerB"] = new SelectList(_context.Players, "Id", "FirstName");
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Teamname,DateFormed,PlayerA,PlayerB")] Team team)
        {
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlayerA"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerA);
            ViewData["PlayerB"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerB);
            return View(team);
        }

        // GET: Teams/Edit/5
        [Route("teams/edit/{id:}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["PlayerA"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerA);
            ViewData["PlayerB"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerB);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("teams/edit/{id:}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Teamname,DateFormed,PlayerA,PlayerB")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
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
            ViewData["PlayerA"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerA);
            ViewData["PlayerB"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerB);
            return View(team);
        }

        // GET: Teams/Delete/5
        [Route("teams/delete/{id:}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.PlayerANavigation)
                .Include(t => t.PlayerBNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("teams/delete/{id:}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}