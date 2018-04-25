using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Adk = AdkNetWrapper;

namespace UCSTest
{


    class VismaData
    {
        Adk.Api.ADKERROR error;
        readonly SkickaData sendData = new SkickaData();

        
        ErrorLogger logger;

        // Sökvägar för visma administration
        private string ftg;
        private string sys;
        private string _appStartDatum;
        private bool hasDate;
        
        // Följande sökvägar verkar också fungera
        //String sys = @"C:\Documents and Settings\All Users\Application Data\SPCS\SPCS Administration\Gemensamma filer";
        //String ftg = @"C:\Documents and Settings\All Users\Application Data\SPCS\SPCS Administration\Företag\Ovnbol2000";

        int pData;
        private int kData;
        private int lData;
        int antalFakturorUtanNr = 1; // Används för att ge fakturor utan nummer ett fakturanummer
        int levRadID = 0;  //Används för att skapa individuella Identiteter för Leverantörsfafakturaraderna i databasen.
        int kundRadID = 0;  //Används för att skapa individuella Identiteter för Leverantörsfafakturaraderna i databasen.
        int avtalsRadID = 0; // Används för att skapa individuella identiteter för avtalsraderna i fatabasen   
        

        public VismaData(string ftg, string sys, string startDatum)
        {
            DateTime temp;
            this.ftg = ftg;
            this.sys = sys;
            if(DateTime.TryParse(startDatum, out temp))
            {
                this._appStartDatum = startDatum;
                hasDate = true;
            }
            else
            {
                hasDate = false;
            }
            


            logger = new ErrorLogger();
            GetResultatEnhet();
            Console.WriteLine("Resultatenhet klar!");

            // Anropar metod som hämtar data om alla artikelgrupper
            GetArtikelGrupper();
            Console.WriteLine("Artikelgrupper klar!");

            // Anropar metod som hämtar data om alla artiklar 
            GetArtikelData();
            Console.WriteLine("Artikeldata klar!");

            // Anropar metod som hämtar data om alla kundfakturor
            GetKundFakturaHuvudData();
            Console.WriteLine("Kundfakturadata klar!");

            //Anropar metod som hämtar data om alla leverantörsfakturor
            GetLevFakturaHuvudData();
            Console.WriteLine("Leverantörsfakturadata klar!");

            GetAvtal();
            Console.WriteLine("Avtal klar!");
            Console.WriteLine("Tryck en tangent för att avsluta!");

            Console.ReadKey();
            
        }

        private void GetResultatEnhet()
        {
            // Öppnar upp ett företag
            error = Adk.Api.AdkOpen(ref sys, ref ftg);
            logger.ErrorMessage(error);
            

            // Gör pData till en referens av typen Artikelgrupp
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_CODE_OF_PROFIT_CENTRE);

            // Pekar pData mot den första raden i Artikelgrupper
            error = AdkNetWrapper.Api.AdkFirst(pData);
            logger.ErrorMessage(error);

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns artikelgrupper
            {
                Resultatenhet r = new Resultatenhet();

                String resultatEnhetID = new String(' ', 6);
                String resultatEnhetNamn = new String(' ', 20);


                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CODE_OF_PROFIT_CENTRE_CODE, ref resultatEnhetID, 6);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CODE_OF_PROFIT_CENTRE_NAME, ref resultatEnhetNamn, 20);
                logger.ErrorMessage(error);

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
            logger.ErrorMessage(error);

