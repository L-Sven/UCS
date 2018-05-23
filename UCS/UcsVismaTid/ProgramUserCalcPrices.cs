using System;

namespace UcsVismaTid
{
    internal class ProgramUserCalcPrices
    {
        public int ProgramUserCalcPriceId { get; set; }
        public int ProgramUserId { get; set; }
        public decimal? CalcPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}