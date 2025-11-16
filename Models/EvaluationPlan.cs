using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicGradingSystem.Models
{
    public class EvaluationPlan
    {
        [Key]
        public int PlanId { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(120)]
        public string ActivityName { get; set; }

        [Required]
        [Range(0,100)]
        public double Weight { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        // Navigation
        [ValidateNever]
        public Course Course { get; set; }
        [ValidateNever]
        public ICollection<Grade> Grades { get; set; }
    }
}
