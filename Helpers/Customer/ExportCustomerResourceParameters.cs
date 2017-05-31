using naturalgas.Helpers.Core;

namespace naturalgas.Helpers.Customer
{
    public class ExportCustomerResourceParameters : ExportResourceParameters
    {
        public string OrderBy { get; set; } = "DateOfBirth";
    }
}