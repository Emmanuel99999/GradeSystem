using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers
{
    public class ProgramController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgramController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Program
        public async Task<IActionResult> Index()
        {
            var programs = await _context.Programs
                                         .Include(p => p.Subjects)
                                         .OrderBy(p => p.ProgramName)
                                         .ToListAsync();
            return View(programs);
        }

        // GET: Program/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var program = await _context.Programs
                                        .Include(p => p.Subjects)
                                        .FirstOrDefaultAsync(m => m.ProgramId == id);
            if (program == null) return NotFound();

            return View(program);
        }

        // GET: Program/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Program/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Program program)
        {
            if (ModelState.IsValid)
            {
                _context.Add(program);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(program);
        }

        // GET: Program/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var program = await _context.Programs.FindAsync(id);
            if (program == null) return NotFound();

            return View(program);
        }

        // POST: Program/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.Program program)
        {
            if (id != program.ProgramId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(program);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Programs.Any(e => e.ProgramId == program.ProgramId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(program);
        }

        // GET: Program/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var program = await _context.Programs
                                        .FirstOrDefaultAsync(m => m.ProgramId == id);
            if (program == null) return NotFound();

            return View(program);
        }

        // POST: Program/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var program = await _context.Programs.FindAsync(id);
            if (program != null)
            {
                _context.Programs.Remove(program);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
