namespace UCSTest
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    class FakturaRad
    {
        public string ArtikelNummer { get; set; } = "";
        public string Benämning { get; set; } = "";
        public double LevAntal { get; set; }
        public string EnhetsTyp { get; set; } = "";
        public double StyckPris { get; set; }
        public double TotalKostnad { get; set; }
    }
}
