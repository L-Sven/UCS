using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace UcsAdm
{
    class SkickaData
    {
        //Skapar en sql-connection mot databasen
        public SqlConnection sqlConAdm = new SqlConnection(@ConfigurationManager.AppSettings["dbPath"]);

        ErrorLogger logger = new ErrorLogger();

        public void AvtalTillDatabas(Avtal a, List<string> avtalPrognosList)
        {
            KundTillDatabas(a);

            var sqlCon2 = new SqlConnection(sqlConAdm.ConnectionString);
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddAgreement = new SqlCommand("sp_add_agreement", sqlCon2);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddAgreement.CommandType = CommandType.StoredProcedure;

            cmdAddAgreement.Parameters.Add(new SqlParameter("@dokumentNummer", a.DokumentNummer));
            cmdAddAgreement.Parameters.Add(
                a.AvtalsDatum == null ? new SqlParameter("@avtalsDatum", DBNull.Value) : new SqlParameter("@avtalsDatum", a.AvtalsDatum));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@startDatum", a.StartDatum));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@tempSlutDatum", a.KommenteratSlutDatum));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@kundNummer", a.KundNummer));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@fakturaIntervall",
                decimal.Parse(a.FakturaIntervall.ToString())));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@periodStart", a.PeriodStart));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@periodEnd", a.PeriodEnd));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@beloppExklMoms", decimal.Parse(a.BeloppExklMoms.ToString())));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@uppsägningstid", a.Uppsägningstid));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@förlängningstid", a.Förlängningstid));
            cmdAddAgreement.Parameters.Add(new SqlParameter("@avtalsDatumSlut", a.AvtalsDatumSlut));
            cmdAddAgreement.Parameters.Add(
                a.ResultatEnhet == null ? new SqlParameter("@resultatEnhet", DBNull.Value) : new SqlParameter("@resultatEnhet", a.ResultatEnhet));

            try
            {
                sqlCon2.Open();
                cmdAddAgreement.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.ErrorMessage(e + " | Dokumentnummer: " + a.DokumentNummer);
            }
            finally
            {
                sqlCon2.Close();
            }

            SqlCommand cmdAddKommentarsfält = new SqlCommand("sp_add_commentfield", sqlCon2);
            cmdAddKommentarsfält.CommandType = CommandType.StoredProcedure;

            try
            {
                sqlCon2.Open();
                cmdAddKommentarsfält.Parameters.Add(new SqlParameter("@dokumentNummer", a.DokumentNummer));
                cmdAddKommentarsfält.Parameters.Add(
                    a.Kommentarsfält == null || a.Kommentarsfält.Length < 2 ? new SqlParameter("@kommentarsfält", DBNull.Value) : new SqlParameter("@kommentarsfält", a.Kommentarsfält));
                cmdAddKommentarsfält.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
                
            }

            SqlCommand cmdAddAvtalPrognos = new SqlCommand("sp_add_agreementprediction", sqlCon2);
            cmdAddAvtalPrognos.CommandType = CommandType.StoredProcedure;


            for (int i = 0; i < avtalPrognosList.Count; i++)
            {
                try
                {
                    sqlCon2.Open();
                    cmdAddAvtalPrognos.Parameters.Add(new SqlParameter("@avtalID", a.DokumentNummer));
                    cmdAddAvtalPrognos.Parameters.Add(new SqlParameter("@avtalPrognosDatum", avtalPrognosList[i]));
                    cmdAddAvtalPrognos.ExecuteNonQuery();
                    cmdAddAvtalPrognos.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    logger.ErrorMessage(ex + " | Dokumentnummer: " + a.DokumentNummer);
                }
                finally
                {
                    sqlCon2.Close();
                }
            }

            foreach (var rad in a.ListAvtalsRad)
            {
                // Pekar Sql-connection mot en stored procedure för kundfakturarad
                SqlCommand cmdAddAgreementRow = new SqlCommand("sp_add_agreementrow", sqlCon2);

                // Ger Sql-kommandot information om att den ska anropa en stored procedure
                cmdAddAgreementRow.CommandType = CommandType.StoredProcedure;

                cmdAddAgreementRow.Parameters.Add(new SqlParameter("@dokumentNummer", a.DokumentNummer));
                cmdAddAgreementRow.Parameters.Add(new SqlParameter("@radID", rad.RadId));
                cmdAddAgreementRow.Parameters.Add(new SqlParameter("@artikelNummer", rad.ArtikelNummer));
                cmdAddAgreementRow.Parameters.Add(new SqlParameter("@beloppExklMoms", rad.BeloppExklMoms));
                cmdAddAgreementRow.Parameters.Add(new SqlParameter("@benämning", rad.Benämning));

                try
                {
                    sqlCon2.Open();
                    cmdAddAgreementRow.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    logger.ErrorMessage(e + " | Dokumentnummer: " + a.DokumentNummer);
                }
                finally
                {
                    sqlCon2.Close();
                }
            }
        }

        public void ArtikelGruppTillDatabas(ArtikelGrupp g)
        {
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddArticleGroup = new SqlCommand("sp_add_articlegroup", sqlConAdm);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddArticleGroup.CommandType = CommandType.StoredProcedure;

            cmdAddArticleGroup.Parameters.Add(new SqlParameter("@aGroupCode", g.ArtikelGruppKod));
            cmdAddArticleGroup.Parameters.Add(new SqlParameter("@aGroupBenämning", g.Benämning));

            try
            {
                sqlConAdm.Open();
                cmdAddArticleGroup.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.ErrorMessage(e + " | Artikelgruppkod: " + g.ArtikelGruppKod);
            }
            finally
            {
                sqlConAdm.Close();
            }
        }

        // Metod som tar emot en artikel och lägger till den i databasen
        public void ArtikelTillDatabas(Artikel a)
        {
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddArticle = new SqlCommand("sp_add_article", sqlConAdm);

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
                sqlConAdm.Open();
                cmdAddArticle.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.ErrorMessage(e + " | Artikelnummer:  " + a.ArtikelNummer);
            }
            finally
            {
                sqlConAdm.Close();
            }
        }

        public void ResultatenhetTillDatabas(Resultatenhet r)
        {
            sqlConAdm.Open();

            using (SqlCommand cmdAddResultatEnhet = new SqlCommand("sp_add_resultunit", sqlConAdm))
            {
                // Ger Sql-kommandot information om att den ska anropa en stored procedure
                cmdAddResultatEnhet.CommandType = CommandType.StoredProcedure;
                cmdAddResultatEnhet.Parameters.Add(new SqlParameter("@resultatEnhetID", r.resultatEnhetID));
                cmdAddResultatEnhet.Parameters.Add(new SqlParameter("@resultatEnhetNamn", r.resultatEnhetNamn));

                try
                {
                    cmdAddResultatEnhet.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    logger.ErrorMessage(e.ToString() + " | ResultatenhetsID: " + r.resultatEnhetID);
                }
                finally
                {
                    sqlConAdm.Close();
                }
            }
        }

        // Metod som tar emot en kundfaktura och lägger till den i databasen
        public void KundFakturaTillDatabas(List<KundFakturaHuvud> kList)
        {
            //Vi använder oss av en Parallel for-loop, delar upp arbete och kör det parallelt.
            Parallel.For(0, kList.Count, new ParallelOptions { MaxDegreeOfParallelism = 4}, (i) =>
            {
                KundTillDatabas(kList[i]);
                using (var sqlCon2 = new SqlConnection(sqlConAdm.ConnectionString))
                {
                    using (SqlCommand cmdAddInvoice = new SqlCommand("sp_add_customerInvoice", sqlCon2))
                    {
                        cmdAddInvoice.CommandType = CommandType.StoredProcedure;

                        try
                        {
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaNummer", kList[i].FakturaNummer));
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaTyp", kList[i].FakturaTyp));
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@kundNummer", kList[i].KundNummer));
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@säljare", kList[i].Säljare));
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaDatum", kList[i].FakturaDatum));
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@beloppExklMoms",
                                decimal.Parse(kList[i].BeloppExklMoms.ToString())));
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@förfalloDatum", ""));
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@slutDatum", ""));
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@fraktAvgift", kList[i].Cargo_amount));
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@administrationsAvgift",
                                kList[i].Dispatch_fee));
                            cmdAddInvoice.Parameters.Add(new SqlParameter("@moms", kList[i].Moms));
                            cmdAddInvoice.Parameters.Add(
                                kList[i] == null ? new SqlParameter("@avtalsNummer", DBNull.Value) : new SqlParameter("@avtalsNummer", kList[i].AvtalsNummer));
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorMessage(ex);
                        }

                        try
                        {
                            sqlCon2.Open();
                            cmdAddInvoice.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            logger.ErrorMessage(e.ToString() + " | Fakturanummer: " + kList[i].FakturaNummer);
                        }
                        finally
                        {
                            sqlCon2.Close();
                        }

                        foreach (var fRad in kList[i].fakturaRader)
                        {
                            // Pekar Sql-connection mot en stored procedure för kundfakturarad
                            SqlCommand cmdAddRow = new SqlCommand("sp_add_customerInvoiceRow", sqlCon2);

                            // Ger Sql-kommandot information om att den ska anropa en stored procedure
                            cmdAddRow.CommandType = CommandType.StoredProcedure;

                            try
                            {
                                cmdAddRow.Parameters.Add(new SqlParameter("@radID", fRad.KundRadID));
                                cmdAddRow.Parameters.Add(new SqlParameter("@artikelNummer", fRad.ArtikelNummer));
                                cmdAddRow.Parameters.Add(new SqlParameter("@levAntal", fRad.LevAntal));
                                cmdAddRow.Parameters.Add(new SqlParameter("@beloppExklMoms", fRad.BeloppExklMoms));
                                cmdAddRow.Parameters.Add(new SqlParameter("@fakturaNummer", kList[i].FakturaNummer));
                                cmdAddRow.Parameters.Add(new SqlParameter("@projekt", fRad.Projekt));
                                cmdAddRow.Parameters.Add(new SqlParameter("@täckningsGrad",
                                    decimal.Parse(fRad.TäckningsGrad.ToString())));
                                cmdAddRow.Parameters.Add(new SqlParameter("@benämning", fRad.Benämning));
                                cmdAddRow.Parameters.Add(new SqlParameter("@täckningsBidrag",
                                    decimal.Parse(fRad.TäckningsBidrag.ToString())));
                                cmdAddRow.Parameters.Add(new SqlParameter("@resultatEnhetID", fRad.ResultatEnhet));
                            }
                            catch (Exception ex)
                            {
                                logger.ErrorMessage(ex.ToString() + " | Fakturanummer: " + kList[i].FakturaNummer +
                                                    " | Rad:" + fRad.KundRadID);
                            }

                            try
                            {
                                sqlCon2.Open();
                                cmdAddRow.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                logger.ErrorMessage(ex.ToString() + " | Fakturanummer: " + kList[i].FakturaNummer +
                                                    " | Rad:" + fRad.KundRadID);
                            }
                            finally
                            {
                                sqlCon2.Close();
                            }
                        }
                    }
                }
            });
        }

        // Metod som tar emot en leverantörsfaktura och lägger till den i databasen
        public void LevFakturaTillDatabas(List<LevFakturaHuvud> lList)
        {
            Parallel.For(0, lList.Count, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (i) =>
            {
                LeverantörTillDatabas(lList[i]);
                var sqlCon2 = new SqlConnection(sqlConAdm.ConnectionString);

                using (SqlCommand cmdAddInvoice = new SqlCommand("sp_add_levInvoice", sqlCon2))
                {
                    cmdAddInvoice.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaNummer", lList[i].FakturaNummer));
                        cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaTyp", lList[i].FakturaTyp));
                        cmdAddInvoice.Parameters.Add(new SqlParameter("@levNummer", lList[i].LevNummer));
                        cmdAddInvoice.Parameters.Add(new SqlParameter("@lopNummer", lList[i].LopNummer));
                        cmdAddInvoice.Parameters.Add(new SqlParameter("@fakturaDatum", lList[i].FakturaDatum));
                        cmdAddInvoice.Parameters.Add(new SqlParameter("@totalKostnad",
                            decimal.Parse(lList[i].TotalKostnad.ToString())));
                        cmdAddInvoice.Parameters.Add(new SqlParameter("@projektHuvud", lList[i].ProjektHuvud));
                        cmdAddInvoice.Parameters.Add(new SqlParameter("@moms", lList[i].Moms));
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorMessage(ex);
                    }

                    try
                    {
                        sqlCon2.Open();
                        cmdAddInvoice.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        logger.ErrorMessage(e.ToString() + " | Fakturanummer: " + lList[i].FakturaNummer);
                    }
                    finally
                    {
                        sqlCon2.Close();
                    }

                    // Snurra som lägger till alla raderna från leverantörsfakturan i databasen
                    foreach (var lRad in lList[i].fakturaRader)
                    {
                        // Pekar Sql-connection mot en stored procedure för leverantörsfakturarader
                        SqlCommand cmdAddRow = new SqlCommand("sp_add_levInvoiceRow", sqlCon2);

                        // Ger Sql-kommandot information om att den ska anropa en stored procedure
                        cmdAddRow.CommandType = CommandType.StoredProcedure;

                        try
                        {
                            cmdAddRow.Parameters.Add(new SqlParameter("@radID", lRad.LevRadID));
                            cmdAddRow.Parameters.Add(new SqlParameter("@artikelNummer", lRad.ArtikelNummer));
                            cmdAddRow.Parameters.Add(new SqlParameter("@information", lRad.Information));
                            cmdAddRow.Parameters.Add(new SqlParameter("@kvantitet",
                                decimal.Parse(lRad.Kvantitet.ToString())));
                            cmdAddRow.Parameters.Add(new SqlParameter("@levArtikelNummer", lRad.LevArtikelNummer));
                            cmdAddRow.Parameters.Add(new SqlParameter("@totalKostnad",
                                decimal.Parse(lRad.TotalKostnad.ToString())));
                            cmdAddRow.Parameters.Add(new SqlParameter("@löpNummer", lList[i].LopNummer.ToString()));
                            cmdAddRow.Parameters.Add(new SqlParameter("@projektRad", lRad.ProjektRad));
                            cmdAddRow.Parameters.Add(new SqlParameter("@resultatEnhetID", lRad.ResultatEnhet));
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorMessage(ex.ToString() + " | Fakturanummer: " + lList[i].FakturaNummer +
                                                " | Rad: " + lRad.LevRadID);
                        }

                        try
                        {
                            sqlCon2.Open();
                            cmdAddRow.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorMessage(ex.ToString() + " | Fakturanummer: " + lList[i].FakturaNummer +
                                                " | Rad: " + lRad.LevRadID);
                        }
                        finally
                        {
                            sqlCon2.Close();
                        }
                    }
                }
            });
        }

        public void KundTillDatabas(Avtal ku)
        {
            var sqlCon2 = new SqlConnection(sqlConAdm.ConnectionString);
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddCustomer = new SqlCommand("sp_add_customer", sqlCon2);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddCustomer.CommandType = CommandType.StoredProcedure;

            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundNummer", ku.KundNummer));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundNamn", ku.KundNamn));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundStad", ku.KundStad));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundLand", ku.KundLand));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundReferens", ku.KundReferens));

            try
            {
                sqlCon2.Open();
                cmdAddCustomer.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.ErrorMessage(e.ToString() + " | Kundnummer: " + ku.KundNummer);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void KundTillDatabas(KundFakturaHuvud ku)
        {

            var sqlCon2 = new SqlConnection(sqlConAdm.ConnectionString);
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddCustomer = new SqlCommand("sp_add_customer", sqlCon2);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddCustomer.CommandType = CommandType.StoredProcedure;

            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundNummer", ku.KundNummer));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundNamn", ku.KundNamn));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundStad", ku.KundStad));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundLand", ku.KundLand));
            cmdAddCustomer.Parameters.Add(new SqlParameter("@kundReferens", ku.KundReferens));

            try
            {
                sqlCon2.Open();
                cmdAddCustomer.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.ErrorMessage(e.ToString() + " | Kundnummer: " + ku.KundNummer);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void LeverantörTillDatabas(LevFakturaHuvud lev)
        {
            var sqlCon2 = new SqlConnection(sqlConAdm.ConnectionString);
            // Pekar Sql-connection mot en stored procedure för artiklar
            SqlCommand cmdAddSupplier = new SqlCommand("sp_add_supplier", sqlCon2);

            // Ger Sql-kommandot information om att den ska anropa en stored procedure
            cmdAddSupplier.CommandType = CommandType.StoredProcedure;

            cmdAddSupplier.Parameters.Add(new SqlParameter("@levNummer", lev.LevNummer));
            cmdAddSupplier.Parameters.Add(new SqlParameter("@levNamn", lev.LevNamn));

            try
            {
                sqlCon2.Open();
                cmdAddSupplier.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.ErrorMessage(e.ToString() + " | Leverantörsnummer: " + lev.LevNummer);
            }
            finally
            {
                sqlCon2.Close();
            }
        }
    }
}