            // Gör pData till en referens av typen Artikelgrupp
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_AGREEMENT_HEAD);

            // Pekar pData mot den första raden i Artikelgrupper
            error = AdkNetWrapper.Api.AdkFirst(pData);
            logger.ErrorMessage(error);

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns artikelgrupper
            {
                Avtal a = new Avtal();

                Double dokumentNummer = new Double();
                int date = new int();
                int endDate = new int();
                String avtalsDatum = new String(' ', 16);
                String kundNummer = new String(' ', 16);
                String startDatum = new String(' ', 16);
                String slutDatum = new String(' ', 16);
                String kommentarsFält = new String(' ', 120);
                String avtalsDatumSlut = new String(' ', 16);

                Double intervall = new Double();
                int periodTemp = new int();
                String periodStart = new string(' ', 8);
                String periodEnd = new String(' ', 8);

                error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_DOCUMENT_DATE1, ref date);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkLongToDate(date, ref avtalsDatum, 16);
                logger.ErrorMessage(error);
                if (!hasDate || DateTime.Parse(avtalsDatum) >= DateTime.Parse(_appStartDatum))
                {
                    error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_DOCUMENT_NUMBER, ref dokumentNummer);
                    logger.ErrorMessage(error);


                    error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_CUSTOMER_NUMBER, ref kundNummer, 16);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_DATE_START, ref date);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkLongToDate(date, ref startDatum, 16);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_DATE_END, ref endDate);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_LOCAL_REMARK, ref kommentarsFält, 120);
                    logger.ErrorMessage(error);

                    error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_PERIOD_START, ref periodTemp);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkLongToDate(periodTemp, ref periodStart, 16);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_PERIOD_END, ref periodTemp);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkLongToDate(periodTemp, ref periodEnd, 16);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_INTERVAL, ref intervall);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_DATE_END, ref date);
                    logger.ErrorMessage(error);

                    if (date == 0)
                    {
                        avtalsDatumSlut = "1111-11-11";
                    }
                    else
                    {
                        error = AdkNetWrapper.Api.AdkLongToDate(date, ref avtalsDatumSlut, 16);
                        logger.ErrorMessage(error);
                    }

                    String datum = new String(' ', 10);
                    string errorText;
                    try
                    {
                        if (kommentarsFält != "" /*|| avtalsDatumSlut == ""*/)
                        {
                            string[] data = kommentarsFält.Split('#');

                            //Vi tar arrayens första element, trimmar bort mellanslaget i början samt tar de första tio tecken.
                            //För detta måste formatet vara yyyy-mm-dd.
                            //För uppsägningstid och förlängningstiden så måste tar vi bort mellanslaget från båda samt tar bara et tecken.
                            datum = data[1].TrimStart(' ').Substring(0, 10);
                            int uppsägningstid = int.Parse(data[2].TrimStart(' ').Substring(0, 1));
                            int förlängningstid = int.Parse(data[3].TrimStart(' ').Substring(0, 1));
                            if (DateTime.TryParse(datum, out var temp))
                            {
                                a.KommenteratSlutDatum = datum;
                            }
                            else
                            {
                                errorText = "Avtal med dokumentnummer " + dokumentNummer + " har inget giltigt slutdatum";
                                logger.ErrorMessage(errorText);
                            }


                            //if (int.Parse(data[2]) > 0 && int.Parse(data[2]) <= 12)
                            if (uppsägningstid > 0 && uppsägningstid <= 12)
                            {
                                a.Uppsägningstid = uppsägningstid;
                                //a.Uppsägningstid = int.Parse(data[2]);
                            }
                            else
                            {
                                errorText = "Avtal med dokumentnummer " + dokumentNummer + " har ingen giltig uppsägningstid";
                                logger.ErrorMessage(errorText);
                            }

                            //if (int.Parse(data[3]) > 0 && int.Parse(data[3]) <= 12)
                            if (förlängningstid > 0 && förlängningstid <= 12)
                            {
                                a.Förlängningstid = förlängningstid;
                                //a.Förlängningstid = int.Parse(data[3]);
                            }
                            else
                            {
                                errorText = "Avtal med dokumentnummer " + dokumentNummer + " har ingen giltig förlängningstid";
                                logger.ErrorMessage(errorText);
                            }


                        }
                        else
                        {

                            errorText = "Avtal med dokumentnummer " + dokumentNummer + " har ingen kommentar";
                            logger.ErrorMessage(errorText);

                        }
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorMessage(ex);
                    }


                    a.DokumentNummer = dokumentNummer;
                    a.AvtalsDatum = avtalsDatum;
                    a.AvtalsDatumSlut = avtalsDatumSlut;
                    a.StartDatum = startDatum;
                    a.KundNummer = kundNummer;

                    a.FakturaIntervall = intervall;
                    a.PeriodStart = periodStart;
                    a.PeriodEnd = periodEnd;

                    GetAvtalRad(a, pData);

                    foreach (var element in a.ListAvtalsRad)
                    {
                        a.TotalKostnad += (double)element.TotalKostnad;
                    }


                    sendData.AvtalTillDatabas(a);
                }
                
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
            logger.ErrorMessage(error);
            int radReferens = new int();

            for (int r = 0; r < NROWS; r++)
            {
                AvtalsRad enAvtalsRad = new AvtalsRad();

                error = AdkNetWrapper.Api.AdkGetData(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_ROWS, r, ref radReferens);
                logger.ErrorMessage(error);
                avtalsRadID++;

                if (error.lRc == AdkNetWrapper.Api.ADKE_OK)
                {

                    String artikelNummer = new String(' ', 16);
                    Double totalKostnad = new Double();

                    error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_AMOUNT_DOMESTIC_CURRENCY, ref totalKostnad);
                    logger.ErrorMessage(error);

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
            logger.ErrorMessage(error);

            // Gör pData till en referens av typen Artikelgrupp
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_CODE_OF_ARTICLE_GROUP);

            // Pekar pData mot den första raden i Artikelgrupper
            error = AdkNetWrapper.Api.AdkFirst(pData);
            logger.ErrorMessage(error);

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns artikelgrupper
            {
                ArtikelGrupp aGrupp = new ArtikelGrupp();

                String aGruppKod = new String(' ', 6);
                String aGruppBenämning = new String(' ', 25);

                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CODE_OF_ARTICLE_GROUP_CODE, ref aGruppKod, 6);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CODE_OF_ARTICLE_GROUP_TEXT, ref aGruppBenämning, 25);
                logger.ErrorMessage(error);

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
            logger.ErrorMessage(error);

            // Gör pData till en referens av typen Artikel
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_ARTICLE);

            // Pekar pData mot den första raden i Artiklar
            error = AdkNetWrapper.Api.AdkFirst(pData);
            logger.ErrorMessage(error);

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
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_ARTICLE_NAME, ref benämning, 30);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_ARTICLE_GROUP, ref artikelGrupp, 6);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_ARTICLE_UNIT_CODE, ref enhetsKod, 4);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_ARTICLE_ESTIMATED_PURCHASE_PRICE, ref inköpsPris);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_ARTICLE_ESTIMATED_CARGO_FEE, ref frakt);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_ARTICLE_ESTIMATED_OTHER, ref ovrigKostnad);
                logger.ErrorMessage(error);

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
            logger.ErrorMessage(error);

            // Gör pData till en referens av typen leverantörsfaktura
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_SUPPLIER_INVOICE_HEAD);

            // Pekar pData mot den första raden i leverantörsfakturahuvud
            error = AdkNetWrapper.Api.AdkFirst(pData);
            logger.ErrorMessage(error);

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns fakturor
                                                 // for (int i = 0; i < 30; i++) // Test som bara kör 30 varv
            {
                // Kontroll om fakturan inte är färdig eller makulerad
                int makulerad = new int();

                error = AdkNetWrapper.Api.AdkGetBool(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_CANCELLED, ref makulerad);
                logger.ErrorMessage(error);

                if (makulerad == 1)
                {
                    // Do nothing
                    Console.WriteLine("Makulerad");
                }
                else
                {
                    // Skapar ny instans av ett lFakturaHuvud
                    LevFakturaHuvud lFakturaHuvud = new LevFakturaHuvud();

                    Double lopNummer = new double();
                    String levNummer = new String(' ', 16);
                    String fakturaNummer = new String(' ', 16);
                    int tmpDatum = new int(); // Temporär datumhållare eftersom data hämtas som 8 siffror
                    String fakturaDatum = new String(' ', 11);
                    String valutaKod = new String(' ', 4);
                    double valutaKurs = 0.00;
                    String fakturaTyp = new String(' ', 12); // fakturatyp F = vanlig faktura, K = kreditfaktura 
                    String projektHuvud = new String(' ', 10);
                    Double moms = new Double();

                    String levNamn = new String(' ', 50);


                    // Hämtar data ur databas och lagrar i de lokala variablerna
                    error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_INVOICE_DATE, ref tmpDatum);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkLongToDate(tmpDatum, ref fakturaDatum, 11);

                    if (!hasDate || DateTime.Parse(fakturaDatum) >= DateTime.Parse(_appStartDatum))
                    {
                        error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_GIVEN_NUMBER, ref lopNummer);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_SUPPLIER_NUMBER, ref levNummer, 16);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_INVOICE_NUMBER, ref fakturaNummer, 16);
                        logger.ErrorMessage(error);

                        // Ger fakturor utan nummer ett eget fakturanummer
                        if (fakturaNummer == "" || fakturaNummer == null)
                        {
                            fakturaNummer = "Faktura-" + antalFakturorUtanNr;
                            antalFakturorUtanNr++;
                        }

                        // Hämtar data ur databas och lagrar i de lokala variablerna

                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_CURRENCY_CODE, ref valutaKod, 4);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_CURRENCY_RATE, ref valutaKurs);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_TYPE_OF_INVOICE, ref fakturaTyp, 12);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_PROJECT, ref projektHuvud, 10);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_VAT_AMOUNT, ref moms);
                        logger.ErrorMessage(error);
                        error = Adk.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_SUPPLIER_NAME, ref levNamn, 50);
                        logger.ErrorMessage(error);

                        // Lägger till data i instansen och lFakturaHuvud
                        lFakturaHuvud.LopNummer = "LF-" + lopNummer;
                        lFakturaHuvud.LevNummer = levNummer;
                        lFakturaHuvud.FakturaNummer = fakturaNummer;
                        lFakturaHuvud.FakturaDatum = fakturaDatum;
                        lFakturaHuvud.ValutaKod = valutaKod;
                        lFakturaHuvud.ValutaKurs = decimal.Parse(valutaKurs.ToString());
                        lFakturaHuvud.FakturaTyp = fakturaTyp;
                        lFakturaHuvud.ProjektHuvud = projektHuvud;
                        lFakturaHuvud.Moms = moms;
                        lFakturaHuvud.LevNamn = levNamn;

                        // Anrop till metod som hämtar alla leverantörsfakturarader i den aktuella fakturan
                        GetLevFakturaRad(lFakturaHuvud, pData);

                        //Totalkostnaden räknas ut genom att slå ihop beloppet i alla rader.
                        if (lFakturaHuvud.FakturaTyp.ToLower() != "k")
                        {
                            lFakturaHuvud.TotalKostnad *= -1;

                            foreach (var rad in lFakturaHuvud.fakturaRader)
                            {
                                rad.TotalKostnad *= -1;
                            }

                        }
                        
                        // Anropr till metod som lägger in data i databasen
                        sendData.LevFakturaTillDatabas(lFakturaHuvud);
  
                    }

                    // Sätter vidare pekaren på nästa instans
                    error = AdkNetWrapper.Api.AdkNext(pData);
                }



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
            logger.ErrorMessage(error);

            int radReferens = new int(); // Pekar på rader i fakturaraderna likt pData pekar på ett specifikt fakturahuvud

            // Snurra som fortgår så länge det finns rader på leverantörsfakturan
            for (int r = 0; r < NROWS; r++)
            {

                // Skapar ny instans av en fakturarad
                LevFakturaRad enFakturaRad = new LevFakturaRad();

                // Sätter radreferensen mot aktuell fakturarad
                error = AdkNetWrapper.Api.AdkGetData(pData, AdkNetWrapper.Api.ADK_SUP_INV_HEAD_ROWS, r, ref radReferens);
                logger.ErrorMessage(error);

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
                    logger.ErrorMessage(error);

                    // om raden presenterar en totalkostnad sparas inte raden utan vi använder värdet för att 
                    // ge leverantörsfakturahuvudet en totalkostnad i svenska kronor 
                    // OBS, kan bli problem om det inte finns någon totalrad i livedatan?!

                    if (information.ToLower() == "total" || information.ToLower() == "moms")
                    {
                        //Do Nothing
                    }

                    // Om det inte är en rad som presenterar totalkostnaden
                    else
                    {
                        // Hämtar data ur databas och lagrar i de lokala variablerna
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_QUANTITY1, ref kvantitet);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_SUPPLIER_ARTICLE_NUMBER, ref levArtikelNummer, 16);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PROJECT, ref projektRad, 10);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_AMOUNT_DOMESTIC_CURRENCY, ref totalKostnad);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PROFIT_CENTRE, ref resultatEnhet, 6);
                        logger.ErrorMessage(error);


                        // Lägger till data i fakturaradinstansen
                        enFakturaRad.Information = information;
                        enFakturaRad.Kvantitet = kvantitet;
                        enFakturaRad.ArtikelNummer = artikelNummer;
                        enFakturaRad.LevArtikelNummer = levArtikelNummer;
                        enFakturaRad.ProjektRad = projektRad;
                        enFakturaRad.TotalKostnad = totalKostnad;
                        enFakturaRad.ResultatEnhet = resultatEnhet;

                        lFaktura.TotalKostnad += totalKostnad;

                        lFaktura.fakturaRader.Add(enFakturaRad);
                    }

                }

            }
        }

        private void GetKunder()
        {
            // Öppnar upp ett företag
            error = Adk.Api.AdkOpen(ref sys, ref ftg);
            logger.ErrorMessage(error);

            // gör pData till en kund-referens
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_CUSTOMER);

            // Pekar pData mot första raden i kundtabellen
            error = AdkNetWrapper.Api.AdkFirst(pData);
            logger.ErrorMessage(error);

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns kunder            
            {
                Kund nyKund = new Kund();
                String kundNr = new String(' ', 16);
                String kundNamn = new String(' ', 50);
                String kundLand = new String(' ', 24);
                String kundStad = new String(' ', 24);
                String kundReferens = new String(' ', 50);

                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CUSTOMER_NUMBER, ref kundNr, 16);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CUSTOMER_NAME, ref kundNamn, 50);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CUSTOMER_COUNTRY, ref kundLand, 24);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CUSTOMER_CITY, ref kundStad, 24);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_CUSTOMER_REFERENCE, ref kundReferens, 50);
                logger.ErrorMessage(error);

                nyKund.KundNummer = kundNr;
                nyKund.KundNamn = kundNamn;
                nyKund.KundLand = kundLand;
                nyKund.KundStad = kundStad;
                nyKund.KundReferens = kundReferens;

                //sendData.KundTillDatabas(nyKund);

                // Sätter vidare pekaren på nästa instans
                error = AdkNetWrapper.Api.AdkNext(pData);
                
            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();
        }

        private void GetLeverantörer()
        {
            // Öppnar upp ett företag
            error = Adk.Api.AdkOpen(ref sys, ref ftg);
            logger.ErrorMessage(error);

            // gör pData till en kund-referens
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_SUPPLIER);

            // Pekar pData mot första raden i kundtabellen
            error = AdkNetWrapper.Api.AdkFirst(pData);
            logger.ErrorMessage(error);

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns kunder            
            {
                Leverantör nyLev = new Leverantör();
                String levNr = new String(' ', 16);
                String levNamn = new String(' ', 50);
                String levLand = new String(' ', 24);
                String levStad = new String(' ', 24);
                String levReferens = new String(' ', 50);

                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUPPLIER_NUMBER, ref levNr, 16);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUPPLIER_NAME, ref levNamn, 50);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUPPLIER_COUNTRY, ref levLand, 24);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUPPLIER_CITY, ref levStad, 24);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_SUPPLIER_REFERENCE, ref levReferens, 50);
                logger.ErrorMessage(error);

                nyLev.LevNummer = levNr;
                nyLev.LevNamn = levNamn;
                nyLev.LevLand = levLand;
                nyLev.LevStad = levStad;
                nyLev.LevReferens = levReferens;

                //sendData.LeverantörTillDatabas(nyLev);

                // Sätter vidare pekaren på nästa instans
                error = AdkNetWrapper.Api.AdkNext(pData);
                
            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();
        }

        // Metod som hämtar data från fakturahuvud i visma
        private void GetKundFakturaHuvudData()
        {

            // Öppnar upp ett företag
            error = Adk.Api.AdkOpen(ref sys, ref ftg);
            logger.ErrorMessage(error);

            // gör pData till en kundfakturahuvud-referens
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_INVOICE_HEAD);

            // Pekar pData mot första raden i kundafaktura-tabellen
            error = AdkNetWrapper.Api.AdkFirst(pData);
            logger.ErrorMessage(error);

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns fakturor            
            {
                // Kontroll om fakturan inte är färdig eller makulerad
                int validFaktura = new int();
                int makulerad = new int();

                error = AdkNetWrapper.Api.AdkGetBool(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DOCUMENT_NOT_DONE, ref validFaktura);
                logger.ErrorMessage(error);
                error = AdkNetWrapper.Api.AdkGetBool(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DOCUMENT_NOT_DONE, ref makulerad);
                logger.ErrorMessage(error);

                // Om det är en ofärdig eller makulerad faktura tar vi inte med den!
                if (validFaktura == 1 || makulerad == 1)
                {
                    // Do nothing
                    Console.WriteLine("Makulerad eller ofärdig faktura");
                }

                else
                {
                    // Skapar ny instans av ett kundafakturahuvud
                    KundFakturaHuvud kFaktura = new KundFakturaHuvud();

                    Double totalKostnad = new Double();
                    String kundNr = new String(' ', 16);
                    int tmpDatum = new int();
                    String fakturaDatum = new String(' ', 11);
                    Double fakturaNr = new Double();
                    String fakturaTyp = new String(' ', 20);
                    String säljare = new String(' ', 25);
                    String valutaKod = new String(' ', 4);
                    Double cargoAmount = new Double();
                    Double dispatchFee = new Double();
                    Double moms = new Double();
                    String kommentarsFält = new String(' ', 120);

                    String kundNamn = new String(' ', 50);
                    String kundLand = new String(' ', 24);
                    String kundStad = new String(' ', 24);
                    String kundReferens = new String(' ', 50);


                    // Hämtar data ur databas och lagrar i de lokala variablerna
                    error = AdkNetWrapper.Api.AdkGetDate(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DOCUMENT_DATE1, ref tmpDatum);
                    logger.ErrorMessage(error);
                    error = AdkNetWrapper.Api.AdkLongToDate(tmpDatum, ref fakturaDatum, 11);
                    logger.ErrorMessage(error);

                    if (!hasDate || DateTime.Parse(fakturaDatum) >= DateTime.Parse(_appStartDatum))
                    {
                        error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DOCUMENT_NUMBER, ref fakturaNr);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_NUMBER, ref kundNr, 16);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_TYPE_OF_INVOICE, ref fakturaTyp, 20);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_OUR_REFERENCE_NAME, ref säljare, 24);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CURRENCY_CODE, ref valutaKod, 4);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CARGO_AMOUNT, ref cargoAmount);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_DISPATCH_FEE, ref dispatchFee);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_VAT_AMOUNT, ref moms);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_TOTAL_AMOUNT, ref totalKostnad);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_REFERENCE_NAME, ref kommentarsFält, 120);
                        logger.ErrorMessage(error);

                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_NAME, ref kundNamn, 50);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_COUNTRY, ref kundLand, 24);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CITY, ref kundStad, 24);
                        logger.ErrorMessage(error);
                        error = AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_CUSTOMER_REFERENCE_NAME, ref kundReferens, 50);
                        logger.ErrorMessage(error);



                        // Lägger info i kundfakturan

                        kFaktura.TotalKostnad = totalKostnad;
                        kFaktura.FakturaDatum = fakturaDatum;

                        String fakturaNummer = new String(' ', 20);
                        fakturaNummer = fakturaNr.ToString();
                        fakturaNummer = "KF-" + fakturaNummer;
                        kFaktura.FakturaNummer = fakturaNummer;
                        kFaktura.KundNummer = kundNr;
                        kFaktura.FakturaTyp = fakturaTyp;
                        kFaktura.Säljare = säljare;
                        kFaktura.Cargo_amount = cargoAmount;
                        kFaktura.Dispatch_fee = dispatchFee;
                        kFaktura.Moms = moms;
                        kFaktura.KommentarsFält = kommentarsFält;
                        kFaktura.KundNamn = kundNamn;
                        kFaktura.KundLand = kundLand;
                        kFaktura.KundStad = kundStad;
                        kFaktura.KundReferens = kundReferens;

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
                    }
                }

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
            logger.ErrorMessage(error);

            int radReferens = new int(); // Pekar på rader i fakturaraderna likt pData pekar på ett specifikt fakturahuvud

            // Snurra som går så länge deta finns rader i fakturan
            for (int r = 0; r < NROWS; r++)
            {

                // Skapar en instans av en kundfakturarad
                KundFakturaRad enFakturaRad = new KundFakturaRad();
                // Gör så att radreferens pekar mot aktuell fakturarad
                error = AdkNetWrapper.Api.AdkGetData(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_ROWS, r, ref radReferens);
                logger.ErrorMessage(error);

                if (error.lRc == AdkNetWrapper.Api.ADKE_OK)
                {
                    String artikelNummer = new String(' ', 16);
                    error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                    logger.ErrorMessage(error);

                    enFakturaRad.ArtikelNummer = artikelNummer;

                    //Fakturarader som är avtalsperioder tas inte med, dess data är för sammanställningen irrelevant?
                    if (enFakturaRad.ArtikelNummer != "Avtalsperiod")   
                    {
                        // Autoinkrementerad primärnyckel för databasen
                        kundRadID++;
                        enFakturaRad.KundRadID = kundRadID;

                        Double kvantitet = new Double();
                        Double totalKostnad = new Double();
                        String projekt = new String(' ', 6);
                        String benämning = new string(' ', 60);
                        Double täckningsgrad = new Double();
                        Double täckningsBidrag = new double();
                        String resultatEnhet = new String(' ', 6);

                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_QUANTITY1, ref kvantitet);
                        logger.ErrorMessage(error);
                        enFakturaRad.LevAntal = kvantitet;

                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_AMOUNT_DOMESTIC_CURRENCY, ref totalKostnad);
                        logger.ErrorMessage(error);
                        enFakturaRad.TotalKostnad = totalKostnad;

                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PROJECT, ref projekt, 6);
                        logger.ErrorMessage(error);
                        enFakturaRad.Projekt = projekt;

                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_TEXT, ref benämning, 60);
                        logger.ErrorMessage(error);
                        enFakturaRad.Benämning = benämning;

                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_CONTRIBUTION_DEGREE, ref täckningsgrad);
                        logger.ErrorMessage(error);
                        enFakturaRad.TäckningsGrad = täckningsgrad;

                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_CONTRIBUTION_MARGIN, ref täckningsBidrag);
                        logger.ErrorMessage(error);
                        enFakturaRad.TäckningsBidrag = täckningsBidrag;

                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PROFIT_CENTRE, ref resultatEnhet, 6);
                        logger.ErrorMessage(error);
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
