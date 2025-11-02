using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RoleApiController(ApplicationDbContext context) => _context = context;

        public record RoleDto(int RoleId, string RoleName, string? Description, int UsersCount);
        public record RoleDetailsDto(int RoleId, string RoleName, string? Description, IEnumerable<UserItem> Users);
        public record UserItem(int UserId, string FullName, string Email, bool IsActive);
        public record RoleCreateDto(string RoleName, string? Description);
        public record RoleUpdateDto(int RoleId, string RoleName, string? Description);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll([FromQuery] string? search = null)
        {
            var q = _context.Roles.AsNoTracking().Include(r => r.Users).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(r => r.RoleName.Contains(search));
            var list = await q
                .OrderBy(r => r.RoleName)
                .Select(r => new RoleDto(r.RoleId, r.RoleName, r.Description, r.Users.Count))
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleDetailsDto>> Get(int id)
        {
            var r = await _context.Roles
                .AsNoTracking()
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.RoleId == id);

            if (r == null) return NotFound();

            var dto = new RoleDetailsDto(
                r.RoleId,
                r.RoleName,
                r.Description,
                r.Users.Select(u => new UserItem(u.UserId, u.FirstName + " " + u.LastName, u.Email, u.IsActive))
            );

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create([FromBody] RoleCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var role = new Role
            {
                RoleName = dto.RoleName,
                Description = dto.Description ?? string.Empty
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var created = new RoleDto(role.RoleId, role.RoleName, role.Description, 0);
            return CreatedAtAction(nameof(Get), new { id = created.RoleId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleUpdateDto dto)
        {
            if (id != dto.RoleId) return BadRequest("ID de URL y cuerpo no coinciden.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var exists = await _context.Roles.AnyAsync(r => r.RoleId == id);
            if (!exists) return NotFound();

            var role = new Role
            {
                RoleId = dto.RoleId,
                RoleName = dto.RoleName,
                Description = dto.Description ?? string.Empty
            };

            _context.Entry(role).Property(r => r.RoleName).IsModified = true;
            _context.Entry(role).Property(r => r.Description).IsModified = true;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id:int}/users")]
        public async Task<ActionResult<IEnumerable<UserItem>>> GetUsers(int id)
        {
            var exists = await _context.Roles.AnyAsync(r => r.RoleId == id);
            if (!exists) return NotFound();

            var users = await _context.Users
                .AsNoTracking()
                .Where(u => u.RoleId == id)
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(u => new UserItem(u.UserId, u.FirstName + " " + u.LastName, u.Email, u.IsActive))
                .ToListAsync();

            return Ok(users);
        }
    }
}
