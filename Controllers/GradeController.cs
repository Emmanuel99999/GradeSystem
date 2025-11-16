using System;
using System.Linq;
using System.Threading.Tasks;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AcademicGradingSystem.Controllers
{
    public class GradeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Grade
        public async Task<IActionResult> Index()
        {
            var grades = await _context.Grades
                .Include(g => g.EvaluationPlan)
                    .ThenInclude(p => p.Course)
                .Include(g => g.Student)
                .OrderBy(g => g.Student.LastName)
                .ThenBy(g => g.Student.FirstName)
                .ToListAsync();

            return View(grades);
        }

        // GET: Grade/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var grade = await _context.Grades
                .Include(g => g.EvaluationPlan)
                    .ThenInclude(p => p.Course)
                .Include(g => g.Student)
                .FirstOrDefaultAsync(m => m.GradeId == id);

            if (grade == null) return NotFound();

            return View(grade);
        }

        // 🔹 Método auxiliar para cargar combos
        private async Task LoadCombosAsync(Grade? grade = null)
        {
            var students = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Student" || u.Role.RoleName == "Estudiante") // 
                .AsNoTracking()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();

            var plans = await _context.EvaluationPlans
                .Include(p => p.Course)
                    .ThenInclude(c => c.Subject)
                .AsNoTracking()
                .OrderBy(p => p.Course.CourseName)
                .ThenBy(p => p.ActivityName)
                .ToListAsync();

            ViewBag.Students = new SelectList(
                students.Select(s => new
                {
                    s.UserId,
                    FullName = s.FirstName + " " + s.LastName + " (" + s.Email + ")"
                }),
                "UserId",
                "FullName",
                grade?.StudentId
            );

            ViewBag.Plans = new SelectList(
                plans.Select(p => new
                {
                    p.PlanId,
                    Text = p.Course.CourseName + " - " + p.ActivityName
                }),
                "PlanId",
                "Text",
                grade?.PlanId
            );
        }

        // GET: Grade/Create
        public async Task<IActionResult> Create()
        {
            await LoadCombosAsync();
            return View(new Grade());
        }

        // POST: Grade/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Grade grade)
        {
            if (!ModelState.IsValid)
            {
                await LoadCombosAsync(grade);
                return View(grade);
            }

            // Si no capturas la fecha en el formulario, puedes asignarla aquí
            if (grade.DateRecorded == default)
            {
                grade.DateRecorded = DateTime.UtcNow;
            }

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Grade/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return NotFound();

            await LoadCombosAsync(grade);
            return View(grade);
        }

        // POST: Grade/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Grade grade)
        {
            if (id != grade.GradeId) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadCombosAsync(grade);
                return View(grade);
            }

            try
            {
                _context.Update(grade);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Grades.Any(e => e.GradeId == grade.GradeId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Grade/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var grade = await _context.Grades
                .Include(g => g.EvaluationPlan)
                    .ThenInclude(p => p.Course)
                .Include(g => g.Student)
                .FirstOrDefaultAsync(m => m.GradeId == id);

            if (grade == null) return NotFound();

            return View(grade);
        }

        // POST: Grade/Delete/5
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
