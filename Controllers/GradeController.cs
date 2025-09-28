using System.Linq;
using System.Threading.Tasks;
using AcademicGradingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers
{
    public class GradeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var grades = await _context.Grades
                                       .Include(g => g.EvaluationPlan)
                                       .Include(g => g.Student)
                                       .ToListAsync();
            return View(grades);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var grade = await _context.Grades
                                      .Include(g => g.EvaluationPlan)
                                      .Include(g => g.Student)
                                      .FirstOrDefaultAsync(m => m.GradeId == id);
            if (grade == null) return NotFound();
            return View(grade);
        }

        public IActionResult Create()
        {
            ViewData["Plans"] = _context.EvaluationPlans.Include(p => p.Course).ToList();
            ViewData["Students"] = _context.Users.Where(u => u.Role.RoleName == "Student").ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Grade grade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(grade);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return NotFound();
            ViewData["Plans"] = _context.EvaluationPlans.ToList();
            ViewData["Students"] = _context.Users.Where(u => u.Role.RoleName == "Student").ToList();
            return View(grade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Grade grade)
        {
            if (id != grade.GradeId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Grades.Any(e => e.GradeId == grade.GradeId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(grade);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var grade = await _context.Grades
                                      .Include(g => g.EvaluationPlan)
                                      .Include(g => g.Student)
                                      .FirstOrDefaultAsync(m => m.GradeId == id);
            if (grade == null) return NotFound();
            return View(grade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
