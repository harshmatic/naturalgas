using System;
using System.ComponentModel.DataAnnotations;
using ESPL.NG.Entities;
using ESPL.NG.Models.Core;

namespace ESPL.NG.Models
{
    public class CustomerForCreationDto : BaseDto
    {
        [RequiredAttribute(ErrorMessage="CustomerName is a required field.")]
        [MaxLengthAttribute(50,ErrorMessage="CustomerName cannot be greater than 50 characters.")]
        public string CustomerName { get; set; }

        [RequiredAttribute(ErrorMessage="Mobile Number is required field.")]
        [MaxLengthAttribute(20, ErrorMessage="Mobile number cannot be greater than 20 characters.")]
        public string Mobile { get; set; }
        
        [MaxLengthAttribute(20, ErrorMessage="Landline cannot be greater than 20 characters.")]
        public string Landline { get; set; }
        
        [RequiredAttribute (ErrorMessage="CustomerEmail is a required field.")]
        [MaxLengthAttribute(50,ErrorMessage="CustomerEmail cannot be greater than 50 characters.")]
        public string CustomerEmail { get; set; }
        
        [RequiredAttribute(ErrorMessage="DateOfBirth is a required field.")]
        public DateTime DateOfBirth { get; set; }
        
        [RequiredAttribute(ErrorMessage="CustomerAddress is a required field.")]
        [MaxLengthAttribute(500,ErrorMessage="CustomerAddress cannot be greater than 500 characters.")]
        public string CustomerAddress { get; set; }
        
        [RequiredAttribute(ErrorMessage="Status is a required field.")]
        public bool Status { get; set; }
        
        [RequiredAttribute(ErrorMessage="DistributorName is a required field.")]
        [MaxLengthAttribute(50,ErrorMessage="DistributorName cannot be greater than 50 characters.")]
        public string  DistributorName { get; set; }

        [MaxLengthAttribute(500,ErrorMessage="DistributorAddress cannot be greater than 500 characters.")]
        public string DistributorAddress { get; set; }

        [RequiredAttribute(ErrorMessage="DistributorContact is a required field.")]
        [MaxLengthAttribute(20,ErrorMessage="DistributorContact cannot be greater than 20 characters.")]
        public string DistributorContact { get; set; }
    }
}