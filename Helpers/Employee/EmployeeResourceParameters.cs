using System.Collections.Generic;
using ESPL.NG.Helpers.Core;

namespace ESPL.NG.Helpers.Employee
{
    public class EmployeesResourceParameters : BaseResourceParameters
    {
        public string OrderBy { get; set; } = "FirstName";
        public string DesignationID { get; set; }
        public string AreaID { get; set; }
        public string DepartmentID { get; set; }
        public bool? CaseAssigned { get; set; }=null;
    }
}