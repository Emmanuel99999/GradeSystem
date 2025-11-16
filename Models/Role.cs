using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AcademicGradingSystem.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        // Navigation
        [ValidateNever]
        public ICollection<User> Users { get; set; }
    }
}
