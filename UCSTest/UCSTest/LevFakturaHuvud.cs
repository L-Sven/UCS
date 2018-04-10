using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCSTest
{
    class LevFakturaHuvud
    {
        public List<LevFakturaRad> fakturaRader = new List<LevFakturaRad>();
        public double LopNummer { get; set; }
        public string LevNummer { get; set; } = "";
        public string LevNamn { get; set; } = "";
        public string FakturaNummer { get; set; } = "";
        public string FakturaDatum { get; set; } = "";
        public string ValutaKod { get; set; } = "";
        public decimal ValutaKurs { get; set; }
        public string FakturaTyp { get; set; } = "";
        public double TotalKostnad { get; set; }
        public string ProjektHuvud { get; set; } = "";
    }
}
