﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adk = AdkNetWrapper;

namespace UCSTest
{
    class VismaData
    {
        List<KundFakturaHuvud> KundFakturor;
        List<LevFakturaHuvud> LevFakturor;
        Adk.Api.ADKERROR error;
        SqlConnection sqlCon = new SqlConnection(
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\users\sijoh0500\Work Folders\Documents\Github\UCSTest\UCSTest\fakturaDB.mdf;Integrated Security=True");
        String ftg = @"C:\ProgramData\SPCS\SPCS Administration\Företag\Ovnbol2000";
        String sys = @"C:\ProgramData\SPCS\SPCS Administration\Gemensamma filer";
        int pData;
        int antalFakturorUtanNr = 1;

        public VismaData()
        {
            KundFakturor = new List<KundFakturaHuvud>();
            LevFakturor = new List<LevFakturaHuvud>();
            GetKundFakturaHuvudData();
            //GetLevFakturaHuvudData();
            
        }

        private void GetLevFakturaHuvudData()
        {
            // Öppnar upp ett företag
            error = Adk.Api.AdkOpen(ref sys, ref ftg);

            // Kontroll om företaget kunde öppnas
            if (error.lRc != Adk.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)Adk.Api.ADK_ERROR_TEXT_TYPE.elRc;
                Adk.Api.AdkGetErrorText(ref error, errtype,
                ref errortext, 200);
                Console.WriteLine(errortext);
            }

            // Gör pData till en referens av typen leverantörsfaktura
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_SUPPLIER_INVOICE_HEAD);

            // Pekar pData mot den första raden i leverantörsfakturahuvud
            error = AdkNetWrapper.Api.AdkFirst(pData);

