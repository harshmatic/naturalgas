using System;
using System.ComponentModel.DataAnnotations;

namespace ESPL.NG.Entities.Core
{
    public class AppModule : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string MenuText { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(2), MinLength(2)]
        public string ShortName { get; set; }
    }
}