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
        public string SerialNumber { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Othername { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Citizenship { get; set; }
        public string Occupation { get; set; }
        public string Pin { get; set; }
        public string Address { get; set; }
        public bool Status { get; set; }
        public string Gender { get; set; }
        public string  DistributorName { get; set; }
        public string DistributorAddress { get; set; }
        public string DistributorContact { get; set; }

    }
}