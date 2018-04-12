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
        
        SqlConnection sqlCon = new SqlConnection(
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\users\sijoh0500\Work Folders\Documents\Github\UCSTest\UCSTest\fakturaDB.mdf;Integrated Security=True");

        public SkickaData (KundFakturaHuvud k)
        {
            KundFakturaTillDatabas(k);
        }

        public SkickaData (LevFakturaHuvud l)
        {
            LevFakturaTillDatabas(l);
        }

        public SkickaData(Artikel a)
        {
            ArtikelTillDatabas(a);
        }

        private void ArtikelTillDatabas(Artikel a)
        {
            SqlCommand cmdAddArticle = new SqlCommand("sp_add_article", sqlCon);

            cmdAddArticle.CommandType = CommandType.StoredProcedure;

            SqlParameter returnParam = cmdAddArticle.Parameters.Add("@ReturnValue", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;

            cmdAddArticle.Parameters.Add(new SqlParameter("@artikelNummer", a.ArtikelNummer));
            cmdAddArticle.Parameters.Add(new SqlParameter("@artikelGrupp", a.ArtikelGrupp));
            cmdAddArticle.Parameters.Add(new SqlParameter("@benämning", a.Benämning));
            cmdAddArticle.Parameters.Add(new SqlParameter("@enhetsKod", a.EnhetsKod));

            sqlCon.Open();
            cmdAddArticle.ExecuteNonQuery();
            var returnFromSp = returnParam.Value;
            sqlCon.Close();
        }

        private void KundFakturaTillDatabas(KundFakturaHuvud kFaktura)
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
                cmdAddRow.Parameters.Add(new SqlParameter("@levAntal", fRad.LevAntal.ToString()));
                cmdAddRow.Parameters.Add(new SqlParameter("@totalKostnad", fRad.TotalKostnad));
                cmdAddRow.Parameters.Add(new SqlParameter("@fakturaNummer", (int)kFaktura.FakturaNummer));
                cmdAddRow.Parameters.Add(new SqlParameter("@projekt", fRad.Projekt));
                cmdAddRow.Parameters.Add(new SqlParameter("@täckningsGrad", decimal.Parse(fRad.TäckningsGrad.ToString())));
                cmdAddRow.Parameters.Add(new SqlParameter("@benämning", fRad.Benämning));

                sqlCon.Open();
                cmdAddRow.ExecuteNonQuery();
                sqlCon.Close();
            }

        }

        private void LevFakturaTillDatabas(LevFakturaHuvud lFaktura)
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
