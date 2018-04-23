using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCSTest
{
    class Avtal
    {
        public List<AvtalsRad> ListAvtalsRad = new List<AvtalsRad>();
        public double DokumentNummer { get; set; }
        public string AvtalsDatum { get; set; }
        public string KundNummer { get; set; }
        public string StartDatum { get; set; }
        public string KommenteratSlutDatum { get; set; } = "1111-11-11";
        public int IsActive { get; set; }
        public string PeriodStart { get; set; }
        public string PeriodEnd { get; set; }
        public double FakturaIntervall { get; set; }
        public double TotalKostnad { get; internal set; }
        public int Uppsägningstid { get; internal set; }
        public int Förlängningstid { get; internal set; }
        public string AvtalsDatumSlut { get; internal set; }
    }
}
