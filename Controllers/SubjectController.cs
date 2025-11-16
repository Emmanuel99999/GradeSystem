using System.Linq;
using System.Threading.Tasks;
using AcademicGradingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AcademicGradingSystem.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subject
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Program)
                .OrderBy(s => s.SubjectName)
                .ToListAsync();

            return View(subjects);
        }

        // GET: Subject/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _context.Subjects
                .Include(s => s.Program)
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(m => m.SubjectId == id);

            if (subject == null) return NotFound();

            return View(subject);
        }

        // GET: Subject/Create
        public async Task<IActionResult> Create()
        {
            var programs = await _context.AcademicProgram
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Programs = new SelectList(programs, "ProgramId", "ProgramName");

            return View(new Subject());
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject)
        {
            if (!ModelState.IsValid)
            {
                var programs = await _context.AcademicProgram
                    .AsNoTracking()
                    .ToListAsync();

                ViewBag.Programs = new SelectList(programs, "ProgramId", "ProgramName", subject.ProgramId);

                return View(subject);
            }

            _context.Add(subject);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();

            var programs = await _context.AcademicProgram
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Programs = new SelectList(programs, "ProgramId", "ProgramName", subject.ProgramId);

            return View(subject);
        }

        // POST: Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subject subject)
        {
            if (id != subject.SubjectId) return NotFound();

            if (!ModelState.IsValid)
            {
                var programs = await _context.AcademicProgram
                    .AsNoTracking()
                    .ToListAsync();

                ViewBag.Programs = new SelectList(programs, "ProgramId", "ProgramName", subject.ProgramId);

                return View(subject);
            }

            try
            {
                _context.Update(subject);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Subjects.Any(e => e.SubjectId == subject.SubjectId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Subject/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _context.Subjects
                .Include(s => s.Program)
                .FirstOrDefaultAsync(m => m.SubjectId == id);

            if (subject == null) return NotFound();

            return View(subject);
        }

        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
