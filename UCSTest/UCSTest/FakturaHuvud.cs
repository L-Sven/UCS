using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCSTest
{
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
