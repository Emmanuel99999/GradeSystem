using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace AcademicGradingSystem.Models
{
    public class AcademicPeriod
    {
        [Key]
        public int PeriodId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ValidateNever]  
        public ICollection<Course> Courses { get; set; } = new List<Course>();

    }
}
