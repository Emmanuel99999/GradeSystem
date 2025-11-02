using System.Linq;
using System.Threading.Tasks;
using AcademicGradingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers
{
    public class ProgramApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgramApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Program
        public async Task<IActionResult> Index()
        {
            var AcademicProgram = await _context.AcademicProgram
                                         .Include(p => p.Subjects)
                                         .OrderBy(p => p.ProgramName)
                                         .ToListAsync();
            return View(AcademicProgram);
        }

        // GET: Program/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var program = await _context.AcademicProgram
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
        public async Task<IActionResult> Create(Models.AcademicProgram program)
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

            var program = await _context.AcademicProgram.FindAsync(id);
            if (program == null) return NotFound();

            return View(program);
        }

        // POST: Program/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.AcademicProgram program)
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
                    if (!_context.AcademicProgram.Any(e => e.ProgramId == program.ProgramId))
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

            var program = await _context.AcademicProgram
                                        .FirstOrDefaultAsync(m => m.ProgramId == id);
            if (program == null) return NotFound();

            return View(program);
        }

        // POST: Program/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var program = await _context.AcademicProgram.FindAsync(id);
            if (program != null)
            {
                _context.AcademicProgram.Remove(program);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
