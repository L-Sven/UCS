using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcsAdm
{
    public class Artikel
    {
        public string ArtikelNummer { get; set; }
        public string Benämning { get; set; }
        public string ArtikelGrupp { get; set; }
        public string EnhetsKod { get; set; }
        public double InköpsPris { get; set; }
        public double Frakt { get; set; }
        public double OvrigKostnad { get; set; }

    }
}
