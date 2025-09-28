using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicGradingSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Role Role { get; set; }

        // If the user is a teacher → Courses they teach
        public ICollection<Course> CoursesTaught { get; set; }

        // If the user is a student → Enrollments they belong to
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
