using System;

namespace UcsVismaTid
{
    internal class TimeReports
    {
        public int TimeReportId { get; set; }
        public int? ProjectId { get; set; }
        public decimal? HourToInvoice { get; set; }
        public decimal? HourOfReport { get; set; }
        public int? ProgramUserId { get; set; }
        public int? TimeCodeId { get; set; }
        public DateTime? DateOfReport { get; set; }
        public int? ResultUnitId { get; set; }
    }
}