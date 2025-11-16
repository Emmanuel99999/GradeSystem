using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicGradingSystem.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [ForeignKey("User")]
        public int StudentId { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Navigation
        [ValidateNever]
        public User Student { get; set; }
        [ValidateNever]
        public Course Course { get; set; }
    }
}
