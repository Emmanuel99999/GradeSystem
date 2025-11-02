using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ReportApiController(ApplicationDbContext context) => _context = context;

        public record ReportDto(
            int ReportId,
            int StudentId,
            string StudentName,
            int CourseId,
            string CourseName,
            string SubjectName,
            double FinalGrade,
            DateTime GeneratedAt
        );

        public record ReportDetailsDto(
            int ReportId,
            int StudentId,
            string StudentName,
            int CourseId,
            string CourseName,
            string SubjectName,
            double FinalGrade,
            DateTime GeneratedAt,
            IEnumerable<BreakdownItem> Breakdown
        );

        public record BreakdownItem(string ActivityName, double Weight, double? Score);
        public record ReportCreateDto(int StudentId, int CourseId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetAll()
        {
            var list = await _context.Reports
                .Include(r => r.Student)
                .Include(r => r.Course).ThenInclude(c => c.Subject)
                .OrderByDescending(r => r.GeneratedAt)
                .Select(r => new ReportDto(
                    r.ReportId,
                    r.StudentId,
                    r.Student.FirstName + " " + r.Student.LastName,
                    r.CourseId,
                    r.Course.CourseName,
                    r.Course.Subject.SubjectName,
                    r.FinalGrade,
                    r.GeneratedAt))
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReportDetailsDto>> Get(int id)
        {
            var report = await _context.Reports
                .Include(r => r.Student)
                .Include(r => r.Course).ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(r => r.ReportId == id);

            if (report == null) return NotFound();

            var breakdown = await _context.EvaluationPlans
                .Where(p => p.CourseId == report.CourseId)
                .Select(p => new BreakdownItem(
                    p.ActivityName,
                    p.Weight,
                    _context.Grades
                        .Where(g => g.PlanId == p.PlanId && g.StudentId == report.StudentId)
                        .OrderByDescending(g => g.DateRecorded)
                        .Select(g => (double?)g.Score)
                        .FirstOrDefault()))
                .ToListAsync();

            var dto = new ReportDetailsDto(
                report.ReportId,
                report.StudentId,
                report.Student.FirstName + " " + report.Student.LastName,
                report.CourseId,
                report.Course.CourseName,
                report.Course.Subject.SubjectName,
                report.FinalGrade,
                report.GeneratedAt,
                breakdown
            );

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ReportDto>> Create([FromBody] ReportCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var student = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == dto.StudentId && (u.Role.RoleName == "Estudiante" || u.Role.RoleName == "Student"));
            if (student == null) return BadRequest("StudentId no válido.");

            var course = await _context.Courses
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.CourseId == dto.CourseId);
            if (course == null) return BadRequest("CourseId no válido.");

            var plans = await _context.EvaluationPlans
                .Where(p => p.CourseId == dto.CourseId)
                .ToListAsync();

            if (!plans.Any()) return BadRequest("El curso no tiene plan de evaluación.");

            var totalWeight = plans.Sum(p => p.Weight);
            if (totalWeight <= 0) return BadRequest("Los pesos del plan de evaluación no son válidos.");

            double finalScore = 0.0;
            foreach (var plan in plans)
            {
                var score = await _context.Grades
                    .Where(g => g.PlanId == plan.PlanId && g.StudentId == dto.StudentId)
                    .OrderByDescending(g => g.DateRecorded)
                    .Select(g => (double?)g.Score)
                    .FirstOrDefaultAsync();

                if (score.HasValue)
                    finalScore += score.Value * (plan.Weight / totalWeight);
            }

            var report = new Report
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                GeneratedAt = DateTime.UtcNow,
                FinalGrade = Math.Round(finalScore, 2)
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            var response = new ReportDto(
                report.ReportId,
                report.StudentId,
                student.FirstName + " " + student.LastName,
                report.CourseId,
                course.CourseName,
                course.Subject.SubjectName,
                report.FinalGrade,
                report.GeneratedAt
            );

            return CreatedAtAction(nameof(Get), new { id = response.ReportId }, response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("catalogs")]
        public async Task<ActionResult<object>> GetCatalogs()
        {
            var students = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Estudiante" || u.Role.RoleName == "Student")
                .Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName })
                .OrderBy(u => u.FullName)
                .ToListAsync();

            var courses = await _context.Courses
                .Include(c => c.Subject)
                .Select(c => new { c.CourseId, c.CourseName, SubjectName = c.Subject.SubjectName })
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            return Ok(new { students, courses });
        }
    }
}
