using System;
using System.Linq;
using System.Threading.Tasks;
using AcademicGradingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers
{
    public class ReportApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportApiController(ApplicationDbContext context)
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
                                       .Include(r => r.Course).ThenInclude(c => c.Subject)
                                       .FirstOrDefaultAsync(m => m.ReportId == id);
            if (report == null) return NotFound();

            // Bring breakdown for display
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
                                          }).ToListAsync();

            ViewData["Breakdown"] = breakdown;
            return View(report);
        }

        // GET: Report/Create  (Generate form)
        public IActionResult Create()
        {
            ViewData["Students"] = _context.Users
                                            .Include(u => u.Role)
                                            .Where(u => u.Role.RoleName == "Student")
                                            .OrderBy(u => u.FirstName)
                                            .ToList();
            ViewData["Courses"] = _context.Courses
                                           .Include(c => c.Subject)
                                           .OrderBy(c => c.CourseName)
                                           .ToList();
            return View();
        }

        // POST: Report/Create (Generate and persist)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int StudentId, int CourseId)
        {
            // Compute final grade from plans and grades
            var plans = await _context.EvaluationPlans
                                      .Where(p => p.CourseId == CourseId)
                                      .ToListAsync();

            if (!plans.Any())
            {
                ModelState.AddModelError(string.Empty, "The selected course has no evaluation plan.");
            }

            // Sum weights (expecting 100)
            double totalWeight = plans.Sum(p => p.Weight);
            if (totalWeight <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid evaluation plan weights.");
            }

            if (!ModelState.IsValid)
            {
                // Reload dropdowns
                ViewData["Students"] = _context.Users.Include(u => u.Role).Where(u => u.Role.RoleName == "Student").ToList();
                ViewData["Courses"] = _context.Courses.Include(c => c.Subject).ToList();
                return View();
            }

            // Weighted score using latest grade per plan
            double finalScore = 0.0;
            foreach (var plan in plans)
            {
                var score = await _context.Grades
                                          .Where(g => g.PlanId == plan.PlanId && g.StudentId == StudentId)
                                          .OrderByDescending(g => g.DateRecorded)
                                          .Select(g => (double?)g.Score)
                                          .FirstOrDefaultAsync();

                if (score.HasValue)
                {
                    finalScore += (score.Value * (plan.Weight / totalWeight));
                }
                // If missing grade, treat as zero contribution (or adjust as needed)
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
