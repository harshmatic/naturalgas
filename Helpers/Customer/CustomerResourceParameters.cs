using System.Collections.Generic;
using ESPL.NG.Helpers.Core;

namespace ESPL.NG.Helpers.Customer
{
    public class CustomerResourceParameters : BaseResourceParameters
    {
        public string OrderBy { get; set; } = "DateOfBirth";
        
    }
}