﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcsGui
{
    class LevFakturaRad
    {

        public double Kvantitet { get; set; }
        public string Information { get; set; } = "";
        public double TotalKostnad { get; set; }
        public string ArtikelNummer { get; set; } = "";
        public string LevArtikelNummer { get; set; } = "";
        public string ProjektRad { get; set; } = "";
        public int LevRadID { get; set; }
        public string ResultatEnhet { get; internal set; }
    }
}
