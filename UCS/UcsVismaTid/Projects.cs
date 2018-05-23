using System;

namespace UcsVismaTid
{
    internal class Projects
    {
        public int? ProjectCategoryId { get; set; }
        public int ProjectId { get; set; }
        public decimal? NoOfHours { get; set; }
        public int CustomerId { get; set; }
        public int ProgramUserId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? StdPriceListId { get; set; }
    }
}