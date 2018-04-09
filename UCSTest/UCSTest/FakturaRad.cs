using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCSTest
{
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
