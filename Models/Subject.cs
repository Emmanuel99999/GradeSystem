using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicGradingSystem.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        [MaxLength(120)]
        public string SubjectName { get; set; }

        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        public int Credits { get; set; }

        [ForeignKey("Program")]
        public int ProgramId { get; set; }

        // Navigation
        public Program Program { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