            // Kontroll om det det finns något värde i pData
            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som borde fortgå så länge det finns fakturor
           // for (int i = 0; i < 30; i++) // Test som bara kör 30 varv
            {

                LevFakturaHuvud lFakturaHuvud = new LevFakturaHuvud(); 
                Double lopNummer = new double(); // löpnummer ADK_SUP_INV_HEAD_GIVEN_NUMBER
                String levNummer = new String(' ', 16); // leverantörsnummer ADK_SUP_INV_HEAD_SUPPLIER_NUMBER
                String levNamn = new String(' ', 50); // leverantörsnamn ADK_SUP_INV_HEAD_SUPPLIER_NAME
                String fakturaNummer = new String(' ', 16); // ADK_SUP_INV_HEAD_INVOICE_NUMBER
                int tmpDatum = new int(); // Temporär datumhållare då data hämtas som 8 siffror
                String fakturaDatum = new String(' ', 11); // ADK_SUP_INV_HEAD_INVOICE_DATE
                String valutaKod = new String(' ', 4); // Valutakod ADK_SUP_INV_HEAD_CURRENCY_CODE 
                double valutaKurs = 0.00;
                String fakturaTyp = new String(' ', 12); // fakturatyp F = vanlig faktura, K = kreditfaktura ADK_SUP_INV_HEAD_TYPE_OF_INVOICE
                Double totalKostnad = new Double(); // ADK_SUP_INV_HEAD_TOTAL
                String projektHuvud = new String(' ', 10);

                


                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_GIVEN_NUMBER, ref lopNummer);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_SUPPLIER_NUMBER, ref levNummer, 16);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_SUPPLIER_NAME, ref levNamn, 50);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_INVOICE_NUMBER, ref fakturaNummer, 16);

                // Ger fakturor utan nummer ett eget fakturanummer
                if (fakturaNummer == "" || fakturaNummer == null)
                {
                    fakturaNummer = "Faktura-" + antalFakturorUtanNr;
                    antalFakturorUtanNr++;
                }

                error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_INVOICE_DATE, ref tmpDatum);
                error = AdkNetWrapper.Api.AdkLongToDate(tmpDatum, ref fakturaDatum, 11);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_CURRENCY_CODE, ref valutaKod, 4);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_CURRENCY_RATE, ref valutaKurs);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_TYPE_OF_INVOICE, ref fakturaTyp, 12);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_TOTAL, ref totalKostnad);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_PROJECT, ref projektHuvud, 10);

                lFakturaHuvud.LopNummer = lopNummer;
                lFakturaHuvud.LevNummer = levNummer;
                lFakturaHuvud.LevNamn = levNamn;
                lFakturaHuvud.FakturaNummer = fakturaNummer;
                lFakturaHuvud.FakturaDatum = fakturaDatum;
                lFakturaHuvud.ValutaKod = valutaKod;
                lFakturaHuvud.ValutaKurs = decimal.Parse(valutaKurs.ToString());
                lFakturaHuvud.FakturaTyp = fakturaTyp;
                lFakturaHuvud.TotalKostnad = totalKostnad;
                lFakturaHuvud.ProjektHuvud = projektHuvud;

                // Lägger till fakturan till listan med Leverantörsfakturor
                LevFakturor.Add(lFakturaHuvud);

                #region Sql Connection för Fakturahuvud

                SqlCommand cmdAddInvoice = new SqlCommand("sp_add_levInvoice", sqlCon);

                cmdAddInvoice.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter("@fakturaNummer", fakturaNummer);
                SqlParameter param2 = new SqlParameter("@fakturaTyp", fakturaTyp);
                SqlParameter param3 = new SqlParameter("@levNummer", levNummer);
                SqlParameter param4 = new SqlParameter("@lopNummer", lopNummer);
                SqlParameter param5 = new SqlParameter("@levNamn", levNamn);
                SqlParameter param8 = new SqlParameter("@fakturaDatum", fakturaDatum);
                SqlParameter param9 = new SqlParameter("@totalKostnad", totalKostnad);
                SqlParameter param10 =  new SqlParameter("@valutaKod", valutaKod);
                SqlParameter param11 = new SqlParameter("@valutaKurs", decimal.Parse(valutaKurs.ToString()));
                SqlParameter param12=  new SqlParameter("@projektHuvud", lFakturaHuvud.ProjektHuvud);

                var returnParam = cmdAddInvoice.Parameters.Add("@ReturnValue", SqlDbType.Int);
                returnParam.Direction = ParameterDirection.ReturnValue;

                cmdAddInvoice.Parameters.Add(param1);
                cmdAddInvoice.Parameters.Add(param2);
                cmdAddInvoice.Parameters.Add(param3);
                cmdAddInvoice.Parameters.Add(param4);
                cmdAddInvoice.Parameters.Add(param5);
                cmdAddInvoice.Parameters.Add(param8);
                cmdAddInvoice.Parameters.Add(param9);
                cmdAddInvoice.Parameters.Add(param10);
                cmdAddInvoice.Parameters.Add(param11);
                cmdAddInvoice.Parameters.Add(param12);

                sqlCon.Open();
                cmdAddInvoice.ExecuteNonQuery();
                var returnFromSp = returnParam.Value;
                sqlCon.Close();
                Console.WriteLine("Returned value from sp is: {0}, and is of type: {1}", returnFromSp.ToString(), returnFromSp.GetType());
                //if (int.Parse(returnFromSp.ToString()) != 0)
                //{
                //    // Anropar metod som hämtar information om de olika raderna i leverantörsfakturan
                //    GetLevFakturaRad(lFakturaHuvud, pData);
                //}

                GetLevFakturaRad(lFakturaHuvud, pData);

                #endregion

                // Sätter vidare pekaren på nästa instans
                error = AdkNetWrapper.Api.AdkNext(pData);

            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();

            int räknare = 1;

            // Loopar igenom kundfakturor och gör testutskrifter
            foreach (var faktura in LevFakturor)
            {
                Console.WriteLine(räknare + " Löpnummer: "+ faktura.LopNummer
                    + " Levnamn " + faktura.LevNamn
                    + " LevNr: " + faktura.LevNummer
                    + " Fakturanummer: " + faktura.FakturaNummer
                    + " datum: " + faktura.FakturaDatum
                    + " kostnad: " + faktura.TotalKostnad                    
                    + " fakturatyp: " + faktura.FakturaTyp
                    + " Valuta: " + faktura.ValutaKod);
                räknare++;

                // Loopar raderna på varje faktura och göra testutskrifter

                foreach (var rad in faktura.fakturaRader)
                {
                    Console.WriteLine("ArtikelNr: " + rad.ArtikelNummer
                        + " Information: " + rad.Information + " Kvantitet: " + rad.Kvantitet
                        + " Styckpris: " + rad.PrisPerEnhet
                        + " Totalkostnad för artikeln: " + rad.TotalKostnad
                        + " LevArtikelNr: " + rad.LevArtikelNummer);
                }
                Console.WriteLine();

            }

            // Ser till så att konsolen inte stänger av sig så fort programmet har körts
            //Console.ReadLine();

        }

        private void GetLevFakturaRad(LevFakturaHuvud lFaktura, int pData)
        {            

            Double NROWS = new Double();
            error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_NROWS, ref NROWS);
            int radReferens = new int(); // Pekar på rader i fakturaraderna likt pData pekar på ett specifikt fakturahuvud

            for (int r = 0; r < NROWS; r++)
            {
                LevFakturaRad enFakturaRad = new LevFakturaRad();
                error = AdkNetWrapper.Api.AdkGetData(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_ROWS, r, ref radReferens);

                if (error.lRc == AdkNetWrapper.Api.ADKE_OK)
                {
                    String information = new String(' ', 60);
                    Double kvantitet = new Double();
                    Double prisPerEnhet = new Double();
                    String artikelNummer = new String(' ', 16); // internt artikelnummer
                    String levArtikelNummer = new String(' ', 16); // Leverantörens artikelnummer
                    double test = 0.00;
                    String projektRad = new String(' ', 10);


                    error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_TEXT, ref information, 60);
                    error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_QUANTITY1, ref kvantitet);
                    error = AdkNetWrapper.Api.AdkGetStr(radReferens,
                        AdkNetWrapper.Api.ADK_OOI_ROW_SUPPLIER_ARTICLE_NUMBER, ref levArtikelNummer, 16);
                    error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_ARTICLE_NUMBER,
                        ref artikelNummer, 16);
                    error = AdkNetWrapper.Api.AdkGetStr(radReferens,
                        AdkNetWrapper.Api.ADK_OOI_ROW_PROJECT, ref projektRad, 10);
                    error = AdkNetWrapper.Api.AdkGetDouble(radReferens,
                        AdkNetWrapper.Api.ADK_OOI_ROW_PRICE_EACH_CURRENT_CURRENCY, ref prisPerEnhet);

                    if (prisPerEnhet == 0 || prisPerEnhet == null)
                    {
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens,
                            AdkNetWrapper.Api.ADK_OOI_ROW_AMOUNT_CURRENT_CURRENCY, ref prisPerEnhet);
                        
                    }

                    if (kvantitet == 0 || kvantitet == null)
                        enFakturaRad.TotalKostnad = prisPerEnhet;
                    else
                    {
                        enFakturaRad.TotalKostnad = prisPerEnhet * kvantitet;
                    }

                    enFakturaRad.Information = information;
                    enFakturaRad.Kvantitet = kvantitet;
                    enFakturaRad.PrisPerEnhet = prisPerEnhet;
                    enFakturaRad.ArtikelNummer = artikelNummer;
                    enFakturaRad.LevArtikelNummer = levArtikelNummer;
                    enFakturaRad.ProjektRad = projektRad;
                    Console.WriteLine("Testa: {0}", test);
                }

                lFaktura.fakturaRader.Add(enFakturaRad);

                #region ===== Sql Connection lägg till levFaktura rader =====

                SqlCommand cmdAddRow = new SqlCommand("sp_add_levInvoiceRow", sqlCon);
                cmdAddRow.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter("@artikelNummer", enFakturaRad.ArtikelNummer);
                SqlParameter param2 = new SqlParameter("@information", enFakturaRad.Information);
                SqlParameter param3 = new SqlParameter("@kvantitet", decimal.Parse(enFakturaRad.Kvantitet.ToString()));
                SqlParameter param4 = new SqlParameter("@levArtikelNummer", enFakturaRad.LevArtikelNummer);
                SqlParameter param5 = new SqlParameter("@prisPerEnhet", decimal.Parse(enFakturaRad.PrisPerEnhet.ToString()));
                SqlParameter param6 = new SqlParameter("@totalKostnad", decimal.Parse(enFakturaRad.TotalKostnad.ToString()));
                SqlParameter param7 = new SqlParameter("@fakturaNummer", lFaktura.FakturaNummer);
                SqlParameter param8 = new SqlParameter("@projektRad", enFakturaRad.ProjektRad);

                
                cmdAddRow.Parameters.Add(param1);
                cmdAddRow.Parameters.Add(param2);
                cmdAddRow.Parameters.Add(param3);
                cmdAddRow.Parameters.Add(param4);
                cmdAddRow.Parameters.Add(param5);
                cmdAddRow.Parameters.Add(param6);
                cmdAddRow.Parameters.Add(param7);
                cmdAddRow.Parameters.Add(param8);

                sqlCon.Open();
                cmdAddRow.ExecuteNonQuery();
                sqlCon.Close();

                #endregion
            }
        }

        // Kingig metod som hämtar data från fakturahuvud i visma
        private void GetKundFakturaHuvudData()
        {
            // oklart vilken sökväg som är korrekt, den översta är tagen från sökvägen som presenteras av Visma på min lokala dator
            // Den andra sökvägen är tagen från API:n "Om installationen är gjord på standardsätt", båda fungerar.

            // String ftg = @"C:\ProgramData\SPCS\SPCS Administration\Företag\Ovnbol2000";
            // String sys = @"C:\ProgramData\SPCS\SPCS Administration\Gemensamma filer";

            //String sys = @"C:\Documents and Settings\All Users\Application Data\SPCS\SPCS Administration\Gemensamma filer";
            //String ftg = @"C:\Documents and Settings\All Users\Application Data\SPCS\SPCS Administration\Företag\Ovnbol2000";

            

            // Öppnar upp ett företag
            error = Adk.Api.AdkOpen(ref sys, ref ftg);

            // Kontroll om företaget kunde öppnas
            if (error.lRc != Adk.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)Adk.Api.ADK_ERROR_TEXT_TYPE.elRc;
                Adk.Api.AdkGetErrorText(ref error, errtype,
                ref errortext, 200);
                Console.WriteLine(errortext);
            }

            // Enligt api är PADK_DATA en pekare som måste deklareras för att skapa en ADk_DATA-struktur (Kommando som inte funkar)
            // I exemplen senare i API deklareras det direkt som en integer utan PADK_DATA pekaren
            // PADK_DATA pData;
            // int pData;

            // gör pData till en kundfakturahuvud-referens
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_INVOICE_HEAD);

            // Pekar pData mot första raden i kundafaktura-tabellen
            error = AdkNetWrapper.Api.AdkFirst(pData);

            // Kontroll om det det finns något värde i pData
            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }


            // Else?
            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som borde fortgå så länge det finns fakturor
            //for (int i = 0; i < 16; i++)
            {

                
                KundFakturaHuvud kFaktura = new KundFakturaHuvud();
                String kundNamn = new String(' ', 50);
                Double totalKostnad = new Double();
                int tmpDatum = new int(); 
                String fakturaDatum = new String(' ', 11);
                String kundNr = new String(' ', 16);
                Double fakturaNr = new Double();
                String fakturaTyp = new String(' ', 20);
                String kundLand = new String(' ', 24);
                String kundStad = new String(' ', 24);
                String säljare = new String(' ', 25);
                String kundReferens = new String(' ', 50);
                String valutaKod = new String(' ', 4);
                Double valutaKurs = new Double();
                Double valutaEnhet = new Double();
                Double cargoAmount = new Double();
                Double dispatchFee = new Double();
                
                // String projektKod = new String(' ', 10);


                // Hämtar en sträng om customerns namn i pData
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_NAME, ref kundNamn, 50);

                // Kontroll om namnet är ok
                // Skall sådan kontroll göras på all data?
                if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
                {
                    String errortext = new String(' ', 200);
                    int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                    AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                    Console.WriteLine(errortext);
                }

                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_TOTAL_AMOUNT, ref totalKostnad);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DOCUMENT_NUMBER, ref fakturaNr);                
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_NUMBER, ref kundNr, 16);
                error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DOCUMENT_DATE1, ref tmpDatum);
                error = AdkNetWrapper.Api.AdkLongToDate(tmpDatum, ref fakturaDatum, 11);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_TYPE_OF_INVOICE, ref fakturaTyp, 20);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CITY, ref kundStad, 24);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_COUNTRY, ref kundLand, 24);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_OUR_REFERENCE_NAME, ref säljare, 24);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CURRENCY_CODE, ref valutaKod, 4);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CURRENCY_RATE, ref valutaKurs);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CURRENCY_UNIT, ref valutaEnhet);               
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_REFERENCE_NAME, ref kundReferens, 50);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CARGO_AMOUNT, ref cargoAmount);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DISPATCH_FEE, ref dispatchFee);
                // error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_PROJECT_C, ref projektKod, 10);

                /* Följande konverterare finns i exemplet, men datumet funkar fint för mig redan
                DateTime FktDatumDateTime = Convert.ToDateTime(FktDatum);
                if (Convert.ToInt32(FktDatumDateTime.Month) < 10)
                {
                    FakturaHuvudObj.ArManad = FktDatumDateTime.Year + "0" + FktDatumDateTime.Month + "";
                }
                else
                {
                    FakturaHuvudObj.ArManad = FktDatumDateTime.Year + "" + FktDatumDateTime.Month + "";
                }
                */

                // Lägger info i kundfakturan
                kFaktura.KundNamn = kundNamn;
                kFaktura.TotalKostnad = totalKostnad;
                kFaktura.FakturaDatum = fakturaDatum;
                kFaktura.FakturaNummer = fakturaNr;
                kFaktura.KundNummer = kundNr;
                kFaktura.FakturaTyp = fakturaTyp;
                kFaktura.KundLand = kundLand;
                kFaktura.KundStad = kundStad;
                kFaktura.Säljare = säljare;
                kFaktura.KundReferens = kundReferens;
                kFaktura.ValutaKod = valutaKod;
                kFaktura.ValutaKurs = valutaKurs;
                kFaktura.ValutaEnhet = valutaEnhet;
                kFaktura.Cargo_amount = cargoAmount;
                kFaktura.Dispatch_fee = dispatchFee;

                // Lägger till fakturan till listan med kundfakturor
                KundFakturor.Add(kFaktura);

                #region Sql Connection för Fakturahuvud

                SqlCommand cmdAddInvoice = new SqlCommand("sp_add_customerInvoice", sqlCon);

                cmdAddInvoice.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter("@fakturaNummer", (int)fakturaNr);
                SqlParameter param2 = new SqlParameter("@fakturaTyp", fakturaTyp);
                SqlParameter param3 = new SqlParameter("@kundNummer", int.Parse(kundNr));
                SqlParameter param4 = new SqlParameter("@säljare", säljare);
                SqlParameter param5 = new SqlParameter("@kundNamn", kundNamn);
                SqlParameter param6 = new SqlParameter("@kundStad", kundStad);
                SqlParameter param7 = new SqlParameter("@kundLand", kundLand);
                SqlParameter param8 = new SqlParameter("@fakturaDatum", fakturaDatum);
                SqlParameter param9 = new SqlParameter("@totalKostnad", totalKostnad);
                SqlParameter param10 = new SqlParameter("@förfalloDatum", "");
                SqlParameter param11 = new SqlParameter("@slutDatum", "");
                SqlParameter param12 = new SqlParameter("@valutaKod", valutaKod);
                SqlParameter param13 = new SqlParameter("@valutaKurs", valutaKurs);
                SqlParameter param14 = new SqlParameter("@fraktAvgift", cargoAmount);
                SqlParameter param15 = new SqlParameter("@administrationsAvgift", dispatchFee);

                SqlParameter returnParam = cmdAddInvoice.Parameters.Add("@ReturnValue", SqlDbType.Int);
                returnParam.Direction = ParameterDirection.ReturnValue;

                cmdAddInvoice.Parameters.Add(param1);
                cmdAddInvoice.Parameters.Add(param2);
                cmdAddInvoice.Parameters.Add(param3);
                cmdAddInvoice.Parameters.Add(param4);
                cmdAddInvoice.Parameters.Add(param5);
                cmdAddInvoice.Parameters.Add(param6);
                cmdAddInvoice.Parameters.Add(param7);
                cmdAddInvoice.Parameters.Add(param8);
                cmdAddInvoice.Parameters.Add(param9);
                cmdAddInvoice.Parameters.Add(param10);
                cmdAddInvoice.Parameters.Add(param11);
                cmdAddInvoice.Parameters.Add(param12);
                cmdAddInvoice.Parameters.Add(param13);
                cmdAddInvoice.Parameters.Add(param14);
                cmdAddInvoice.Parameters.Add(param15);

                sqlCon.Open();
                cmdAddInvoice.ExecuteNonQuery();
                var returnFromSp = returnParam.Value;
                sqlCon.Close();

                if (int.Parse(returnFromSp.ToString()) != 0)
                {
                    // Anropar metod som hämtar information om de olika raderna i fakturorna
                    GetKundFakturaRad(kFaktura, pData);


                }

                #endregion
                GetKundFakturaRad(kFaktura, pData);

                // Sätter vidare pekaren på nästa instans
                error = AdkNetWrapper.Api.AdkNext(pData);
            }
           

            

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();

            /*
            int räknare = 1;
            // Loopar igenom kundfakturor och gör testutskrifter
            foreach (var faktura in KundFakturor)
                
            {
                Console.WriteLine(räknare+" "+faktura.KundNamn
                    +" KudNr: "+faktura.KundNummer
                    +" Fakturanummer: "+faktura.FakturaNummer
                    +" datum: "+faktura.FakturaDatum
                    +" kostnad: "+ faktura.TotalKostnad
                    +" Säljare: "+faktura.Säljare
                    +" Land: "+faktura.KundLand
                    +" Stad: "+faktura.KundStad
                    +" fakturatyp: "+faktura.FakturaTyp);
                räknare++;

                // Loopar raderna på varje faktura och göra testutskrifter
                
                foreach (var rad in faktura.fakturaRader)
                {
                    Console.WriteLine("ArtikelNr: " + rad.ArtikelNummer
                        + " Benämning: " + rad.Benämning + " Kvantitet: " + rad.LevAntal
                        + " Styckpris: "+rad.StyckPris
                        + " Totalkostnad för artikeln: "+rad.TotalKostnad
                        + " Enhetstyp: "+rad.EnhetsTyp);
                }
                Console.WriteLine();
                
            }
            */
            // Ser till så att konsolen inte stänger av sig så fort programmet har körts
            //Console.ReadLine();

        }

        // Kingig metod som hämtar informationen om raderna i fakturorna (Alltså vad som beställts)
        private void GetKundFakturaRad(KundFakturaHuvud Faktura, int pData)
        {

            Double NROWS = new Double();
            error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_NROWS, ref NROWS);
            int radReferens = new int(); // Pekar på rader i fakturaraderna likt pData pekar på ett specifikt fakturahuvud

            for (int r = 0; r < NROWS; r++)
            {
                KundFakturaRad enFakturaRad = new KundFakturaRad();
                error = AdkNetWrapper.Api.AdkGetData(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_ROWS, r, ref radReferens);

                if (error.lRc == AdkNetWrapper.Api.ADKE_OK)
                {
                    String artikelNummer = new String(' ', 16);
                    error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                    enFakturaRad.ArtikelNummer = artikelNummer;

                    Double kvantitet = new Double();
                    error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_QUANTITY1, ref kvantitet);

                    
                    enFakturaRad.LevAntal = kvantitet;

                    // Om krediterade faktura
                    if (Faktura.FakturaTyp.ToUpper() == "K")
                    {
                        enFakturaRad.LevAntal = kvantitet * -1;
                    }
                    else
                    {
                        enFakturaRad.LevAntal = kvantitet;
                    }

                    

                    if (enFakturaRad.ArtikelNummer == "Avtalsperiod")
                    {
                        String text = new String(' ', 60);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_TEXT, ref text, 60);
                        enFakturaRad.Benämning = text.Replace(";", "");
                    }

                    else 
                    {
                        String text = new String(' ', 60);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_TEXT, ref text, 60);
                        enFakturaRad.Benämning = text.Replace(";", "");

                        Double prisPerStyck = new Double();
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PRICE_EACH_CURRENT_CURRENCY, ref prisPerStyck);
                        enFakturaRad.StyckPris = prisPerStyck;

                        enFakturaRad.TotalKostnad = kvantitet * prisPerStyck;

                        String enhetsTyp = new String(' ', 4);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_UNIT, ref enhetsTyp, 4);
                        enFakturaRad.EnhetsTyp = enhetsTyp;

                        String PROJECT = new String(' ', 6);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PROJECT, ref PROJECT, 6);
                        enFakturaRad.Projekt = PROJECT;

                    }


                }


                Faktura.fakturaRader.Add(enFakturaRad);

                
                #region Sql Connection lägg till kundfaktura rader

                SqlCommand cmdAddRow = new SqlCommand("sp_add_customerInvoiceRow", sqlCon);
                cmdAddRow.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter("@artikelNummer", enFakturaRad.ArtikelNummer);
                SqlParameter param2 = new SqlParameter("@benämning", enFakturaRad.Benämning);
                SqlParameter param3 = new SqlParameter("@levAntal", enFakturaRad.LevAntal.ToString());
                SqlParameter param4 = new SqlParameter("@enhetsTyp", enFakturaRad.EnhetsTyp);
                SqlParameter param5 = new SqlParameter("@styckPris", enFakturaRad.StyckPris.ToString());
                SqlParameter param6 = new SqlParameter("@totalKostnad", enFakturaRad.TotalKostnad);
                SqlParameter param7 = new SqlParameter("@fakturaNummer", (int)Faktura.FakturaNummer);
                SqlParameter param8 = new SqlParameter("@projekt", enFakturaRad.Projekt);
               
                cmdAddRow.Parameters.Add(param1);
                cmdAddRow.Parameters.Add(param2);
                cmdAddRow.Parameters.Add(param3);
                cmdAddRow.Parameters.Add(param4);
                cmdAddRow.Parameters.Add(param5);
                cmdAddRow.Parameters.Add(param6);
                cmdAddRow.Parameters.Add(param7);
                cmdAddRow.Parameters.Add(param8);

                sqlCon.Open();
                cmdAddRow.ExecuteNonQuery();
                sqlCon.Close();

                #endregion


            }




        }
    }
}
