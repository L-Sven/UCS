namespace UCSTest
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    class FakturaHuvud
    {

        public List<FakturaRad> fakturaRader = new List<FakturaRad>();
        public double FakturaNummer { get; set; }
        public string FakturaTyp { get; set; } = "";
        public string KundNummer { get; set; } = "";
        public string Säljare { get; set; } = "";
        public string KundNamn { get; set; } = "";
        public string KundStad { get; set; } = "";
        public string KundLand { get; set; } = "";
        public string FakturaDatum { get; set; } = "";
        public double TotalKostnad { get; set; }
        public string FörfalloDatum { get; set; } = "";
        public string SlutDatum { get; set; } = "";


    }
}
