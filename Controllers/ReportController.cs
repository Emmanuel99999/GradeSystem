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
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Report
        public async Task<IActionResult> Index()
        {
            var reports = await _context.Reports
                .Include(r => r.Student)
                .Include(r => r.Course)
                    .ThenInclude(c => c.Subject)
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();

            return View(reports);
        }

        // GET: Report/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.Reports
                .Include(r => r.Student)
                .Include(r => r.Course)
                    .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.ReportId == id);

            if (report == null) return NotFound();

            // Breakdown: detalles del cálculo
            var breakdown = await _context.EvaluationPlans
                .Where(p => p.CourseId == report.CourseId)
                .Select(p => new
                {
                    p.ActivityName,
                    p.Weight,
                    Score = _context.Grades
                        .Where(g => g.PlanId == p.PlanId && g.StudentId == report.StudentId)
                        .OrderByDescending(g => g.DateRecorded)
                        .Select(g => (double?)g.Score)
                        .FirstOrDefault()
                })
                .ToListAsync();

            ViewData["Breakdown"] = breakdown;

            return View(report);
        }

        // 🔹 Método auxiliar para combos (Students + Courses)
        private async Task LoadCombosAsync(int? studentId = null, int? courseId = null)
        {
            var students = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Student" || u.Role.RoleName == "Estudiante") // ajusta al texto real
                .AsNoTracking()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();

            var courses = await _context.Courses
                .Include(c => c.Subject)
                .AsNoTracking()
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            ViewBag.Students = new SelectList(
                students.Select(s => new
                {
                    s.UserId,
                    FullName = s.FirstName + " " + s.LastName + " (" + s.Email + ")"
                }),
                "UserId",
                "FullName",
                studentId
            );

            ViewBag.Courses = new SelectList(
                courses.Select(c => new
                {
                    c.CourseId,
                    Text = c.CourseName + " - " + c.Subject.SubjectName
                }),
                "CourseId",
                "Text",
                courseId
            );
        }

        // GET: Report/Create
        public async Task<IActionResult> Create()
        {
            await LoadCombosAsync();
            return View();
        }

        // POST: Report/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int StudentId, int CourseId)
        {
            var plans = await _context.EvaluationPlans
                .Where(p => p.CourseId == CourseId)
                .ToListAsync();

            // Validaciones
            if (!plans.Any())
                ModelState.AddModelError(string.Empty, "El curso seleccionado no tiene plan de evaluación.");

            double totalWeight = plans.Sum(p => p.Weight);
            if (totalWeight <= 0)
                ModelState.AddModelError(string.Empty, "Los pesos del plan de evaluación no son válidos.");

            if (!ModelState.IsValid)
            {
                await LoadCombosAsync(StudentId, CourseId);
                return View();
            }

            // Cálculo del puntaje final
            double finalScore = 0.0;

            foreach (var plan in plans)
            {
                double? score = await _context.Grades
                    .Where(g => g.PlanId == plan.PlanId && g.StudentId == StudentId)
                    .OrderByDescending(g => g.DateRecorded)
                    .Select(g => (double?)g.Score)
                    .FirstOrDefaultAsync();

                if (score != null)
                {
                    finalScore += score.Value * (plan.Weight / totalWeight);
                }
                // Si no hay nota para ese plan, no aporta nada
            }

            var report = new Report
            {
                StudentId = StudentId,
                CourseId = CourseId,
                GeneratedAt = DateTime.UtcNow,
                FinalGrade = Math.Round(finalScore, 2)
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = report.ReportId });
        }

        // GET: Report/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.Reports
                .Include(r => r.Student)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(m => m.ReportId == id);

            if (report == null) return NotFound();

            return View(report);
        }

        // POST: Report/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report != null)
            {
                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
