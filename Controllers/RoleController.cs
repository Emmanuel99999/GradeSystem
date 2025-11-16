using System.Linq;
using System.Threading.Tasks;
using AcademicGradingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Role
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles
                .OrderBy(r => r.RoleName)
                .ToListAsync();

            return View(roles);
        }

        // GET: Role/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var role = await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null) return NotFound();

            return View(role);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();

            return View(role);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Role role)
        {
            if (id != role.RoleId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Roles.Any(e => e.RoleId == role.RoleId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null) return NotFound();

            return View(role);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
