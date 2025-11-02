using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProgramApiController(ApplicationDbContext context) => _context = context;

        public record ProgramDto(int ProgramId, string ProgramName, string? Description, int SubjectsCount);
        public record ProgramDetailsDto(int ProgramId, string ProgramName, string? Description, IEnumerable<SubjectItem> Subjects);
        public record SubjectItem(int SubjectId, string SubjectName, string Code);
        public record ProgramCreateDto(string ProgramName, string? Description);
        public record ProgramUpdateDto(int ProgramId, string ProgramName, string? Description);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgramDto>>> GetAll([FromQuery] string? search = null)
        {
            var q = _context.AcademicProgram.AsNoTracking().Include(p => p.Subjects).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(p => p.ProgramName.Contains(search));
            var list = await q
                .OrderBy(p => p.ProgramName)
                .Select(p => new ProgramDto(p.ProgramId, p.ProgramName, p.Description, p.Subjects.Count))
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProgramDetailsDto>> Get(int id)
        {
            var p = await _context.AcademicProgram
                .AsNoTracking()
                .Include(x => x.Subjects)
                .FirstOrDefaultAsync(x => x.ProgramId == id);

            if (p == null) return NotFound();

            var dto = new ProgramDetailsDto(
                p.ProgramId,
                p.ProgramName,
                p.Description,
                p.Subjects.Select(s => new SubjectItem(s.SubjectId, s.SubjectName, s.Code)));

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ProgramDto>> Create([FromBody] ProgramCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var program = new AcademicProgram
            {
                ProgramName = dto.ProgramName,
                Description = dto.Description ?? string.Empty
            };

            _context.AcademicProgram.Add(program);
            await _context.SaveChangesAsync();

            var created = new ProgramDto(program.ProgramId, program.ProgramName, program.Description, 0);
            return CreatedAtAction(nameof(Get), new { id = created.ProgramId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProgramUpdateDto dto)
        {
            if (id != dto.ProgramId) return BadRequest("ID de URL y cuerpo no coinciden.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var exists = await _context.AcademicProgram.AnyAsync(p => p.ProgramId == id);
            if (!exists) return NotFound();

            var program = new AcademicProgram
            {
                ProgramId = dto.ProgramId,
                ProgramName = dto.ProgramName,
                Description = dto.Description ?? string.Empty
            };

            _context.Entry(program).Property(p => p.ProgramName).IsModified = true;
            _context.Entry(program).Property(p => p.Description).IsModified = true;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var program = await _context.AcademicProgram.FindAsync(id);
            if (program == null) return NotFound();

            _context.AcademicProgram.Remove(program);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id:int}/subjects")]
        public async Task<ActionResult<IEnumerable<SubjectItem>>> GetSubjects(int id)
        {
            var exists = await _context.AcademicProgram.AnyAsync(p => p.ProgramId == id);
            if (!exists) return NotFound();

            var items = await _context.Subjects
                .AsNoTracking()
                .Where(s => s.ProgramId == id)
                .OrderBy(s => s.SubjectName)
                .Select(s => new SubjectItem(s.SubjectId, s.SubjectName, s.Code))
                .ToListAsync();

            return Ok(items);
        }
    }
}
