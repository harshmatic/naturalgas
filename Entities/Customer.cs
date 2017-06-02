using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ESPL.NG.Entities
{
    public class Customer : BaseEntity
    {
        [Key]
        public Guid CustomerID { get; set; }
        
        [RequiredAttribute]
        [MaxLengthAttribute(50)]
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