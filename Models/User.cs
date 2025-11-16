using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AcademicGradingSystem.Models
{
    [Index(nameof(Email), IsUnique = true)] // índice único para login
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

      
        [Required, MaxLength(200)]
        [ValidateNever]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, ForeignKey(nameof(Role))]
        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegaciones: NO se validan en formularios
        [ValidateNever]
        public Role Role { get; set; } = null!;

        // Si es docente: cursos que dicta
        [ValidateNever]
        public ICollection<Course> CoursesTaught { get; set; } = new List<Course>();

        // Si es estudiante: sus matrículas
        [ValidateNever]
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        // Conveniencia para mostrar en vistas
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
