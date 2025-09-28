using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AcademicGradingSystem.Models
{
    public class AcademicProgram
    {
        [Key]
        public int ProgramId { get; set; }

        [Required]
        [MaxLength(120)]
        public string ProgramName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // Inicializa la colección para evitar null reference y ayudar a EF Core
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}