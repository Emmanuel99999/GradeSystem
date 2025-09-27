using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Enrollment
        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.Enrollments
                                            .Include(e => e.Student)
                                            .ThenInclude(s => s.Role)
                                            .Include(e => e.Course)
                                            .ThenInclude(c => c.Subject)
                                            .OrderByDescending(e => e.RegistrationDate)
                                            .ToListAsync();
            return View(enrollments);
        }

        // GET: Enrollment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                                           .Include(e => e.Student)
                                           .Include(e => e.Course)
                                               .ThenInclude(c => c.Subject)
                                           .FirstOrDefaultAsync(m => m.EnrollmentId == id);
            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        // GET: Enrollment/Create
        public IActionResult Create()
        {
            ViewData["Students"] = _context.Users
                                           .Include(u => u.Role)
                                           .Where(u => u.Role.RoleName == "Student")
                                           .ToList();
            ViewData["Courses"] = _context.Courses
                                          .Include(c => c.Subject)
                                          .ToList();
            return View();
        }

        // POST: Enrollment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                // Prevent duplicate enrollment
                bool exists = await _context.Enrollments.AnyAsync(e => e.StudentId == enrollment.StudentId && e.CourseId == enrollment.CourseId);
                if (!exists)
                {
                    _context.Add(enrollment);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(enrollment);
        }

        // GET: Enrollment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return NotFound();

            ViewData["Students"] = _context.Users
                                           .Include(u => u.Role)
                                           .Where(u => u.Role.RoleName == "Student")
                                           .ToList();
            ViewData["Courses"] = _context.Courses
                                          .Include(c => c.Subject)
                                          .ToList();
            return View(enrollment);
        }

        // POST: Enrollment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Enrollments.Any(e => e.EnrollmentId == enrollment.EnrollmentId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(enrollment);
        }

        // GET: Enrollment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                                           .Include(e => e.Student)
                                           .Include(e => e.Course)
                                           .FirstOrDefaultAsync(m => m.EnrollmentId == id);
            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        // POST: Enrollment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
