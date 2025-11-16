using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace AcademicGradingSystem.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        [MaxLength(120)]
        public string SubjectName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        // Foreign key a AcademicProgram
        [Required]
        public int ProgramId { get; set; }

        [ForeignKey("ProgramId")]
        [ValidateNever]  
        public AcademicProgram Program { get; set; } = null!;


        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
