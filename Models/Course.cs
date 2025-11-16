using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace AcademicGradingSystem.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(120)]
        public string CourseName { get; set; }

        [ForeignKey("Subject")]
        public int SubjectId { get; set; }

        [ForeignKey("AcademicPeriod")]
        public int PeriodId { get; set; }

        [ForeignKey("User")]
        public int TeacherId { get; set; }

        // Navigation
        [ValidateNever]
        public Subject Subject { get; set; } = null!;

        [ValidateNever]
        public AcademicPeriod AcademicPeriod { get; set; } = null!;

        [ValidateNever]
        public User Teacher { get; set; } = null!;

        [ValidateNever]
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        [ValidateNever]
        public ICollection<EvaluationPlan> EvaluationPlans { get; set; } = new List<EvaluationPlan>();
    }
}
