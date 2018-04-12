using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCSTest
{
    class SkickaData
    {
        private KundFakturaHuvud kFaktura;
        private LevFakturaHuvud lFaktura;
        SqlConnection sqlCon = new SqlConnection(
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\users\sijoh0500\Work Folders\Documents\Github\UCSTest\UCSTest\fakturaDB.mdf;Integrated Security=True");

        public SkickaData (KundFakturaHuvud k)
        {
            kFaktura = new KundFakturaHuvud();
            kFaktura = k;
            KundFakturaTillDatabas();
        }

        public SkickaData (LevFakturaHuvud l)
        {
            lFaktura = new LevFakturaHuvud();
            lFaktura = l;
            LevFakturaTillDatabas();
        }

        private void KundFakturaTillDatabas()
        {

            SqlCommand cmdAddInvoice = new SqlCommand("sp_add_customerInvoice", sqlCon);

            cmdAddInvoice.CommandType = CommandType.StoredProcedure;

            SqlParameter returnParam = cmdAddInvoice.Parameters.Add("@ReturnValue", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;

            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaNummer", (int)kFaktura.FakturaNummer));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaTyp", kFaktura.FakturaTyp));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@kundNummer", int.Parse(kFaktura.KundNummer)));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@säljare", kFaktura.Säljare));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@kundNamn", kFaktura.KundNamn));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@kundStad", kFaktura.KundStad));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@kundLand", kFaktura.KundLand));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaDatum", kFaktura.FakturaDatum));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@totalKostnad", decimal.Parse(kFaktura.TotalKostnad.ToString())));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@förfalloDatum", ""));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@slutDatum", ""));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@fraktAvgift", kFaktura.Cargo_amount));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@administrationsAvgift", kFaktura.Dispatch_fee));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@moms", kFaktura.Moms));

            sqlCon.Open();
            cmdAddInvoice.ExecuteNonQuery();
            var returnFromSp = returnParam.Value;
            sqlCon.Close();

            foreach (var fRad in kFaktura.fakturaRader)
            {
                SqlCommand cmdAddRow = new SqlCommand("sp_add_customerInvoiceRow", sqlCon);
                cmdAddRow.CommandType = CommandType.StoredProcedure;

                cmdAddRow.Parameters.Add(new SqlParameter("@radID", fRad.KundRadID));
                cmdAddRow.Parameters.Add(new SqlParameter("@artikelNummer", fRad.ArtikelNummer));
                cmdAddRow.Parameters.Add(new SqlParameter("@benämning", fRad.Benämning));
                cmdAddRow.Parameters.Add(new SqlParameter("@levAntal", fRad.LevAntal.ToString()));
                cmdAddRow.Parameters.Add(new SqlParameter("@enhetsTyp", fRad.EnhetsTyp));
                cmdAddRow.Parameters.Add(new SqlParameter("@totalKostnad", fRad.TotalKostnad));
                cmdAddRow.Parameters.Add(new SqlParameter("@fakturaNummer", (int)kFaktura.FakturaNummer));
                cmdAddRow.Parameters.Add(new SqlParameter("@projekt", fRad.Projekt));

                sqlCon.Open();
                cmdAddRow.ExecuteNonQuery();
                sqlCon.Close();
            }

        }

        private void LevFakturaTillDatabas()
        {
            SqlCommand cmdAddInvoice = new SqlCommand("sp_add_levInvoice", sqlCon);

            cmdAddInvoice.CommandType = CommandType.StoredProcedure;

            var returnParam = cmdAddInvoice.Parameters.Add("@ReturnValue", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;

            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaNummer", lFaktura.FakturaNummer));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaTyp", lFaktura.FakturaTyp));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@levNummer", lFaktura.LevNummer));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@lopNummer", (int)lFaktura.LopNummer));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@levNamn", lFaktura.LevNamn));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaDatum", lFaktura.FakturaDatum));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@totalKostnad", decimal.Parse(lFaktura.TotalKostnad.ToString())));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@projektHuvud", lFaktura.ProjektHuvud));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@moms", lFaktura.Moms));

            sqlCon.Open();
            cmdAddInvoice.ExecuteNonQuery();
            var returnFromSp = returnParam.Value;
            sqlCon.Close();

            foreach (var lRad in lFaktura.fakturaRader)
            {
                SqlCommand cmdAddRow = new SqlCommand("sp_add_levInvoiceRow", sqlCon);
                cmdAddRow.CommandType = CommandType.StoredProcedure;

                cmdAddRow.Parameters.Add(new SqlParameter("@radID", lRad.LevRadID));
                cmdAddRow.Parameters.Add(new SqlParameter("@artikelNummer", lRad.ArtikelNummer));
                cmdAddRow.Parameters.Add(new SqlParameter("@information", lRad.Information));
                cmdAddRow.Parameters.Add(new SqlParameter("@kvantitet", decimal.Parse(lRad.Kvantitet.ToString())));
                cmdAddRow.Parameters.Add(new SqlParameter("@levArtikelNummer", lRad.LevArtikelNummer));
                cmdAddRow.Parameters.Add(new SqlParameter("@totalKostnad", decimal.Parse(lRad.TotalKostnad.ToString())));
                cmdAddRow.Parameters.Add(new SqlParameter("@fakturaNummer", lFaktura.FakturaNummer));
                cmdAddRow.Parameters.Add(new SqlParameter("@projektRad", lRad.ProjektRad));
                

                sqlCon.Open();
                cmdAddRow.ExecuteNonQuery();
                sqlCon.Close();
            }
        }
        
    }
}
