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

        [RequiredAttribute(ErrorMessage="Please Enter Name.")]
        [MaxLengthAttribute(50,ErrorMessage="Name cannot be greater than 50 characters.")]
        public string CustomerName { get; set; }

        [RequiredAttribute(ErrorMessage="Please enter mobile number.")]
        [MaxLengthAttribute(20, ErrorMessage="Mobile number cannot be greater than 20 characters.")]
        public string Mobile { get; set; }
        
        [MaxLengthAttribute(20, ErrorMessage="Landline cannot be greater than 20 characters.")]
        public string Landline { get; set; }
        
        [RequiredAttribute (ErrorMessage="Pelase enter email address")]
        [MaxLengthAttribute(50,ErrorMessage="Email cannot be greater than 50 characters.")]
        public string CustomerEmail { get; set; }
        
        [RequiredAttribute(ErrorMessage="Please enter date of birth")]
        public DateTime DateOfBirth { get; set; }
        
        [RequiredAttribute(ErrorMessage="Please enter current address")]
        [MaxLengthAttribute(500,ErrorMessage="Address cannot be greater than 500 characters.")]
        public string CustomerAddress { get; set; }
        
        [RequiredAttribute(ErrorMessage="Pelase check your status")]
        public bool Status { get; set; }
        
        [RequiredAttribute(ErrorMessage="Please enter distributor name.")]
        [MaxLengthAttribute(50,ErrorMessage="Distributor Name cannot be greater than 50 characters.")]
        public string  DistributorName { get; set; }

        [MaxLengthAttribute(500,ErrorMessage="Distributor Address cannot be greater than 500 characters.")]
        public string DistributorAddress { get; set; }

        [RequiredAttribute(ErrorMessage="Please enter distributor contact details")]
        [MaxLengthAttribute(20,ErrorMessage="DistributorContact cannot be greater than 20 characters.")]
        public string DistributorContact { get; set; }
    }
}