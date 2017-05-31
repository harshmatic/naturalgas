using ESPL.NG.Helpers.Core;

namespace naturalgas.Helpers.Core
{
    public class ExportResourceParameters : BaseResourceParameters
    {
        public new int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value < 0) ? 0 : value;
            }
        }
    }
}