using System.Linq;
using System.Threading.Tasks;
using AcademicGradingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AcademicGradingSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();

            return View(users);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // GET: User/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new SelectList(
                await _context.Roles.AsNoTracking().ToListAsync(),
                "RoleId", "RoleName"
            );

            return View(new User());
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(
                    await _context.Roles.AsNoTracking().ToListAsync(),
                    "RoleId", "RoleName",
                    user.RoleId
                );

                return View(user);
            }

            // ⚠ RECOMENDACIÓN: Aquí deberías hashear la contraseña antes de guardar.
            _context.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = new SelectList(
                await _context.Roles.AsNoTracking().ToListAsync(),
                "RoleId", "RoleName",
                user.RoleId
            );

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(
                    await _context.Roles.AsNoTracking().ToListAsync(),
                    "RoleId", "RoleName",
                    user.RoleId
                );

                return View(user);
            }

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Users.AnyAsync(e => e.UserId == user.UserId))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
