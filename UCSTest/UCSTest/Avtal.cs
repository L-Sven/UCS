using System;
using System.Collections.Generic;
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
        public string SlutDatum { get; set; }
        public string KommentarsFält { get; set; }
        public int IsActive { get; set; }
        public string PeriodStart { get; set; }
        public string PeriodEnd { get; set; }
        public double FakturaIntervall { get; set; }
        public double TotalKostnad { get; internal set; }
    }
}
