using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")] // → /api/course
    public class CourseApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CourseApiController(ApplicationDbContext context) => _context = context;

        // DTOs para requests y responses
        public record CourseCreateDto(string CourseName, int SubjectId, int PeriodId, int TeacherId);
        public record CourseUpdateDto(int CourseId, string CourseName, int SubjectId, int PeriodId, int TeacherId);
        public record CourseDto(
            int CourseId, string CourseName,
            int SubjectId, string SubjectName,
            int PeriodId, string PeriodName,
            int TeacherId, string TeacherFullName
        );

        // GET: api/course?search=prog&teacherId=1&subjectId=2&periodId=3&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll(
            [FromQuery] string? search = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] int? periodId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;

            var q = _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.AcademicPeriod)
                .Include(c => c.Teacher)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(c => c.CourseName.Contains(search));

            if (teacherId.HasValue) q = q.Where(c => c.TeacherId == teacherId.Value);
            if (subjectId.HasValue) q = q.Where(c => c.SubjectId == subjectId.Value);
            if (periodId.HasValue) q = q.Where(c => c.PeriodId == periodId.Value);

            var items = await q
                .OrderBy(c => c.CourseName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseDto(
                    c.CourseId, c.CourseName,
                    c.SubjectId, c.Subject.SubjectName,
                    c.PeriodId, c.AcademicPeriod.Name,
                    c.TeacherId, (c.Teacher.FirstName + " " + c.Teacher.LastName)))
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/course/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CourseDto>> Get(int id)
        {
            var c = await _context.Courses
                .Include(x => x.Subject)
                .Include(x => x.AcademicPeriod)
                .Include(x => x.Teacher)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CourseId == id);

            if (c == null) return NotFound();

            var dto = new CourseDto(
                c.CourseId, c.CourseName,
                c.SubjectId, c.Subject.SubjectName,
                c.PeriodId, c.AcademicPeriod.Name,
                c.TeacherId, (c.Teacher.FirstName + " " + c.Teacher.LastName));

            return Ok(dto);
        }

        // POST: api/course
        [HttpPost]
        public async Task<ActionResult<CourseDto>> Create([FromBody] CourseCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Validaciones FK
            var subjectExists = await _context.Subjects.AnyAsync(s => s.SubjectId == dto.SubjectId);
            if (!subjectExists) return BadRequest("SubjectId no existe.");

            var periodExists = await _context.AcademicPeriods.AnyAsync(p => p.PeriodId == dto.PeriodId);
            if (!periodExists) return BadRequest("PeriodId no existe.");

            var teacherExists = await _context.Users.AnyAsync(u => u.UserId == dto.TeacherId);
            if (!teacherExists) return BadRequest("TeacherId no existe.");

            var course = new Course
            {
                CourseName = dto.CourseName,
                SubjectId = dto.SubjectId,
                PeriodId = dto.PeriodId,
                TeacherId = dto.TeacherId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // proyectar respuesta
            var created = await _context.Courses
                .Include(x => x.Subject)
                .Include(x => x.AcademicPeriod)
                .Include(x => x.Teacher)
                .AsNoTracking()
                .FirstAsync(x => x.CourseId == course.CourseId);

            var response = new CourseDto(
                created.CourseId, created.CourseName,
                created.SubjectId, created.Subject.SubjectName,
                created.PeriodId, created.AcademicPeriod.Name,
                created.TeacherId, (created.Teacher.FirstName + " " + created.Teacher.LastName));

            return CreatedAtAction(nameof(Get), new { id = response.CourseId }, response);
        }

        // PUT: api/course/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CourseUpdateDto dto)
        {
            if (id != dto.CourseId) return BadRequest("ID de URL y cuerpo no coinciden.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var exists = await _context.Courses.AnyAsync(c => c.CourseId == id);
            if (!exists) return NotFound();

            // Validaciones FK
            if (!await _context.Subjects.AnyAsync(s => s.SubjectId == dto.SubjectId))
                return BadRequest("SubjectId no existe.");
            if (!await _context.AcademicPeriods.AnyAsync(p => p.PeriodId == dto.PeriodId))
                return BadRequest("PeriodId no existe.");
            if (!await _context.Users.AnyAsync(u => u.UserId == dto.TeacherId))
                return BadRequest("TeacherId no existe.");

            var course = new Course
            {
                CourseId = dto.CourseId,
                CourseName = dto.CourseName,
                SubjectId = dto.SubjectId,
                PeriodId = dto.PeriodId,
                TeacherId = dto.TeacherId
            };

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Courses.AnyAsync(e => e.CourseId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/course/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Helpers opcionales para catálogos (subjects, periods, teachers)
        // GET: api/course/catalogs
        [HttpGet("catalogs")]
        public async Task<ActionResult<object>> GetCatalogs()
        {
            var subjects = await _context.Subjects
                .AsNoTracking()
                .Select(s => new { s.SubjectId, s.SubjectName })
                .OrderBy(s => s.SubjectName)
                .ToListAsync();

            var periods = await _context.AcademicPeriods
                .AsNoTracking()
                .Select(p => new { p.PeriodId, p.Name })
                .OrderByDescending(p => p.Name)
                .ToListAsync();

            // Soporta "Docente" (seed en español) y "Teacher" (si lo usaste antes)
            var teachers = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Docente" || u.Role.RoleName == "Teacher")
                .AsNoTracking()
                .Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName })
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return Ok(new { subjects, periods, teachers });
        }
    }
}
