using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ESPL.NG.Entities
{
    public class Employee : BaseEntity
    {
        [Key]
        public Guid EmployeeID { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        
        
        [MaxLength(50)]
        public string EmployeeCode { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }       
        
        [Required]
        [MaxLength(10)]
        public string Gender { get; set; }

        [Required]
        [MaxLength(20)]
        public string Mobile { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
        
        [MaxLength(20)]
        public string ResidencePhone { get; set; }
        
        public DateTime OrganizationJoiningDate { get; set; }
        
        public DateTime ServiceJoiningDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address1 { get; set; }

        [MaxLength(500)]
        public string Address2 { get; set; }

        [ForeignKey ("UserID")]
        public AppUser AppUser { get; set; }
        public string UserID { get; set; }
    }
}