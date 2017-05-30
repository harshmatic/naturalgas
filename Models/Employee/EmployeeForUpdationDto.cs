using System;
using ESPL.NG.Models.Core;

namespace ESPL.NG.Models
{
    public class EmployeeForUpdationDto : BaseDto
    {
        public EmployeeForUpdationDto()
        {
            this.CreatedOn = null;
            this.UpdatedOn = DateTime.Now;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmployeeCode { get; set; }

        public DateTime DateofBirth { get; set; }

        public string Gender { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string ResidencePhone1 { get; set; }

        public DateTime OrganizationJoiningDate { get; set; }

        public DateTime ServiceJoiningDate { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public Guid AreaID { get; set; }

        public Guid DepartmentID { get; set; }

        public Guid DesignationID { get; set; }

        public Guid ShiftID { get; set; }

        public string UserID { get; set; }
    }
}