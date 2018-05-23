using System;

namespace UcsVismaTid
{
    internal class PriceListPeriods
    {
        public PriceListPeriods()
        {
        }

        public int PriceListId { get; set; }
        public bool PriceForCustomer { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int PriceListPeriodId { get; set; }
    }
}