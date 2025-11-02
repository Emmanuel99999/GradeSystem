using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentApiController(ApplicationDbContext context) => _context = context;

        public record EnrollmentDto(
            int EnrollmentId,
            int StudentId,
            string StudentName,
            int CourseId,
            string CourseName,
            DateTime RegistrationDate
        );

        public record EnrollmentCreateDto(int StudentId, int CourseId);
        public record EnrollmentUpdateDto(int EnrollmentId, int StudentId, int CourseId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetAll()
        {
            var list = await _context.Enrollments
                .Include(e => e.Student)
                .ThenInclude(s => s.Role)
                .Include(e => e.Course)
                .ThenInclude(c => c.Subject)
                .OrderByDescending(e => e.RegistrationDate)
                .Select(e => new EnrollmentDto(
                    e.EnrollmentId,
                    e.StudentId,
                    e.Student.FirstName + " " + e.Student.LastName,
                    e.CourseId,
                    e.Course.CourseName,
                    e.RegistrationDate))
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EnrollmentDto>> Get(int id)
        {
            var e = await _context.Enrollments
                .Include(x => x.Student)
                .Include(x => x.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(x => x.EnrollmentId == id);

            if (e == null) return NotFound();

            var dto = new EnrollmentDto(
                e.EnrollmentId,
                e.StudentId,
                e.Student.FirstName + " " + e.Student.LastName,
                e.CourseId,
                e.Course.CourseName,
                e.RegistrationDate);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> Create([FromBody] EnrollmentCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var exists = await _context.Enrollments.AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId);
            if (exists) return Conflict("El estudiante ya está inscrito en este curso.");

            var enrollment = new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                RegistrationDate = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var created = await _context.Enrollments
                .Include(x => x.Student)
                .Include(x => x.Course)
                .FirstAsync(x => x.EnrollmentId == enrollment.EnrollmentId);

            var response = new EnrollmentDto(
                created.EnrollmentId,
                created.StudentId,
                created.Student.FirstName + " " + created.Student.LastName,
                created.CourseId,
                created.Course.CourseName,
                created.RegistrationDate);

            return CreatedAtAction(nameof(Get), new { id = response.EnrollmentId }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EnrollmentUpdateDto dto)
        {
            if (id != dto.EnrollmentId) return BadRequest("ID de URL y cuerpo no coinciden.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var exists = await _context.Enrollments.AnyAsync(e => e.EnrollmentId == id);
            if (!exists) return NotFound();

            var enrollment = new Enrollment
            {
                EnrollmentId = dto.EnrollmentId,
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                RegistrationDate = DateTime.UtcNow
            };

            _context.Entry(enrollment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return NotFound();

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
