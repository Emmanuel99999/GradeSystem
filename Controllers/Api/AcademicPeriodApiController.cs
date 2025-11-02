using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Data;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")] // → /api/academicperiod
    public class AcademicPeriodApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AcademicPeriodApiController(ApplicationDbContext context) => _context = context;

        // GET: api/AcademicPeriod?includeCourses=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicPeriod>>> GetAll([FromQuery] bool includeCourses = false)
        {
            IQueryable<AcademicPeriod> q = _context.AcademicPeriods;
            if (includeCourses) q = q.Include(p => p.Courses);
            var list = await q.OrderByDescending(p => p.StartDate).ToListAsync();
            return Ok(list);
        }

        // GET: api/AcademicPeriod/5?includeCourses=true
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AcademicPeriod>> Get(int id, [FromQuery] bool includeCourses = false)
        {
            IQueryable<AcademicPeriod> q = _context.AcademicPeriods;
            if (includeCourses) q = q.Include(p => p.Courses);

            var period = await q.FirstOrDefaultAsync(p => p.PeriodId == id);
            if (period == null) return NotFound();
            return Ok(period);
        }

        // POST: api/AcademicPeriod
        [HttpPost]
        public async Task<ActionResult<AcademicPeriod>> Create([FromBody] AcademicPeriod period)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // (Opcional) Reglas simples
            if (period.EndDate < period.StartDate)
                return BadRequest("EndDate no puede ser menor que StartDate.");

            _context.AcademicPeriods.Add(period);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = period.PeriodId }, period);
        }

        // PUT: api/AcademicPeriod/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AcademicPeriod period)
        {
            if (id != period.PeriodId) return BadRequest("El ID de la URL no coincide con el del cuerpo.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (period.EndDate < period.StartDate)
                return BadRequest("EndDate no puede ser menor que StartDate.");

            // Adjuntar y marcar como modificado (o mapear campos si prefieres)
            _context.Entry(period).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.AcademicPeriods.AnyAsync(p => p.PeriodId == id);
                if (!exists) return NotFound();
                throw;
            }

            return NoContent();
            // (Alternativa) return Ok(period);
        }

        // DELETE: api/AcademicPeriod/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var period = await _context.AcademicPeriods.FindAsync(id);
            if (period == null) return NotFound();

            // Si no quieres borrar cascada cursos, valida aquí si existen
            _context.AcademicPeriods.Remove(period);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
