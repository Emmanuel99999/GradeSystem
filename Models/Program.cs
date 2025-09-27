using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AcademicGradingSystem.Models
{
    public class Program
    {
        [Key]
        public int ProgramId { get; set; }

        [Required]
        [MaxLength(120)]
        public string ProgramName { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        // Navigation: one Program -> many Subjects
        public ICollection<Subject> Subjects { get; set; }
    }
}
