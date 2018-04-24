using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adk = AdkNetWrapper;

namespace UCSTest
{
    class VismaTidData
    {
        private string _ftg;
        private string _sys;
        int pData;
        private Adk.Api.ADKERROR error;

        public VismaTidData(string ftg, string sys)
        {
            _ftg = ftg;
            _sys = sys;
        }

    }
}
