using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcsAdm
{
    class KundFakturaRad
    {

        public string ArtikelNummer { get; set; } = "";
        public double LevAntal { get; set; }
        public string ResultatEnhet { get; internal set; }
        public double StyckPris { get; set; }
        public double TotalKostnad { get; set; }
        public string Projekt { get; set; } = "";
        public int KundRadID { get; set; }
        public double TäckningsGrad { get; set; }
        public double TäckningsBidrag { get; set; }
        public string Benämning { get; set; }


    }
}
