using System.Linq;
using System.Threading.Tasks;
using AcademicGradingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers
{
    public class AcademicPeriodController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicPeriodController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AcademicPeriod
        public async Task<IActionResult> Index()
        {
            var periods = await _context.AcademicPeriods
                                        .OrderByDescending(p => p.StartDate)
                                        .ToListAsync();
            return View(periods);
        }

        // GET: AcademicPeriod/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var period = await _context.AcademicPeriods
                                       .Include(p => p.Courses)
                                       .FirstOrDefaultAsync(m => m.PeriodId == id);
            if (period == null) return NotFound();

            return View(period);
        }

        // GET: AcademicPeriod/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AcademicPeriod/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AcademicPeriod period)
        {
            if (ModelState.IsValid)
            {
                _context.Add(period);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(period);
        }

        // GET: AcademicPeriod/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var period = await _context.AcademicPeriods.FindAsync(id);
            if (period == null) return NotFound();

            return View(period);
        }

        // POST: AcademicPeriod/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AcademicPeriod period)
        {
            if (id != period.PeriodId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(period);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AcademicPeriods.Any(e => e.PeriodId == period.PeriodId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(period);
        }

        // GET: AcademicPeriod/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var period = await _context.AcademicPeriods
                                       .FirstOrDefaultAsync(m => m.PeriodId == id);
            if (period == null) return NotFound();

            return View(period);
        }

        // POST: AcademicPeriod/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var period = await _context.AcademicPeriods.FindAsync(id);
            if (period != null)
            {
                _context.AcademicPeriods.Remove(period);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
