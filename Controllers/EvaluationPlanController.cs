using System.Linq;
using System.Threading.Tasks;
using AcademicGradingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers
{
    public class EvaluationPlanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvaluationPlanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EvaluationPlan
        public async Task<IActionResult> Index()
        {
            var plans = await _context.EvaluationPlans
                .Include(p => p.Course)
                    .ThenInclude(c => c.Subject)
                .ToListAsync();

            return View(plans);
        }

        // GET: EvaluationPlan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.EvaluationPlans
                .Include(p => p.Course)
                    .ThenInclude(c => c.Subject)
                .Include(p => p.Grades)
                .FirstOrDefaultAsync(m => m.PlanId == id);

            if (plan == null) return NotFound();

            return View(plan);
        }

        // GET: EvaluationPlan/Create
        public IActionResult Create()
        {
            ViewData["Courses"] = _context.Courses
                .Include(c => c.Subject)
                .ToList();

            return View();
        }

        // POST: EvaluationPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EvaluationPlan plan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recargar combos si hay error
            ViewData["Courses"] = _context.Courses
                .Include(c => c.Subject)
                .ToList();

            return View(plan);
        }

        // GET: EvaluationPlan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.EvaluationPlans.FindAsync(id);
            if (plan == null) return NotFound();

            ViewData["Courses"] = _context.Courses
                .Include(c => c.Subject)
                .ToList();

            return View(plan);
        }

        // POST: EvaluationPlan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EvaluationPlan plan)
        {
            if (id != plan.PlanId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.EvaluationPlans.Any(e => e.PlanId == plan.PlanId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // Recargar combos si hay error
            ViewData["Courses"] = _context.Courses
                .Include(c => c.Subject)
                .ToList();

            return View(plan);
        }

        // GET: EvaluationPlan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.EvaluationPlans
                .Include(p => p.Course)
                    .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.PlanId == id);

            if (plan == null) return NotFound();

            return View(plan);
        }

        // POST: EvaluationPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _context.EvaluationPlans.FindAsync(id);
            if (plan != null)
            {
                _context.EvaluationPlans.Remove(plan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
