using System.Linq;
using System.Threading.Tasks;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;  
using Microsoft.EntityFrameworkCore;

namespace AcademicGradingSystem.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Course
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.AcademicPeriod)
                .Include(c => c.Teacher)
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            return View(courses);
        }

        // GET: Course/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.AcademicPeriod)
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(m => m.CourseId == id);

            if (course == null) return NotFound();

            return View(course);
        }

        // 🔹 Método auxiliar para cargar combos
        private async Task LoadCombosAsync(Course? course = null)
        {
            var subjects = await _context.Subjects
                .AsNoTracking()
                .OrderBy(s => s.SubjectName)
                .ToListAsync();

            var periods = await _context.AcademicPeriods
                .AsNoTracking()
                .OrderBy(p => p.StartDate)
                .ToListAsync();

            var teachers = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Docente" || u.Role.RoleName == "Teacher") 
                .AsNoTracking()
                .OrderBy(u => u.FirstName)
                .ToListAsync();

            ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName", course?.SubjectId);
            ViewBag.Periods = new SelectList(periods, "PeriodId", "Name", course?.PeriodId);
            ViewBag.Teachers = new SelectList(teachers, "UserId", "FirstName", course?.TeacherId);
        }

        // GET: Course/Create
        public async Task<IActionResult> Create()
        {
            await LoadCombosAsync();
            return View(new Course());
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (!ModelState.IsValid)
            {
                await LoadCombosAsync(course);
                return View(course);
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            await LoadCombosAsync(course);
            return View(course);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.CourseId) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadCombosAsync(course);
                return View(course);
            }

            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Courses.Any(e => e.CourseId == course.CourseId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.AcademicPeriod)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.CourseId == id);

            if (course == null) return NotFound();

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
