using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ESPL.NG.Entities;
using ESPL.NG.Models.Core;


namespace ESPL.NG.Models
{
    public class CustomerDto : BaseDto
    {
        public Guid CustomerID { get; set; }

        public string NationalID { get; set; }
        
        public string CustomerName { get; set; }
        
        public string Mobile { get; set; }
        
        public string Landline { get; set; }
        
        public string CustomerEmail { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        public string CustomerAddress { get; set; }
        
        public bool Status { get; set; }
        
        public string  DistributorName { get; set; }
        
        public string DistributorAddress { get; set; }

        public string DistributorContact { get; set; }

    }
}