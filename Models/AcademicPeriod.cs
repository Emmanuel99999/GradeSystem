using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        // Navigation: one period has many courses
        public ICollection<Course> Courses { get; set; }
    }
}
