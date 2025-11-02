using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SubjectApiController(ApplicationDbContext context) => _context = context;

        public record SubjectDto(int SubjectId, string SubjectName, string Code, int ProgramId, string ProgramName, int CoursesCount);
        public record SubjectCreateDto(string SubjectName, string Code, int ProgramId);
        public record SubjectUpdateDto(int SubjectId, string SubjectName, string Code, int ProgramId);
        public record ProgramItem(int ProgramId, string ProgramName);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAll(
            [FromQuery] string? search = null,
            [FromQuery] int? programId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var q = _context.Subjects
                .AsNoTracking()
                .Include(s => s.Program)
                .Include(s => s.Courses)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(s => s.SubjectName.Contains(search) || s.Code.Contains(search));

            if (programId.HasValue)
                q = q.Where(s => s.ProgramId == programId.Value);

            var items = await q
                .OrderBy(s => s.SubjectName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SubjectDto(
                    s.SubjectId,
                    s.SubjectName,
                    s.Code,
                    s.ProgramId,
                    s.Program.ProgramName,
                    s.Courses.Count))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubjectDto>> Get(int id)
        {
            var s = await _context.Subjects
                .AsNoTracking()
                .Include(x => x.Program)
                .Include(x => x.Courses)
                .FirstOrDefaultAsync(x => x.SubjectId == id);

            if (s == null) return NotFound();

            return Ok(new SubjectDto(
                s.SubjectId,
                s.SubjectName,
                s.Code,
                s.ProgramId,
                s.Program.ProgramName,
                s.Courses.Count));
        }

        [HttpPost]
        public async Task<ActionResult<SubjectDto>> Create([FromBody] SubjectCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var programExists = await _context.AcademicProgram.AnyAsync(p => p.ProgramId == dto.ProgramId);
            if (!programExists) return BadRequest("ProgramId no existe.");

            var subject = new Subject
            {
                SubjectName = dto.SubjectName,
                Code = dto.Code,
                ProgramId = dto.ProgramId
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            var created = await _context.Subjects
                .AsNoTracking()
                .Include(s => s.Program)
                .Include(s => s.Courses)
                .FirstAsync(s => s.SubjectId == subject.SubjectId);

            var response = new SubjectDto(
                created.SubjectId,
                created.SubjectName,
                created.Code,
                created.ProgramId,
                created.Program.ProgramName,
                created.Courses.Count);

            return CreatedAtAction(nameof(Get), new { id = response.SubjectId }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SubjectUpdateDto dto)
        {
            if (id != dto.SubjectId) return BadRequest("ID de URL y cuerpo no coinciden.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var exists = await _context.Subjects.AnyAsync(s => s.SubjectId == id);
            if (!exists) return NotFound();

            var programExists = await _context.AcademicProgram.AnyAsync(p => p.ProgramId == dto.ProgramId);
            if (!programExists) return BadRequest("ProgramId no existe.");

            var subject = new Subject
            {
                SubjectId = dto.SubjectId,
                SubjectName = dto.SubjectName,
                Code = dto.Code,
                ProgramId = dto.ProgramId
            };

            _context.Entry(subject).Property(s => s.SubjectName).IsModified = true;
            _context.Entry(subject).Property(s => s.Code).IsModified = true;
            _context.Entry(subject).Property(s => s.ProgramId).IsModified = true;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("catalogs")]
        public async Task<ActionResult<object>> GetCatalogs()
        {
            var programs = await _context.AcademicProgram
                .AsNoTracking()
                .OrderBy(p => p.ProgramName)
                .Select(p => new ProgramItem(p.ProgramId, p.ProgramName))
                .ToListAsync();

            return Ok(new { programs });
        }
    }
}
