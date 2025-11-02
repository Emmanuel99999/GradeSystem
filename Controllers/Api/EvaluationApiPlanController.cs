using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationPlanApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EvaluationPlanApiController(ApplicationDbContext context) => _context = context;

        public record EvaluationPlanDto(
            int PlanId,
            string ActivityName,
            double Weight,
            DateTime DueDate,
            int CourseId,
            string CourseName,
            string SubjectName
        );

        public record EvaluationPlanCreateDto(string ActivityName, double Weight, DateTime DueDate, int CourseId);
        public record EvaluationPlanUpdateDto(int PlanId, string ActivityName, double Weight, DateTime DueDate, int CourseId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvaluationPlanDto>>> GetAll()
        {
            var list = await _context.EvaluationPlans
                .Include(p => p.Course)
                .ThenInclude(c => c.Subject)
                .Select(p => new EvaluationPlanDto(
                    p.PlanId,
                    p.ActivityName,
                    p.Weight,
                    p.DueDate,
                    p.CourseId,
                    p.Course.CourseName,
                    p.Course.Subject.SubjectName))
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EvaluationPlanDto>> Get(int id)
        {
            var p = await _context.EvaluationPlans
                .Include(x => x.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(x => x.PlanId == id);

            if (p == null) return NotFound();

            var dto = new EvaluationPlanDto(
                p.PlanId,
                p.ActivityName,
                p.Weight,
                p.DueDate,
                p.CourseId,
                p.Course.CourseName,
                p.Course.Subject.SubjectName);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<EvaluationPlanDto>> Create([FromBody] EvaluationPlanCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var exists = await _context.Courses.AnyAsync(c => c.CourseId == dto.CourseId);
            if (!exists) return BadRequest("CourseId no existe.");

            var plan = new EvaluationPlan
            {
                ActivityName = dto.ActivityName,
                Weight = dto.Weight,
                DueDate = dto.DueDate,
                CourseId = dto.CourseId
            };

            _context.EvaluationPlans.Add(plan);
            await _context.SaveChangesAsync();

            var created = await _context.EvaluationPlans
                .Include(p => p.Course)
                .ThenInclude(c => c.Subject)
                .FirstAsync(p => p.PlanId == plan.PlanId);

            var response = new EvaluationPlanDto(
                created.PlanId,
                created.ActivityName,
                created.Weight,
                created.DueDate,
                created.CourseId,
                created.Course.CourseName,
                created.Course.Subject.SubjectName);

            return CreatedAtAction(nameof(Get), new { id = response.PlanId }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EvaluationPlanUpdateDto dto)
        {
            if (id != dto.PlanId) return BadRequest("ID de URL y cuerpo no coinciden.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var exists = await _context.EvaluationPlans.AnyAsync(p => p.PlanId == id);
            if (!exists) return NotFound();

            if (!await _context.Courses.AnyAsync(c => c.CourseId == dto.CourseId))
                return BadRequest("CourseId no existe.");

            var plan = new EvaluationPlan
            {
                PlanId = dto.PlanId,
                ActivityName = dto.ActivityName,
                Weight = dto.Weight,
                DueDate = dto.DueDate,
                CourseId = dto.CourseId
            };

            _context.Entry(plan).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _context.EvaluationPlans.FindAsync(id);
            if (plan == null) return NotFound();

            _context.EvaluationPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("catalogs")]
        public async Task<ActionResult<object>> GetCatalogs()
        {
            var courses = await _context.Courses
                .Include(c => c.Subject)
                .Select(c => new
                {
                    c.CourseId,
                    CourseName = c.CourseName,
                    SubjectName = c.Subject.SubjectName
                })
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            return Ok(new { courses });
        }
    }
}
