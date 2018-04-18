﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        
        Adk.Api.ADKERROR error;
        readonly SkickaData sendData = new SkickaData();

        // Sökvägar för visma administration
        String ftg = @"C:\ProgramData\SPCS\SPCS Administration\Företag\Ovnbol2000";
        String sys = @"C:\ProgramData\SPCS\SPCS Administration\Gemensamma filer";

        // Följande sökvägar verkar också fungera
        //String sys = @"C:\Documents and Settings\All Users\Application Data\SPCS\SPCS Administration\Gemensamma filer";
        //String ftg = @"C:\Documents and Settings\All Users\Application Data\SPCS\SPCS Administration\Företag\Ovnbol2000";

        int pData;
        int antalFakturorUtanNr = 1; // Används för att ge fakturor utan nummer ett fakturanummer
        int levRadID = 0;  //Används för att skapa individuella Identiteter för Leverantörsfafakturaraderna i databasen.
        int kundRadID = 0;  //Används för att skapa individuella Identiteter för Leverantörsfafakturaraderna i databasen.
        int avtalsRadID = 0; // Används för att skapa individuella identiteter för avtalsraderna i fatabasen

        public VismaData()
        {

            //GetResultatEnhet();
            //Console.WriteLine("Resultatenhet klar!");

            //// Anropar metod som hämtar data om alla artikelgrupper
            //GetArtikelGrupper();
            //Console.WriteLine("Artikelgrupper klar!");

            //// Anropar metod som hämtar data om alla artiklar 
            //GetArtikelData();
            //Console.WriteLine("Artikeldata klar!");

            //// Anropar metod som hämtar data om alla kundfakturor
            //GetKundFakturaHuvudData();
            //Console.WriteLine("Kundfakturadata klar!");

            //// Anropar metod som hämtar data om alla leverantörsfakturor
            //GetLevFakturaHuvudData();
            //Console.WriteLine("Leverantförafkturadata klar!");

            GetAvtal();
            Console.WriteLine("Avtal klar!");

        }

        private void GetResultatEnhet()
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

            // Gör pData till en referens av typen Artikelgrupp
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_CODE_OF_PROFIT_CENTRE);

            // Pekar pData mot den första raden i Artikelgrupper
            error = AdkNetWrapper.Api.AdkFirst(pData);

            // Kontroll om det det finns något värde i pData
            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns artikelgrupper
            {
                Resultatenhet r = new Resultatenhet();
                
                String resultatEnhetID = new String(' ', 6);
                String resultatEnhetNamn = new String(' ', 20);

                
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CODE_OF_PROFIT_CENTRE_CODE, ref resultatEnhetID, 6);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CODE_OF_PROFIT_CENTRE_NAME, ref resultatEnhetNamn, 20);

                r.resultatEnhetID = resultatEnhetID;
                r.resultatEnhetNamn = resultatEnhetNamn;

                sendData.ResultatenhetTillDatabas(r);

                // Sätter vidare pekaren till nästa artikelgrupp
                error = AdkNetWrapper.Api.AdkNext(pData);
            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();


        }

        private void GetAvtal()
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

            // Gör pData till en referens av typen Artikelgrupp
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_AGREEMENT_HEAD);

            // Pekar pData mot den första raden i Artikelgrupper
            error = AdkNetWrapper.Api.AdkFirst(pData);

            // Kontroll om det det finns något värde i pData
            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns artikelgrupper
            {
                Avtal a = new Avtal();

                Double DokumentNummer = new Double();
                int Date = new int();
                int endDate = new int();
                String avtalsDatum = new String(' ', 16);
                String kundNummer = new String(' ', 16);
                String startDatum = new String(' ', 16);
                String slutDatum = new String(' ', 16);
                String kommentarsFält = new String(' ', 120);

                Double intervall = new Double();
                int finnsAvtal = new int();    //isActive i Avtal.cs
                int periodTemp = new int();
                String periodStart = new string(' ', 8);
                String periodEnd = new String(' ', 8);


                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_DOCUMENT_NUMBER, ref DokumentNummer);
                error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_DOCUMENT_DATE1, ref Date);
                error = AdkNetWrapper.Api.AdkLongToDate(Date, ref avtalsDatum, 16);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_CUSTOMER_NUMBER, ref kundNummer, 16);
                error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_DATE_START, ref Date);
                error = AdkNetWrapper.Api.AdkLongToDate(Date, ref startDatum, 16);
                error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_DATE_END, ref endDate);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_LOCAL_REMARK, ref kommentarsFält, 120);

                error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_PERIOD_START, ref periodTemp);
                error = AdkNetWrapper.Api.AdkLongToDate(periodTemp, ref periodStart, 16);
                error = AdkNetWrapper.Api.AdkGetBool(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_PERIOD, ref finnsAvtal);
                error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_PERIOD_END, ref periodTemp);
                error = AdkNetWrapper.Api.AdkLongToDate(periodTemp, ref periodEnd, 16);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_INTERVAL, ref intervall);
                if (endDate == 0)
                {
                    slutDatum = "2099-01-31";
                }
                else
                {
                    error = AdkNetWrapper.Api.AdkLongToDate(endDate, ref slutDatum, 16);
                }
                
                a.DokumentNummer = DokumentNummer;
                a.AvtalsDatum = avtalsDatum;
                a.StartDatum = startDatum;
                a.SlutDatum = slutDatum;
                a.KundNummer = kundNummer;
                a.KommentarsFält = kommentarsFält;

                a.IsActive = finnsAvtal;
                a.FakturaIntervall = intervall;
                a.PeriodStart = periodStart;
                a.PeriodEnd = periodEnd;
                
                
                GetAvtalRad(a, pData);

                foreach (var element in a.ListAvtalsRad)
                {
                    a.TotalKostnad += (double)element.TotalKostnad;
                }

                sendData.AvtalTillDatabas(a);

                // Sätter vidare pekaren till nästa artikelgrupp
                error = AdkNetWrapper.Api.AdkNext(pData);
            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();

            
        }

        private void GetAvtalRad(Avtal avtal, int pData)
        {

            Double NROWS = new Double();

            error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_NROWS, ref NROWS);
            int radReferens = new int();

            for (int r = 0; r < NROWS; r++)
            {
                AvtalsRad enAvtalsRad = new AvtalsRad();

                error = AdkNetWrapper.Api.AdkGetData(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_ROWS, r, ref radReferens);
                avtalsRadID++;

                if (error.lRc == AdkNetWrapper.Api.ADKE_OK)
                {

                    String artikelNummer = new String(' ', 16);
                    Double totalKostnad = new Double();

                    error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                    error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_AMOUNT_DOMESTIC_CURRENCY, ref totalKostnad);

                    enAvtalsRad.ArtikelNummer = artikelNummer;
                    enAvtalsRad.RadId = avtalsRadID;
                    enAvtalsRad.TotalKostnad = totalKostnad;

                    avtal.ListAvtalsRad.Add(enAvtalsRad);
                }

            }
        }

        private void GetArtikelGrupper()
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

            // Gör pData till en referens av typen Artikelgrupp
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_CODE_OF_ARTICLE_GROUP);

            // Pekar pData mot den första raden i Artikelgrupper
            error = AdkNetWrapper.Api.AdkFirst(pData);

            // Kontroll om det det finns något värde i pData
            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns artikelgrupper
            {
                ArtikelGrupp aGrupp = new ArtikelGrupp();

                String aGruppKod = new String(' ', 6);
                String aGruppBenämning = new String(' ', 25);

                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CODE_OF_ARTICLE_GROUP_CODE, ref aGruppKod, 6);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CODE_OF_ARTICLE_GROUP_TEXT, ref aGruppBenämning, 25);

                aGrupp.ArtikelGruppKod = aGruppKod;
                aGrupp.Benämning = aGruppBenämning;

                sendData.ArtikelGruppTillDatabas(aGrupp);

                // Sätter vidare pekaren till nästa artikelgrupp
                error = AdkNetWrapper.Api.AdkNext(pData);

            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();

        }

        // Metod som hämtar artikeldata
        private void GetArtikelData()
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

            // Gör pData till en referens av typen Artikel
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_ARTICLE);

            // Pekar pData mot den första raden i Artiklar
            error = AdkNetWrapper.Api.AdkFirst(pData);

            // Kontroll om det det finns något värde i pData
            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns artiklar
            {

                // Skapar ett nytt artikelobjekt
                Artikel artikel = new Artikel();

                String artikelNummer = new String(' ', 16);
                String benämning = new string(' ', 30);
                String artikelGrupp = new String(' ', 6);
                String enhetsKod = new string(' ', 4);
                Double inköpsPris = new Double();
                Double frakt = new Double();
                Double ovrigKostnad = new Double();

                // Hämtar data från databasen och lagrar i de lokala variablerna
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_ARTICLE_NUMBER, ref artikelNummer, 16);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_ARTICLE_NAME, ref benämning, 30);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_ARTICLE_GROUP, ref artikelGrupp, 6);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_ARTICLE_UNIT_CODE, ref enhetsKod, 4);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_ARTICLE_ESTIMATED_PURCHASE_PRICE, ref inköpsPris);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_ARTICLE_ESTIMATED_CARGO_FEE, ref frakt);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_ARTICLE_ESTIMATED_OTHER, ref ovrigKostnad);

                // Lägger till data den aktuella artikelinstansen
                artikel.ArtikelNummer = artikelNummer;
                artikel.ArtikelGrupp = artikelGrupp;
                artikel.Benämning = benämning;
                artikel.EnhetsKod = enhetsKod;
                artikel.InköpsPris = inköpsPris;
                artikel.Frakt = frakt;
                artikel.OvrigKostnad = ovrigKostnad;

                // Skickar data till sendData
                sendData.ArtikelTillDatabas(artikel);

                // Sätter vidare pekaren till nästa artikel
                error = AdkNetWrapper.Api.AdkNext(pData);
            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();
        }

        // Metod som hämtar data om ett leverantörsfakturahuvud
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

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns fakturor
           // for (int i = 0; i < 30; i++) // Test som bara kör 30 varv
            {

                // Skapar ny instans av ett lFakturaHuvud
                LevFakturaHuvud lFakturaHuvud = new LevFakturaHuvud();

                Double lopNummer = new double(); 
                String levNummer = new String(' ', 16); 
                String levNamn = new String(' ', 50); 
                String fakturaNummer = new String(' ', 16); 
                int tmpDatum = new int(); // Temporär datumhållare eftersom data hämtas som 8 siffror
                String fakturaDatum = new String(' ', 11); 
                String valutaKod = new String(' ', 4);  
                double valutaKurs = 0.00;
                String fakturaTyp = new String(' ', 12); // fakturatyp F = vanlig faktura, K = kreditfaktura 
                Double totalKostnad = new Double(); 
                String projektHuvud = new String(' ', 10);
                Double moms = new Double();

                // Hämtar data ur databas och lagrar i de lokala variablerna
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

                // Hämtar data ur databas och lagrar i de lokala variablerna
                error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_INVOICE_DATE, ref tmpDatum);
                error = AdkNetWrapper.Api.AdkLongToDate(tmpDatum, ref fakturaDatum, 11);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_CURRENCY_CODE, ref valutaKod, 4);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_CURRENCY_RATE, ref valutaKurs);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_TYPE_OF_INVOICE, ref fakturaTyp, 12);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_PROJECT, ref projektHuvud, 10);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_VAT_AMOUNT, ref moms);

                // Lägger till data i instansen och lFakturaHuvud
                lFakturaHuvud.LopNummer = lopNummer;
                lFakturaHuvud.LevNummer = levNummer;
                lFakturaHuvud.LevNamn = levNamn;
                lFakturaHuvud.FakturaNummer = fakturaNummer;
                lFakturaHuvud.FakturaDatum = fakturaDatum;
                lFakturaHuvud.ValutaKod = valutaKod;
                lFakturaHuvud.ValutaKurs = decimal.Parse(valutaKurs.ToString());
                lFakturaHuvud.FakturaTyp = fakturaTyp;
                lFakturaHuvud.ProjektHuvud = projektHuvud;
                lFakturaHuvud.Moms = moms;
                        
                // Anrop till metod som hämtar alla leverantörsfakturarader i den aktuella fakturan
                GetLevFakturaRad(lFakturaHuvud, pData);

                // Anropr till metod som lägger in data i databasen
                sendData.LevFakturaTillDatabas(lFakturaHuvud);

                // Sätter vidare pekaren på nästa instans
                error = AdkNetWrapper.Api.AdkNext(pData);

            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();

            
        }

        // Metod som hämtar rader från en leverantörsfaktura
        private void GetLevFakturaRad(LevFakturaHuvud lFaktura, int pData)
        {
            
            Double NROWS = new Double();

            // Hämtar antalet rader som finns på leverantörsfakturan
            error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_NROWS, ref NROWS);
            int radReferens = new int(); // Pekar på rader i fakturaraderna likt pData pekar på ett specifikt fakturahuvud

            // Snurra som fortgår så länge det finns rader på leverantörsfakturan
            for (int r = 0; r < NROWS; r++)
            {

                // Skapar ny instans av en fakturarad
                LevFakturaRad enFakturaRad = new LevFakturaRad();

                // Sätter radreferensen mot aktuell fakturarad
                error = AdkNetWrapper.Api.AdkGetData(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_ROWS, r, ref radReferens);

                // Autoinkrementad primärnyckel för leverantörsfakturaraden i databasen
                levRadID++;
                enFakturaRad.LevRadID = levRadID;

                if (error.lRc == AdkNetWrapper.Api.ADKE_OK)
                {
                    String information = new String(' ', 60);
                    Double kvantitet = new Double();
                    String artikelNummer = new String(' ', 16); // internt artikelnummer
                    String levArtikelNummer = new String(' ', 16); // Leverantörens artikelnummer
                    String projektRad = new String(' ', 10);
                    Double totalKostnad = new Double();
                    String resultatEnhet = new string(' ', 6);

                    error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_TEXT, ref information, 60);

                    // om raden presenterar en totalkostnad sparas inte raden utan vi använder värdet för att 
                    // ge leverantörsfakturahuvudet en totalkostnad i svenska kronor 
                    if (information.ToLower() == "total")
                    {
                        
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens,
                            AdkNetWrapper.Api.ADK_OOI_ROW_AMOUNT_DOMESTIC_CURRENCY, ref totalKostnad);
                        lFaktura.TotalKostnad = totalKostnad;


                    }

                    // Om det inte är en rad som presenterar totalkostnaden
                    else
                    {
                        // Hämtar data ur databas och lagrar i de lokala variablerna
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_QUANTITY1, ref kvantitet);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_SUPPLIER_ARTICLE_NUMBER, ref levArtikelNummer, 16);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PROJECT, ref projektRad, 10);
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_AMOUNT_DOMESTIC_CURRENCY, ref totalKostnad);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PROFIT_CENTRE, ref resultatEnhet, 6);


                        // Lägger till data i fakturaradinstansen
                        enFakturaRad.Information = information;
                        enFakturaRad.Kvantitet = kvantitet;
                        enFakturaRad.ArtikelNummer = artikelNummer;
                        enFakturaRad.LevArtikelNummer = levArtikelNummer;
                        enFakturaRad.ProjektRad = projektRad;
                        enFakturaRad.TotalKostnad = totalKostnad;
                        enFakturaRad.ResultatEnhet = resultatEnhet;
                        lFaktura.fakturaRader.Add(enFakturaRad);
                    }

                    
                    
                }

            }
        }

        // Metod som hämtar data från fakturahuvud i visma
        private void GetKundFakturaHuvudData()
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


            
            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns fakturor            
            {

                // Skapar ny instans av ett kundafakturahuvud
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
                Double cargoAmount = new Double();
                Double dispatchFee = new Double();
                Double moms = new Double();
                String kommentarsFält = new String(' ', 120);


                // Hämtar data ur databas och lagrar i de lokala variablerna
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_NAME, ref kundNamn, 50);               
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DOCUMENT_NUMBER, ref fakturaNr);                
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_NUMBER, ref kundNr, 16);
                error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DOCUMENT_DATE1, ref tmpDatum);
                error = AdkNetWrapper.Api.AdkLongToDate(tmpDatum, ref fakturaDatum, 11);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_TYPE_OF_INVOICE, ref fakturaTyp, 20);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CITY, ref kundStad, 24);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_COUNTRY, ref kundLand, 24);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_OUR_REFERENCE_NAME, ref säljare, 24);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CURRENCY_CODE, ref valutaKod, 4);             
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_REFERENCE_NAME, ref kundReferens, 50);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CARGO_AMOUNT, ref cargoAmount);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DISPATCH_FEE, ref dispatchFee);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_VAT_AMOUNT, ref moms);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_TOTAL_AMOUNT, ref totalKostnad);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_REFERENCE_NAME, ref kommentarsFält, 120);

                /* Följande konverterare finns i kodexemplet om man vill dela upp data över månader/år
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
                kFaktura.Cargo_amount = cargoAmount;
                kFaktura.Dispatch_fee = dispatchFee;
                kFaktura.Moms = moms;
                kFaktura.KommentarsFält = kommentarsFält;
              
                // Hämtar alla fakturarader på kundfakturan
                GetKundFakturaRad(kFaktura, pData);

                // Om valutan inte är svenska kronor, så beräknas totalkostnaden utefter fakturaraderna
                if (valutaKod != "SEK")
                {
                    totalKostnad = 0;
                    foreach (var rad in kFaktura.fakturaRader)
                    {
                        totalKostnad += rad.TotalKostnad;
                    }
                    kFaktura.TotalKostnad = totalKostnad;
                    
                }

                if (fakturaTyp.ToUpper() == "K")
                {
                    kFaktura.Moms *= -1;
                    kFaktura.TotalKostnad *= -1;
                }


                sendData.KundFakturaTillDatabas(kFaktura);

                // Sätter vidare pekaren på nästa instans
                error = AdkNetWrapper.Api.AdkNext(pData);
            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();

        }

        // Metod som hämtar information om raderna i fakturorna 
        private void GetKundFakturaRad(KundFakturaHuvud Faktura, int pData)
        {
            Double NROWS = new Double();
            // Hämtar antalet rader på fakturan
            error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_NROWS, ref NROWS);
            int radReferens = new int(); // Pekar på rader i fakturaraderna likt pData pekar på ett specifikt fakturahuvud

            // Snurra som går så länge deta finns rader i fakturan
            for (int r = 0; r < NROWS; r++)
            {

                // Skapar en instans av en kundfakturarad
                KundFakturaRad enFakturaRad = new KundFakturaRad();
                // Gör så att radreferens pekar mot aktuell fakturarad
                error = AdkNetWrapper.Api.AdkGetData(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_ROWS, r, ref radReferens);
                

                if (error.lRc == AdkNetWrapper.Api.ADKE_OK)
                {
                    String artikelNummer = new String(' ', 16);
                    error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                    enFakturaRad.ArtikelNummer = artikelNummer;

                    //Fakturarader som är avtalsperioder tas inte med, dess data är för sammanställningen irrelevant?
                    if (enFakturaRad.ArtikelNummer != "Avtalsperiod")   
                    {
                        // Autoinkrementerad primärnyckel för databasen
                        kundRadID++;
                        enFakturaRad.KundRadID = kundRadID;

                        Double kvantitet = new Double();
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_QUANTITY1, ref kvantitet);
                        enFakturaRad.LevAntal = kvantitet;

                        Double totalKostnad = new Double();
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_AMOUNT_DOMESTIC_CURRENCY, ref totalKostnad);
                        enFakturaRad.TotalKostnad = totalKostnad;

                        String PROJECT = new String(' ', 6);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PROJECT, ref PROJECT, 6);
                        enFakturaRad.Projekt = PROJECT;

                        String benämning = new string(' ', 60);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_TEXT, ref benämning, 60);
                        enFakturaRad.Benämning = benämning;

                        Double täckningsgrad = new Double();
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_CONTRIBUTION_DEGREE, ref täckningsgrad);
                        enFakturaRad.TäckningsGrad = täckningsgrad;

                        Double täckningsBidrag = new double();
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_CONTRIBUTION_MARGIN, ref täckningsBidrag);
                        enFakturaRad.TäckningsBidrag = täckningsBidrag;

                        String resultatEnhet = new String(' ', 6);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PROFIT_CENTRE, ref resultatEnhet, 6);
                        enFakturaRad.ResultatEnhet = resultatEnhet;

                        // Om krediterade faktura
                        if (Faktura.FakturaTyp.ToUpper() == "K")
                        {
                            enFakturaRad.LevAntal = kvantitet * -1;
                            enFakturaRad.TotalKostnad *= -1;
                            enFakturaRad.TäckningsBidrag *= -1;
                        }

                        

                        Faktura.fakturaRader.Add(enFakturaRad);

                    }

                }


                

            }




        }
    }
}
