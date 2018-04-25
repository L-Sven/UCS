using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace UCSTest
{
    class SkickaData
    {
        //Skapar en sql-connection mot databasen
        private SqlConnection sqlCon = new SqlConnection(
              @"Data Source=SIMONJO-6570B\UCSTEST;Initial Catalog=UCSTest;Persist Security Info=True;User ID=sa;Password=Ucstest2018");

        ErrorLogger logger = new ErrorLogger();

        public void AvtalTillDatabas(Avtal a)
        {
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddAgreement = new SqlCommand("sp_add_agreement", sqlCon);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddAgreement.CommandType = CommandType.StoredProcedure;

            cmdAddAgreement.Parameters.Add(new SqlParameter("@dokumentNummer", a.DokumentNummer));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@avtalsDatum", a.AvtalsDatum));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@startDatum", a.StartDatum));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@tempSlutDatum", a.KommenteratSlutDatum));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@kundNummer", a.KundNummer));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@fakturaIntervall", decimal.Parse(a.FakturaIntervall.ToString())));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@periodStart", a.PeriodStart));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@periodEnd", a.PeriodEnd));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@totalKostnad", decimal.Parse(a.TotalKostnad.ToString())));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@uppsägningstid", a.Uppsägningstid));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@förlängningstid", a.Förlängningstid));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@avtalsDatumSlut", a.AvtalsDatumSlut));

            try
            {
                sqlCon.Open();
                cmdAddAgreement.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (SqlException e)
            {
                logger.ErrorMessage(e);
            }
            

            foreach (var rad in a.ListAvtalsRad)
            {
                // Pekar Sql-connection mot en stored procedure för kundfakturarad
                SqlCommand cmdAddAgreementRow = new SqlCommand("sp_add_agreementrow", sqlCon);

                // Ger Sql-kommandot information om att den ska anropa en stored procedure
                cmdAddAgreementRow.CommandType = CommandType.StoredProcedure;

                cmdAddAgreementRow.Parameters.Add(new SqlParameter("@dokumentNummer", a.DokumentNummer));
                cmdAddAgreementRow.Parameters.Add(new SqlParameter("@radID", rad.RadId));
                cmdAddAgreementRow.Parameters.Add(new SqlParameter("@artikelNummer", rad.ArtikelNummer));
                cmdAddAgreementRow.Parameters.Add(new SqlParameter("@totalKostnad", rad.TotalKostnad));

                try
                {
                    sqlCon.Open();
                    cmdAddAgreementRow.ExecuteNonQuery();
                    sqlCon.Close();
                }
                catch (SqlException e)
                {
                    logger.ErrorMessage(e);
                }
            }
        }

        public void ArtikelGruppTillDatabas(ArtikelGrupp g)
        {
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddArticleGroup = new SqlCommand("sp_add_articlegroup", sqlCon);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddArticleGroup.CommandType = CommandType.StoredProcedure;

            cmdAddArticleGroup.Parameters.Add(new SqlParameter("@aGroupCode", g.ArtikelGruppKod));
            cmdAddArticleGroup.Parameters.Add(new SqlParameter("@aGroupBenämning", g.Benämning));

            try
            {
                sqlCon.Open();
                cmdAddArticleGroup.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (SqlException e)
            {
                logger.ErrorMessage(e);
            }
        }

        // Metod som tar emot en artikel och lägger till den i databasen
        public void ArtikelTillDatabas(Artikel a)
        {
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddArticle = new SqlCommand("sp_add_article", sqlCon);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddArticle.CommandType = CommandType.StoredProcedure;
            

            cmdAddArticle.Parameters.Add(new SqlParameter("@artikelNummer", a.ArtikelNummer));
            cmdAddArticle.Parameters.Add(new SqlParameter("@artikelGrupp", a.ArtikelGrupp));
            cmdAddArticle.Parameters.Add(new SqlParameter("@benämning", a.Benämning));
            cmdAddArticle.Parameters.Add(new SqlParameter("@enhetsKod", a.EnhetsKod));
            cmdAddArticle.Parameters.Add(new SqlParameter("@inköpsPris", a.InköpsPris));
            cmdAddArticle.Parameters.Add(new SqlParameter("@frakt", a.Frakt));
            cmdAddArticle.Parameters.Add(new SqlParameter("@ovrigKostnad", a.OvrigKostnad));

            try
            {
                sqlCon.Open();
                cmdAddArticle.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (SqlException e)
            {
                logger.ErrorMessage(e);
            }
        }

        public void ResultatenhetTillDatabas(Resultatenhet r)
        {
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddResultatEnhet = new SqlCommand("sp_add_resultatenhet", sqlCon);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddResultatEnhet.CommandType = CommandType.StoredProcedure;

            cmdAddResultatEnhet.Parameters.Add(new SqlParameter("@resultatEnhetID", r.resultatEnhetID));
            cmdAddResultatEnhet.Parameters.Add(new SqlParameter("@resultatEnhetNamn", r.resultatEnhetNamn));

            try
            {
                sqlCon.Open();
                cmdAddResultatEnhet.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (SqlException e)
            {
                logger.ErrorMessage(e);
            }
        }

        // Metod som tar emot en kundfaktura och lägger till den i databasen
        public void KundFakturaTillDatabas(KundFakturaHuvud kFaktura)
        {
            KundTillDatabas(kFaktura);
            // Pekar Sql-connection mot en stored procedure för kundfakturor
            SqlCommand cmdAddInvoice = new SqlCommand("sp_add_customerInvoice", sqlCon);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddInvoice.CommandType = CommandType.StoredProcedure;


            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaNummer", kFaktura.FakturaNummer));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaTyp", kFaktura.FakturaTyp));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@kundNummer", int.Parse(kFaktura.KundNummer)));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@säljare", kFaktura.Säljare));            
            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaDatum", kFaktura.FakturaDatum));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@totalKostnad", decimal.Parse(kFaktura.TotalKostnad.ToString())));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@förfalloDatum", ""));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@slutDatum", ""));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@fraktAvgift", kFaktura.Cargo_amount));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@administrationsAvgift", kFaktura.Dispatch_fee));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@moms", kFaktura.Moms));
            //cmdAddInvoice.Parameters.Add(new SqlParameter("@kommentarsFält", kFaktura.KommentarsFält));

            try
            {
                sqlCon.Open();
                cmdAddInvoice.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (SqlException e)
            {
                logger.ErrorMessage(e);
            }

            // Snurra som lägger till alla raderna från kundfakturan i databasen
            foreach (var fRad in kFaktura.fakturaRader)
            {
                // Pekar Sql-connection mot en stored procedure för kundfakturarad
                SqlCommand cmdAddRow = new SqlCommand("sp_add_customerInvoiceRow", sqlCon);

                // Ger Sql-kommandot information om att den ska anropa en stored procedure
                cmdAddRow.CommandType = CommandType.StoredProcedure;

                cmdAddRow.Parameters.Add(new SqlParameter("@radID", fRad.KundRadID));
                cmdAddRow.Parameters.Add(new SqlParameter("@artikelNummer", fRad.ArtikelNummer));
                cmdAddRow.Parameters.Add(new SqlParameter("@levAntal", fRad.LevAntal));
                cmdAddRow.Parameters.Add(new SqlParameter("@totalKostnad", fRad.TotalKostnad));
                cmdAddRow.Parameters.Add(new SqlParameter("@fakturaNummer", kFaktura.FakturaNummer));
                cmdAddRow.Parameters.Add(new SqlParameter("@projekt", fRad.Projekt));
                cmdAddRow.Parameters.Add(new SqlParameter("@täckningsGrad", decimal.Parse(fRad.TäckningsGrad.ToString())));
                cmdAddRow.Parameters.Add(new SqlParameter("@benämning", fRad.Benämning));
                cmdAddRow.Parameters.Add(new SqlParameter("@täckningsBidrag", decimal.Parse(fRad.TäckningsBidrag.ToString())));
                cmdAddRow.Parameters.Add(new SqlParameter("@resultatEnhetID", fRad.ResultatEnhet));

                try
                {
                    sqlCon.Open();
                    cmdAddRow.ExecuteNonQuery();
                    sqlCon.Close();
                }
                catch (SqlException e)
                {
                    logger.ErrorMessage(e);
                }
            }

        }

        // Metod som tar emot en leverantörsfaktura och lägger till den i databasen
        public void LevFakturaTillDatabas(LevFakturaHuvud lFaktura)
        {
            LeverantörTillDatabas(lFaktura);
            // Pekar Sql-connection mot en stored procedure för leverantörsfakturor
            SqlCommand cmdAddInvoice = new SqlCommand("sp_add_levInvoice", sqlCon);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddInvoice.CommandType = CommandType.StoredProcedure;

            /*
            var returnParam = cmdAddInvoice.Parameters.Add("@ReturnValue", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;
            */

            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaNummer", lFaktura.FakturaNummer));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaTyp", lFaktura.FakturaTyp));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@levNummer", lFaktura.LevNummer));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@lopNummer", lFaktura.LopNummer));           
            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaDatum", lFaktura.FakturaDatum));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@totalKostnad", decimal.Parse(lFaktura.TotalKostnad.ToString())));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@projektHuvud", lFaktura.ProjektHuvud));
            cmdAddInvoice.Parameters.Add(new SqlParameter("@moms", lFaktura.Moms));

            try
            {
                sqlCon.Open();
                cmdAddInvoice.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (SqlException e)
            {
                logger.ErrorMessage(e);
            }

            // Snurra som lägger till alla raderna från leverantörsfakturan i databasen
            foreach (var lRad in lFaktura.fakturaRader)
            {
                // Pekar Sql-connection mot en stored procedure för leverantörsfakturarader
                SqlCommand cmdAddRow = new SqlCommand("sp_add_levInvoiceRow", sqlCon);
                
                // Ger Sql-kommandot information om att den ska anropa en stored procedure
                cmdAddRow.CommandType = CommandType.StoredProcedure;

                cmdAddRow.Parameters.Add(new SqlParameter("@radID", lRad.LevRadID));
                cmdAddRow.Parameters.Add(new SqlParameter("@artikelNummer", lRad.ArtikelNummer));
                cmdAddRow.Parameters.Add(new SqlParameter("@information", lRad.Information));
                cmdAddRow.Parameters.Add(new SqlParameter("@kvantitet", decimal.Parse(lRad.Kvantitet.ToString())));
                cmdAddRow.Parameters.Add(new SqlParameter("@levArtikelNummer", lRad.LevArtikelNummer));
                cmdAddRow.Parameters.Add(new SqlParameter("@totalKostnad", decimal.Parse(lRad.TotalKostnad.ToString())));
                cmdAddRow.Parameters.Add(new SqlParameter("@löpNummer", lFaktura.LopNummer.ToString()));
                cmdAddRow.Parameters.Add(new SqlParameter("@projektRad", lRad.ProjektRad));
                cmdAddRow.Parameters.Add(new SqlParameter("@resultatEnhetID", lRad.ResultatEnhet));


                try
                {
                    sqlCon.Open();
                    cmdAddRow.ExecuteNonQuery();
                    sqlCon.Close();
                }
                catch (SqlException e)
                {
                    logger.ErrorMessage(e);
                }
            }
        }

        public void KundTillDatabas(KundFakturaHuvud ku)
        {
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddCustomer = new SqlCommand("sp_add_customer", sqlCon);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddCustomer.CommandType = CommandType.StoredProcedure;
            

            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundNummer", int.Parse(ku.KundNummer)));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundNamn", ku.KundNamn));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundStad", ku.KundStad));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundLand", ku.KundLand));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundReferens", ku.KundReferens));


            try
            {
                sqlCon.Open();
                cmdAddCustomer.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (SqlException e)
            {
                logger.ErrorMessage(e);
            }
        }

        public void LeverantörTillDatabas(LevFakturaHuvud lev)
        {
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddSupplier = new SqlCommand("sp_add_supplier", sqlCon);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddSupplier.CommandType = CommandType.StoredProcedure;


            cmdAddSupplier.Parameters.Add(new SqlParameter("@levNummer", int.Parse(lev.LevNummer)));
            cmdAddSupplier.Parameters.Add(new SqlParameter("@levNamn", lev.LevNamn));


            try
            {
                sqlCon.Open();
                cmdAddSupplier.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (SqlException e)
            {
                logger.ErrorMessage(e);
            }
        }
    }
}
