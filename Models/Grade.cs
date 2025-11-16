using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicGradingSystem.Models
{
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }

        [ForeignKey("EvaluationPlan")]
        public int PlanId { get; set; }

        [ForeignKey("User")]
        public int StudentId { get; set; }

        [Required]
        [Range(0,100)]
        public double Score { get; set; }

        [Required]
        public DateTime DateRecorded { get; set; } = DateTime.UtcNow;

        // Navigation
        [ValidateNever]
        public EvaluationPlan EvaluationPlan { get; set; }
        [ValidateNever]
        public User Student { get; set; }
    }
}
