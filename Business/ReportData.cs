using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTI.Modules.Shared.Business
{
    public class ReportData
    {
        public int ReportId { get; set; }
        public bool IsActive { get; set; }
        public string ReportDisplayName { get; set; }
        public string ReportFileName { get; set; }
        public int ReportTypeId { get; set; }
        public string ReportType { get; set; }
    }
}
