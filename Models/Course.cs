using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public Subject Subject { get; set; }
        public AcademicPeriod AcademicPeriod { get; set; }
        public User Teacher { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<EvaluationPlan> EvaluationPlans { get; set; }
    }
}
