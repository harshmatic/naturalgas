using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ESPL.NG.Entities
{
    //public class AppUser : IdentityUser {
    public class AppUser : BaseEntity
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime LastLogin { get; set; }

        [Required]
        public int FailedPasswordAttemptCount { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string NormalizedUserName { get; set; }

    }
}