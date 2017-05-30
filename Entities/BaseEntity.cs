using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESPL.NG.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            this.CreatedOn = DateTime.Now;
            this.IsDelete = false;
            this.IsAssigned = false;
        }
        public DateTime CreatedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDelete { get; set; }
        public bool IsAssigned { get; set; }
        [NotMapped]
        public string CreatedByName { get; set; }
        [NotMapped]
        public string UpdatedByName { get; set; }
    }
}