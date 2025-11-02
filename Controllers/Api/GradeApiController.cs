using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradeApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public GradeApiController(ApplicationDbContext context) => _context = context;

        public record GradeDto(
            int GradeId,
            int StudentId,
            string StudentName,
            int PlanId,
            string ActivityName,
            double Score,
            DateTime DateRecorded
        );

        public record GradeCreateDto(int StudentId, int PlanId, double Score);
        public record GradeUpdateDto(int GradeId, int StudentId, int PlanId, double Score);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GradeDto>>> GetAll()
        {
            var list = await _context.Grades
                .Include(g => g.EvaluationPlan)
                .Include(g => g.Student)
                .Select(g => new GradeDto(
                    g.GradeId,
                    g.StudentId,
                    g.Student.FirstName + " " + g.Student.LastName,
                    g.PlanId,
                    g.EvaluationPlan.ActivityName,
                    g.Score,
                    g.DateRecorded))
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GradeDto>> Get(int id)
        {
            var g = await _context.Grades
                .Include(x => x.EvaluationPlan)
                .Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.GradeId == id);

            if (g == null) return NotFound();

            var dto = new GradeDto(
                g.GradeId,
                g.StudentId,
                g.Student.FirstName + " " + g.Student.LastName,
                g.PlanId,
                g.EvaluationPlan.ActivityName,
                g.Score,
                g.DateRecorded);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<GradeDto>> Create([FromBody] GradeCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var planExists = await _context.EvaluationPlans.AnyAsync(p => p.PlanId == dto.PlanId);
            var studentExists = await _context.Users.AnyAsync(u => u.UserId == dto.StudentId && u.Role.RoleName == "Student");
            if (!planExists || !studentExists) return BadRequest("Plan o estudiante no válido.");

            var grade = new Grade
            {
                StudentId = dto.StudentId,
                PlanId = dto.PlanId,
                Score = dto.Score,
                DateRecorded = DateTime.UtcNow
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            var created = await _context.Grades
                .Include(x => x.EvaluationPlan)
                .Include(x => x.Student)
                .FirstAsync(x => x.GradeId == grade.GradeId);

            var response = new GradeDto(
                created.GradeId,
                created.StudentId,
                created.Student.FirstName + " " + created.Student.LastName,
                created.PlanId,
                created.EvaluationPlan.ActivityName,
                created.Score,
                created.DateRecorded);

            return CreatedAtAction(nameof(Get), new { id = response.GradeId }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GradeUpdateDto dto)
        {
            if (id != dto.GradeId) return BadRequest("ID de URL y cuerpo no coinciden.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var exists = await _context.Grades.AnyAsync(g => g.GradeId == id);
            if (!exists) return NotFound();

            var grade = new Grade
            {
                GradeId = dto.GradeId,
                StudentId = dto.StudentId,
                PlanId = dto.PlanId,
                Score = dto.Score,
                DateRecorded = DateTime.UtcNow
            };

            _context.Entry(grade).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return NotFound();

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("catalogs")]
        public async Task<ActionResult<object>> GetCatalogs()
        {
            var plans = await _context.EvaluationPlans
                .Include(p => p.Course)
                .Select(p => new
                {
                    p.PlanId,
                    p.ActivityName,
                    CourseName = p.Course.CourseName
                })
                .OrderBy(p => p.ActivityName)
                .ToListAsync();

            var students = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Student")
                .Select(u => new
                {
                    u.UserId,
                    FullName = u.FirstName + " " + u.LastName
                })
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return Ok(new { plans, students });
        }
    }
}
