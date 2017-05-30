using System;
using System.ComponentModel.DataAnnotations;
using ESPL.NG.Models.Core;

namespace ESPL.NG.Models
{
    public class CustomerForUpdationDto : BaseDto
    {
        public CustomerForUpdationDto()
        {
            this.CreatedOn = null;
            this.UpdatedOn = DateTime.Now;
        }

         public string CustomerName { get; set; }
        [RequiredAttribute]
        [MaxLengthAttribute(20)]
        public string Mobile { get; set; }
        
        [MaxLengthAttribute(20)]
        public string Landline { get; set; }
        
        [RequiredAttribute]
        [MaxLengthAttribute(50)]
        public string CustomerEmail { get; set; }
        
        [RequiredAttribute]
        public DateTime DateOfBirth { get; set; }
        
        [RequiredAttribute]
        [MaxLengthAttribute(500)]
        public string CustomerAddress { get; set; }
        
        [RequiredAttribute]
        public bool Status { get; set; }
        
        [RequiredAttribute]
        [MaxLengthAttribute(50)]
        public string  DistributorName { get; set; }
        [MaxLengthAttribute(500)]
        public string DistributorAddress { get; set; }

        [RequiredAttribute]
        [MaxLengthAttribute(20)]
        public string DistributorContact { get; set; }
    }
}