using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicGradingSystem.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }

        [ForeignKey("User")]
        public int StudentId { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required]
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // Final course grade for the student (0-100)
        [Range(0, 100)]
        public double FinalGrade { get; set; }

        // Navigation
        [ValidateNever]
        public User Student { get; set; }
        [ValidateNever]
        public Course Course { get; set; }
    }
}
