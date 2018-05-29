using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Adk = AdkNetWrapper;

namespace UcsAdm
{
    class VismaData
    {
        Adk.Api.ADKERROR error;
        readonly SkickaData sendData = new SkickaData();
        private List<KundFakturaHuvud> kundList = new List<KundFakturaHuvud>();
        private List<LevFakturaHuvud> levList = new List<LevFakturaHuvud>();
        ErrorLogger logger;

        private string ftg;
        private string sys;
        string _appStartDatum;
        bool hasDate;

        int antalFakturorUtanNr = 1; // Används för att ge fakturor utan nummer ett fakturanummer
        int levRadID = 0;  //Används för att skapa individuella Identiteter för Leverantörsfafakturaraderna i databasen.
        int kundRadID = 0;  //Används för att skapa individuella Identiteter för Leverantörsfafakturaraderna i databasen.
        int avtalsRadID = 0; // Används för att skapa individuella identiteter för avtalsraderna i databasen   
        
        public VismaData(string ftg, string sys, string startDatum)
        {
            this.ftg = ftg;
            this.sys = sys;
            try
            {
                DateTime temp;
                // Kontroll för om det finns ett korrekt startdatum för när data ska hämtas från
                if (DateTime.TryParse(startDatum, out temp))
                {
                    this._appStartDatum = startDatum;
                    hasDate = true;
                }

                else
                {
                    hasDate = false;
                }

                logger = new ErrorLogger();

                // Öppnar upp ett företag
                error = Adk.Api.AdkOpen(ref sys, ref ftg);
                if (error.lRc != Adk.Api.ADKE_OK)
                {
                    string errortext = new string(' ', 200);
                    int errtype = (int)Adk.Api.ADK_ERROR_TEXT_TYPE.elRc;
                    Adk.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                    logger.ErrorMessage(errortext);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex + ". Ftg eller Sys sökväg får ej vara null!");
                return;
            }


            // Anropar metd som hämtar data om alla resultatenheter
            GetResultatEnhet();
            Console.WriteLine("Resultatenhet klar!");

            //Tömmer errorloggen.
            sendData.EmptyRowsInErrorlog();


            //// Anropar metod som hämtar data om alla artikelgrupper
            //GetArtikelGrupper();
            //Console.WriteLine("Artikelgrupper klar!");

            //// Anropar metod som hämtar data om alla artiklar 
            //GetArtikelData();
            //Console.WriteLine("Artikeldata klar!");

            // Anropar metod som hämtar data om alla kundfakturor   
            GetKundFakturaHuvudData();
            Console.WriteLine("Kundfakturadata klar!");

            ////Anropar metod som hämtar data om alla leverantörsfakturor
            //GetLevFakturaHuvudData();
            //Console.WriteLine("Leverantörsfakturadata klar!");

            //// Anropar metod som hämtar data om alla avtal
            //GetAvtal();
            //Console.WriteLine("Avtal klar!");

            Console.WriteLine("All information är hämtad!");
            Console.WriteLine("Så här många errors har loggats: " + logger.counter);
            Console.WriteLine("Tryck en tangent för att avsluta!");

            Console.ReadKey();

            //Stänger företaget
            Adk.Api.AdkClose();
        }
        

        private void GetResultatEnhet()
        {
            try
            {
                int pData;
                // Gör pData till en referens av typen Artikelgrupp
                pData = Adk.Api.AdkCreateData(Adk.Api.ADK_DB_CODE_OF_PROFIT_CENTRE);

                // Pekar pData mot den första raden i Artikelgrupper
                error = Adk.Api.AdkFirst(pData);
                logger.ErrorMessage(error);

                while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns artikelgrupper
                {
                    Resultatenhet r = new Resultatenhet();

                    String resultatEnhetID = new String(' ', 6);
                    String resultatEnhetNamn = new String(' ', 20);

                    error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_CODE_OF_PROFIT_CENTRE_CODE,
                        ref resultatEnhetID, 6);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_CODE_OF_PROFIT_CENTRE_NAME,
                        ref resultatEnhetNamn, 20);
                    logger.ErrorMessage(error);

                    r.resultatEnhetID = resultatEnhetID;
                    r.resultatEnhetNamn = resultatEnhetNamn;

                    // Skickar det aktuella objektet av en resultatenhet till databasen
                    sendData.ResultatenhetTillDatabas(r);

                    // Sätter vidare pekaren till nästa artikelgrupp
                    error = Adk.Api.AdkNext(pData);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex.ToString() + " | Fel i resultatenhetshanteringen!");
            }
        }
        
        private void GetAvtal()
        {
            try
            {
                int pData;
                // Gör pData till en referens av typen Artikelgrupp
                pData = Adk.Api.AdkCreateData(Adk.Api.ADK_DB_AGREEMENT_HEAD);

                // Pekar pData mot den första raden i Artikelgrupper
                error = Adk.Api.AdkFirst(pData);
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
                    String kundNamn = new String(' ', 50);
                    String kundLand = new String(' ', 24);
                    String kundStad = new String(' ', 24);
                    String kundReferens = new String(' ', 50);
                    String resultatEnhet = new String(' ', 6);
                    int makulerat = new int();

                    bool slutDatumFinns = false;
                    bool kommenteratSlutDatumFinns = false;

                    error = Adk.Api.AdkGetDate(pData, Adk.Api.ADK_AGREEMENT_HEAD_DOCUMENT_DATE1,
                        ref date);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkLongToDate(date, ref avtalsDatum, 16);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetBool(pData, Adk.Api.ADK_AGREEMENT_HEAD_DOCUMENT_CANCELLED, ref makulerat);
                    logger.ErrorMessage(error);

                    if (makulerat != 1)
                    {
                        // Kontroll för att enbart hämta data efter det startdatum som hämtats från App.config
                        if (!hasDate || DateTime.Parse(avtalsDatum) >= DateTime.Parse(_appStartDatum))
                        {
                            error = Adk.Api.AdkGetDouble(pData,
                                Adk.Api.ADK_AGREEMENT_HEAD_DOCUMENT_NUMBER,
                                ref dokumentNummer);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_AGREEMENT_HEAD_CUSTOMER_NUMBER,
                                ref kundNummer, 16);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetDate(pData, 
                                Adk.Api.ADK_AGREEMENT_HEAD_DATE_START,
                                ref date);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkLongToDate(date,
                                ref startDatum, 16);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetDate(pData, 
                                Adk.Api.ADK_AGREEMENT_HEAD_DATE_END,
                                ref endDate);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_AGREEMENT_HEAD_LOCAL_REMARK,
                                ref kommentarsFält, 120);
                            logger.ErrorMessage(error);

                            error = Adk.Api.AdkGetDate(pData,
                                Adk.Api.ADK_AGREEMENT_HEAD_PERIOD_START,
                                ref periodTemp);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkLongToDate(periodTemp,
                                ref periodStart, 16);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetDate(pData, 
                                Adk.Api.ADK_AGREEMENT_HEAD_PERIOD_END,
                                ref periodTemp);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkLongToDate(periodTemp,
                                ref periodEnd, 16);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetDouble(pData, 
                                Adk.Api.ADK_AGREEMENT_HEAD_INTERVAL,
                                ref intervall);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetDate(pData, 
                                Adk.Api.ADK_AGREEMENT_HEAD_DATE_END,
                                ref date);
                            logger.ErrorMessage(error);

                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_AGREEMENT_HEAD_CUSTOMER_NAME,
                                ref kundNamn, 50);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData, 
                                Adk.Api.ADK_AGREEMENT_HEAD_COUNTRY,
                                ref kundLand, 24);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData, 
                                Adk.Api.ADK_AGREEMENT_HEAD_CITY,
                                ref kundStad, 24);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_AGREEMENT_HEAD_CUSTOMER_REFERENCE_NAME,
                                ref kundReferens, 50);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_AGREEMENT_HEAD_PROFIT_CENTRE,
                                ref resultatEnhet, 6);
                            logger.ErrorMessage(error);

                            // Kontroll om det finns ett angivet slutdatum på avtalet
                            if (date != 0)
                            {
                                error = Adk.Api.AdkLongToDate(date,
                                    ref avtalsDatumSlut, 16);
                                logger.ErrorMessage(error);
                                slutDatumFinns = true;
                            }

                            if (date == 0 || DateTime.Parse(avtalsDatumSlut) > DateTime.Today)
                            {
                                if (date == 0
                                ) //Finns inget slutdatum på avtalet tilldelar vi det ett temporärt värde som gör att den nollställs i Stored proceduren.
                                {
                                    avtalsDatumSlut = "1111-11-11";
                                }

                                String datum = new String(' ', 10);
                                string errorText;

                                try
                                {
                                    // Hantering av kommentarsfältet på avtalet
                                    if (kommentarsFält != "" /*|| avtalsDatumSlut == ""*/)
                                    {
                                        Console.WriteLine(kommentarsFält.Length);
                                        string[] data = kommentarsFält.Split('#');

                                        //Vi tar arrayens första element, trimmar bort mellanslaget i början samt tar de första tio tecken.
                                        //För detta måste formatet vara yyyy-mm-dd.
                                        //För uppsägningstid och förlängningstiden så måste tar vi bort mellanslaget från båda samt tar bara ett tecken.
                                        datum = data[1].TrimStart(' ').Substring(0, 10);
                                        int uppsägningstid = int.Parse(data[2].TrimStart(' ').Substring(0, 1));
                                        int förlängningstid = int.Parse(data[3].TrimStart(' ').Substring(0, 1));
                                        DateTime temp = new DateTime();

                                        if (DateTime.TryParse(datum, out temp))
                                        {
                                            a.KommenteratSlutDatum = datum;
                                            kommenteratSlutDatumFinns = true;
                                        }
                                        else
                                        {
                                            errorText = " Avtal med dokumentnummer " + dokumentNummer +
                                                        " har inget giltigt slutdatum";
                                            logger.ErrorMessage(errorText);
                                        }



                                        if (uppsägningstid > 0 && uppsägningstid <= 12)
                                        {
                                            a.Uppsägningstid = uppsägningstid;

                                        }
                                        else
                                        {
                                            errorText = " Avtal med dokumentnummer " + dokumentNummer +
                                                        " har ingen giltig uppsägningstid";
                                            logger.ErrorMessage(errorText);
                                        }


                                        if (förlängningstid > 0 && förlängningstid <= 12)
                                        {
                                            a.Förlängningstid = förlängningstid;

                                        }
                                        else
                                        {
                                            errorText = " Avtal med dokumentnummer " + dokumentNummer +
                                                        " har ingen giltig förlängningstid";
                                            logger.ErrorMessage(errorText);
                                        }


                                    }
                                    else
                                    {
                                        errorText = " Avtal med dokumentnummer " + dokumentNummer +
                                                    " har ingen kommentar";
                                        logger.ErrorMessage(errorText);

                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.ErrorMessage(
                                        ex + " Avtalnummer: " + dokumentNummer + ". Felaktig kommentarsfält.");

                                }

                                a.DokumentNummer = dokumentNummer;
                                a.AvtalsDatum = avtalsDatum;
                                a.AvtalsDatumSlut = avtalsDatumSlut;
                                a.StartDatum = startDatum;
                                a.KundNummer = kundNummer;
                                a.ResultatEnhet = resultatEnhet;
                                a.Kommentarsfält = kommentarsFält;

                                a.FakturaIntervall = intervall;
                                a.PeriodStart = periodStart;
                                a.PeriodEnd = periodEnd;

                                a.KundNamn = kundNamn;
                                a.KundStad = kundStad;
                                a.KundLand = kundLand;
                                a.KundReferens = kundReferens;

                                //För att kunna göra en prognos på 1 år skapar vi alla datum vi behöver direkt här
                                List<string> avtalPrognosList = new List<string>();

                                //Vi delar intervallen med antal månadern på ett år, dvs att är intervall 3 månader
                                //så behöver vi 12/3 = 4 prognosdatum. Så vi lägger dem till den ursprungliga periodStart datumet och addera hela tiden.
                                for (int i = 0; i < 12 / intervall; i++)
                                {
                                    // Första gången det i snurran läggs nästa periodstart in om datumet förekommer efter dagens datum
                                    // Annars kommer man inte räkna med en prognos på nästa periodstart, utan direkt öka den med en intervall-enhet
                                    if (i == 0 && DateTime.Parse(periodStart) > DateTime.Today)
                                    {
                                        avtalPrognosList.Add(periodStart);
                                    }

                                    periodStart = DateTime.Parse(periodStart).AddMonths((int) intervall).ToShortDateString();

                                    // Kontroll som inte lägger till prognosperiod efter slutdatum för avtalet
                                    if (slutDatumFinns || kommenteratSlutDatumFinns)
                                    {
                                        // Kollar om nästa period ligger efter avtalsslutet
                                        if (slutDatumFinns && DateTime.Parse(periodStart) > DateTime.Parse(avtalsDatumSlut))
                                        {
                                            break;
                                        }
                                        // Kollar om nästa period ligger efter det avtalsslutet i kommentarsfältet
                                        else if (kommenteratSlutDatumFinns && DateTime.Parse(periodStart) > DateTime.Parse(a.KommenteratSlutDatum))
                                        {
                                            break;
                                        }

                                        // Om nästa period inte ligger utanför något slutdatum adderas datumet till prognoslistan
                                        else
                                        {
                                            avtalPrognosList.Add(periodStart);
                                        }
                                    }

                                    // Om det inte finns något slutdatum för avtalet adderas datumet till prognoslistan
                                    else
                                    {
                                        avtalPrognosList.Add(periodStart);
                                    }
                                }

                                // Hämtar avtalsraderna på avtalet
                                GetAvtalRad(a, pData);

                                foreach (var element in a.ListAvtalsRad)
                                {
                                    a.BeloppExklMoms += (double) element.BeloppExklMoms;
                                }

                                // Skickar avtalet till databasen
                                sendData.AvtalTillDatabas(a, avtalPrognosList);
                                avtalPrognosList.Clear();
                            }
                        }
                    }
                    // Sätter vidare pekaren till nästa artikelgrupp
                    error = Adk.Api.AdkNext(pData);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex + " | Fel i Avtalhanterng!");
            }
        }

        private void GetAvtalRad(Avtal avtal, int pData)
        {
            Double NROWS = new Double();

            error = Adk.Api.AdkGetDouble(pData, Adk.Api.ADK_AGREEMENT_HEAD_NROWS, ref NROWS);
            logger.ErrorMessage(error);
            int radReferens = new int();

            for (int r = 0; r < NROWS; r++)
            {
                AvtalsRad enAvtalsRad = new AvtalsRad();

                error = Adk.Api.AdkGetData(pData, Adk.Api.ADK_AGREEMENT_HEAD_ROWS, r, ref radReferens);
                logger.ErrorMessage(error);
                avtalsRadID++;

                if (error.lRc == Adk.Api.ADKE_OK)
                {
                    String artikelNummer = new String(' ', 16);
                    Double totalKostnad = new Double();
                    String benämning = new String(' ', 60);

                    error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetDouble(radReferens, Adk.Api.ADK_OOI_ROW_AMOUNT_DOMESTIC_CURRENCY, ref totalKostnad);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_TEXT, ref benämning, 60);
                    logger.ErrorMessage(error);

                    enAvtalsRad.Benämning = benämning;
                    enAvtalsRad.ArtikelNummer = artikelNummer;
                    enAvtalsRad.RadId = avtalsRadID;
                    enAvtalsRad.BeloppExklMoms = totalKostnad;

                    if ((artikelNummer != string.Empty || artikelNummer != "") && totalKostnad != 0.00)
                    {
                        // Lägger till avtalsraden i listan över det aktuella avtalets avtalsrader
                        avtal.ListAvtalsRad.Add(enAvtalsRad);
                    }
                }
            }
        }

        private void GetArtikelGrupper()
        {
            try
            {
                int pData;
                // Gör pData till en referens av typen Artikelgrupp
                pData = Adk.Api.AdkCreateData(Adk.Api.ADK_DB_CODE_OF_ARTICLE_GROUP);

                // Pekar pData mot den första raden i Artikelgrupper
                error = Adk.Api.AdkFirst(pData);
                logger.ErrorMessage(error);

                while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns artikelgrupper
                {
                    ArtikelGrupp aGrupp = new ArtikelGrupp();

                    String aGruppKod = new String(' ', 6);
                    String aGruppBenämning = new String(' ', 25);

                    error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_CODE_OF_ARTICLE_GROUP_CODE,
                        ref aGruppKod, 6);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_CODE_OF_ARTICLE_GROUP_TEXT,
                        ref aGruppBenämning, 25);
                    logger.ErrorMessage(error);

                    aGrupp.ArtikelGruppKod = aGruppKod;
                    aGrupp.Benämning = aGruppBenämning;

                    sendData.ArtikelGruppTillDatabas(aGrupp);

                    // Sätter vidare pekaren till nästa artikelgrupp
                    error = Adk.Api.AdkNext(pData);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex.ToString() + " | Fel i GetArtikelgrupper!");
            }
        }

        // Metod som hämtar artikeldata
        private void GetArtikelData()
        {
            try
            {
                int pData;
                // Gör pData till en referens av typen Artikel
                pData = Adk.Api.AdkCreateData(Adk.Api.ADK_DB_ARTICLE);

                // Pekar pData mot den första raden i Artiklar
                error = Adk.Api.AdkFirst(pData);
                logger.ErrorMessage(error);

                while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns artiklar
                {
                    Artikel artikel = new Artikel();

                    String artikelNummer = new String(' ', 16);
                    String benämning = new string(' ', 30);
                    String artikelGrupp = new String(' ', 6);
                    String enhetsKod = new string(' ', 4);
                    Double inköpsPris = new Double();
                    Double frakt = new Double();
                    Double ovrigKostnad = new Double();

                    // Hämtar data från databasen och lagrar i de lokala variablerna
                    error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_ARTICLE_NUMBER, ref artikelNummer,
                        16);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_ARTICLE_NAME, ref benämning, 30);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_ARTICLE_GROUP, ref artikelGrupp,
                        6);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_ARTICLE_UNIT_CODE, ref enhetsKod,
                        4);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetDouble(pData,
                        Adk.Api.ADK_ARTICLE_ESTIMATED_PURCHASE_PRICE, ref inköpsPris);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetDouble(pData, Adk.Api.ADK_ARTICLE_ESTIMATED_CARGO_FEE,
                        ref frakt);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetDouble(pData, Adk.Api.ADK_ARTICLE_ESTIMATED_OTHER,
                        ref ovrigKostnad);
                    logger.ErrorMessage(error);

                    // Lägger till data den aktuella artikelinstansen
                    artikel.ArtikelNummer = artikelNummer;
                    artikel.ArtikelGrupp = artikelGrupp;
                    artikel.Benämning = benämning;
                    artikel.EnhetsKod = enhetsKod;
                    artikel.InköpsPris = inköpsPris;
                    artikel.Frakt = frakt;
                    artikel.OvrigKostnad = ovrigKostnad;

                    // Skickar data till sendData som i sin tur lägger in artikeln i databasen
                    sendData.ArtikelTillDatabas(artikel);

                    // Sätter vidare pekaren till nästa artikel
                    error = Adk.Api.AdkNext(pData);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex.ToString() + " | Fel i Artikel hanteringen");
            }
        }

        // Metod som hämtar data om ett leverantörsfakturahuvud
        private void GetLevFakturaHuvudData()
        {
            try
            {
                int pData;
                // Gör pData till en referens av typen leverantörsfaktura
                pData = Adk.Api.AdkCreateData(Adk.Api.ADK_DB_SUPPLIER_INVOICE_HEAD);

                // Pekar pData mot den första raden i leverantörsfakturahuvud
                error = Adk.Api.AdkFirst(pData);
                logger.ErrorMessage(error);

                while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns fakturor
                {
                    // Kontroll om fakturan är makulerad
                    int makulerad = new int();

                    error = Adk.Api.AdkGetBool(pData, Adk.Api.ADK_SUP_INV_HEAD_CANCELLED,
                        ref makulerad);
                    logger.ErrorMessage(error);

                    if (makulerad == 1)
                    {
                        // Do nothing
                    }
                    else
                    {
                        LevFakturaHuvud lFakturaHuvud = new LevFakturaHuvud();

                        Double lopNummer = new double();
                        String levNummer = new String(' ', 16);
                        String fakturaNummer = new String(' ', 16);
                        int tmpDatum = new int(); // Temporär datumhållare 
                        String fakturaDatum = new String(' ', 11);
                        String valutaKod = new String(' ', 4);
                        double valutaKurs = 0.00;
                        String fakturaTyp = new String(' ', 12); // fakturatyp F = vanlig faktura, K = kreditfaktura 
                        String projektHuvud = new String(' ', 10);
                        Double moms = new Double();

                        String levNamn = new String(' ', 50);

                        error = Adk.Api.AdkGetDate(pData, Adk.Api.ADK_SUP_INV_HEAD_INVOICE_DATE,
                            ref tmpDatum);
                        logger.ErrorMessage(error);
                        error = Adk.Api.AdkLongToDate(tmpDatum, ref fakturaDatum, 11);
                        logger.ErrorMessage(error);

                        // Kontrollerar om fakturan är inom korrekt datum
                        if (!hasDate || DateTime.Parse(fakturaDatum) >= DateTime.Parse(_appStartDatum))
                        {
                            error = Adk.Api.AdkGetDouble(pData,
                                Adk.Api.ADK_SUP_INV_HEAD_GIVEN_NUMBER, ref lopNummer);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_SUP_INV_HEAD_SUPPLIER_NUMBER, ref levNummer, 16);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_SUP_INV_HEAD_INVOICE_NUMBER, ref fakturaNummer, 16);
                            logger.ErrorMessage(error);

                            // Ger fakturor utan nummer ett eget fakturanummer
                            if (fakturaNummer == "" || fakturaNummer == null)
                            {
                                fakturaNummer = "Faktura-" + antalFakturorUtanNr;
                                antalFakturorUtanNr++;
                            }

                            // Hämtar data ur databas och lagrar i de lokala variablerna
                            error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_SUP_INV_HEAD_CURRENCY_CODE,
                                ref valutaKod, 4);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetDouble(pData,
                                Adk.Api.ADK_SUP_INV_HEAD_CURRENCY_RATE, ref valutaKurs);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_SUP_INV_HEAD_TYPE_OF_INVOICE, ref fakturaTyp, 12);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_SUP_INV_HEAD_PROJECT,
                                ref projektHuvud, 10);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetDouble(pData, Adk.Api.ADK_SUP_INV_HEAD_VAT_AMOUNT,
                                ref moms);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_SUP_INV_HEAD_SUPPLIER_NAME,
                                ref levNamn, 50);
                            logger.ErrorMessage(error);

                            // Ger leverantörsfakturan prefixet "LF-" för att undvika samma fakturanummer som kundfakturorna
                            try
                            {
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
                            }
                            catch (Exception ex)
                            {
                                logger.ErrorMessage(ex);
                            }

                            // Anrop till metod som hämtar alla leverantörsfakturarader i den aktuella fakturan
                            GetLevFakturaRad(lFakturaHuvud, pData);

                            //Totalkostnaden görs negativ vid krediterad faktura
                            if (lFakturaHuvud.FakturaTyp.ToLower() != "k")
                            {
                                lFakturaHuvud.TotalKostnad *= -1;

                                foreach (var rad in lFakturaHuvud.fakturaRader)
                                {
                                    rad.TotalKostnad *= -1;
                                }
                            }

                            levList.Add(lFakturaHuvud);
                            // Anropr till metod som lägger in data i databasen
                        }
                    }
                    // Sätter vidare pekaren på nästa instans
                    error = Adk.Api.AdkNext(pData);
                }

            sendData.LevFakturaTillDatabas(levList);
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex + " | Fel i Leverantörsfakturahanteringen!");
            }
        }

        // Metod som hämtar rader från en leverantörsfaktura
        private void GetLevFakturaRad(LevFakturaHuvud lFaktura, int pData)
        {
            Double NROWS = new Double();

            // Hämtar antalet rader som finns på leverantörsfakturan
            error = Adk.Api.AdkGetDouble(pData, Adk.Api.ADK_SUP_INV_HEAD_NROWS, ref NROWS);
            logger.ErrorMessage(error);

            int radReferens = new int(); // Pekar på rader i fakturaraderna likt pData pekar på ett specifikt fakturahuvud

            // Snurra som fortgår så länge det finns rader på leverantörsfakturan
            for (int r = 0; r < NROWS; r++)
            {
                // Skapar ny instans av en fakturarad
                LevFakturaRad enFakturaRad = new LevFakturaRad();

                // Sätter radreferensen mot aktuell fakturarad
                error = Adk.Api.AdkGetData(pData, Adk.Api.ADK_SUP_INV_HEAD_ROWS, r, ref radReferens);
                logger.ErrorMessage(error);

                // Autoinkrementad primärnyckel för leverantörsfakturaraden i databasen
                levRadID++;
                enFakturaRad.LevRadID = levRadID;

                if (error.lRc == Adk.Api.ADKE_OK)
                {
                    String information = new String(' ', 60);
                    Double kvantitet = new Double();
                    String artikelNummer = new String(' ', 16); // internt artikelnummer
                    String levArtikelNummer = new String(' ', 16); // Leverantörens artikelnummer
                    String projektRad = new String(' ', 10);
                    Double totalKostnad = new Double();
                    String resultatEnhet = new string(' ', 6);

                    error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_TEXT, ref information, 60);
                    logger.ErrorMessage(error);

                    // om raden presenterar en totalkostnad sparas inte raden utan vi använder värdet för att 
                    // ge leverantörsfakturahuvudet en totalkostnad i svenska kronor 
                    // OBS, kan bli problem om det inte finns någon totalrad i livedatan?!

                    if (information.ToLower() == "total" || information.ToLower() == "moms")
                    {
                        //Do Nothing
                    }

                    // Om det inte är en rad som presenterar totalkostnaden eller moms
                    else
                    {
                        // Hämtar data ur databas och lagrar i de lokala variablerna
                        error = Adk.Api.AdkGetDouble(radReferens, Adk.Api.ADK_OOI_ROW_QUANTITY1, ref kvantitet);
                        logger.ErrorMessage(error);
                        error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_SUPPLIER_ARTICLE_NUMBER, ref levArtikelNummer, 16);
                        logger.ErrorMessage(error);
                        error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                        logger.ErrorMessage(error);
                        error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_PROJECT, ref projektRad, 10);
                        logger.ErrorMessage(error);
                        error = Adk.Api.AdkGetDouble(radReferens, Adk.Api.ADK_OOI_ROW_AMOUNT_DOMESTIC_CURRENCY, ref totalKostnad);
                        logger.ErrorMessage(error);
                        error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_PROFIT_CENTRE, ref resultatEnhet, 6);
                        logger.ErrorMessage(error);

                        // Lägger till data i fakturaradinstansen
                        enFakturaRad.Information = information;
                        enFakturaRad.Kvantitet = kvantitet;
                        enFakturaRad.ArtikelNummer = artikelNummer;
                        enFakturaRad.LevArtikelNummer = levArtikelNummer;
                        enFakturaRad.ProjektRad = projektRad;
                        enFakturaRad.TotalKostnad = totalKostnad;
                        enFakturaRad.ResultatEnhet = resultatEnhet;

                        // Ökar totalkostnaden på fakturan med kostnaden för aktuell rad
                        lFaktura.TotalKostnad += totalKostnad;

                        // Lägger till raden i den aktuella leverantörsfakturan lista över rader
                        lFaktura.fakturaRader.Add(enFakturaRad);
                    }
                }
            }
        }

        #region ====== INAKTIV KOD ======

        // Används inte just nu eftersom vi hämtar kunder direkt från fakturorna istället för kunddatabasen.
        // Detta för att det kan finnas kunder utan fakturor, nu kommer enbart kunder med som har fakturor i databasen.
        /*
        private void GetKunder()
        {
            // Öppnar upp ett företag
            error = Adk.Api.AdkOpen(ref sys, ref ftg);
            logger.ErrorMessage(error);

            // gör pData till en kund-referens
            pData = Adk.Api.AdkCreateData(Adk.Api.ADK_DB_CUSTOMER);

            // Pekar pData mot första raden i kundtabellen
            error = Adk.Api.AdkFirst(pData);
            logger.ErrorMessage(error);

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns kunder            
            {
                Kund nyKund = new Kund();
                String kundNr = new String(' ', 16);
                String kundNamn = new String(' ', 50);
                String kundLand = new String(' ', 24);
                String kundStad = new String(' ', 24);
                String kundReferens = new String(' ', 50);

                error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_CUSTOMER_NUMBER, ref kundNr, 16);
                logger.ErrorMessage(error);
                error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_CUSTOMER_NAME, ref kundNamn, 50);
                logger.ErrorMessage(error);
                error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_CUSTOMER_COUNTRY, ref kundLand, 24);
                logger.ErrorMessage(error);
                error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_CUSTOMER_CITY, ref kundStad, 24);
                logger.ErrorMessage(error);
                error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_CUSTOMER_REFERENCE, ref kundReferens, 50);
                logger.ErrorMessage(error);

                nyKund.KundNummer = kundNr;
                nyKund.KundNamn = kundNamn;
                nyKund.KundLand = kundLand;
                nyKund.KundStad = kundStad;
                nyKund.KundReferens = kundReferens;

                //sendData.KundTillDatabas(nyKund);

                // Sätter vidare pekaren på nästa instans
                error = Adk.Api.AdkNext(pData);
                
            }

            // Stänger företaget
            Adk.Api.AdkClose();
        }
        */

        // Används inte eftersom vi hämtar leverantörerna direkt från leverantörsfakturorna istället för databasen med leverantörer.
        // Detta för att inte hämta leverantörer som inte finns med på några leverantörsfakturor
        /*
        private void GetLeverantörer()
        {
            // Öppnar upp ett företag
            error = Adk.Api.AdkOpen(ref sys, ref ftg);
            logger.ErrorMessage(error);

            // gör pData till en kund-referens
            pData = Adk.Api.AdkCreateData(Adk.Api.ADK_DB_SUPPLIER);

            // Pekar pData mot första raden i kundtabellen
            error = Adk.Api.AdkFirst(pData);
            logger.ErrorMessage(error);

            while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns kunder            
            {
                Leverantör nyLev = new Leverantör();
                String levNr = new String(' ', 16);
                String levNamn = new String(' ', 50);
                String levLand = new String(' ', 24);
                String levStad = new String(' ', 24);
                String levReferens = new String(' ', 50);

                error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_SUPPLIER_NUMBER, ref levNr, 16);
                logger.ErrorMessage(error);
                error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_SUPPLIER_NAME, ref levNamn, 50);
                logger.ErrorMessage(error);
                error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_SUPPLIER_COUNTRY, ref levLand, 24);
                logger.ErrorMessage(error);
                error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_SUPPLIER_CITY, ref levStad, 24);
                logger.ErrorMessage(error);
                error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_SUPPLIER_REFERENCE, ref levReferens, 50);
                logger.ErrorMessage(error);

                nyLev.LevNummer = levNr;
                nyLev.LevNamn = levNamn;
                nyLev.LevLand = levLand;
                nyLev.LevStad = levStad;
                nyLev.LevReferens = levReferens;

                //sendData.LeverantörTillDatabas(nyLev);

                // Sätter vidare pekaren på nästa instans
                error = Adk.Api.AdkNext(pData);
                
            }

            // Stänger företaget
            Adk.Api.AdkClose();
        }
        */

        #endregion

        // Metod som hämtar data från kundfakturahuvuden i visma
        private void GetKundFakturaHuvudData()
        {
            try
            {
                int pData;
                // gör pData till en kundfakturahuvud-referens
                pData = Adk.Api.AdkCreateData(Adk.Api.ADK_DB_INVOICE_HEAD);

                // Pekar pData mot första raden i kundafaktura-tabellen
                error = Adk.Api.AdkFirst(pData);
                logger.ErrorMessage(error);

                while (error.lRc == Adk.Api.ADKE_OK) // Snurra som fortgår så länge det finns fakturor            
                {
                    // Kontroll om fakturan inte är färdig eller makulerad
                    int InvalidFaktura = new int();
                    int makulerad = new int();
                    int utskriven = new int();


                    error = Adk.Api.AdkGetBool(pData, Adk.Api.ADK_OOI_HEAD_DOCUMENT_NOT_DONE,
                        ref InvalidFaktura);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetBool(pData, Adk.Api.ADK_OOI_HEAD_DOCUMENT_CANCELLED,
                        ref makulerad);
                    logger.ErrorMessage(error);
                    error = Adk.Api.AdkGetBool(pData, Adk.Api.ADK_OOI_HEAD_DOCUMENT_PRINTED,
                        ref utskriven);
                    logger.ErrorMessage(error);

                    // Om det är en ofärdig eller makulerad faktura tar vi inte med den!
                    if (utskriven == 0 || InvalidFaktura == 1 || makulerad == 1)
                    {
                        // Do nothing
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
                        String avtalsNummer = new String(' ', 15);

                        String kundNamn = new String(' ', 50);
                        String kundLand = new String(' ', 24);
                        String kundStad = new String(' ', 24);
                        String kundReferens = new String(' ', 50);

                        // Hämtar data ur databas och lagrar i de lokala variablerna
                        error = Adk.Api.AdkGetDate(pData, Adk.Api.ADK_OOI_HEAD_DOCUMENT_DATE1,
                            ref tmpDatum);
                        logger.ErrorMessage(error);
                        error = Adk.Api.AdkLongToDate(tmpDatum, ref fakturaDatum, 11);
                        logger.ErrorMessage(error);

                        // Kontroll om fakturna är inom korrekt datum från vad som angivits i App.config
                        if (!hasDate || DateTime.Parse(fakturaDatum) >= DateTime.Parse(_appStartDatum))
                        {
                            error = Adk.Api.AdkGetDouble(pData,
                                Adk.Api.ADK_OOI_HEAD_DOCUMENT_NUMBER, ref fakturaNr);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_OOI_HEAD_CUSTOMER_NUMBER,
                                ref kundNr, 16);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_OOI_HEAD_TYPE_OF_INVOICE,
                                ref fakturaTyp, 20);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_OOI_HEAD_OUR_REFERENCE_NAME, ref säljare, 24);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_OOI_HEAD_CURRENCY_CODE,
                                ref valutaKod, 4);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetDouble(pData, Adk.Api.ADK_OOI_HEAD_CARGO_AMOUNT,
                                ref cargoAmount);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetDouble(pData, Adk.Api.ADK_OOI_HEAD_DISPATCH_FEE,
                                ref dispatchFee);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetDouble(pData, Adk.Api.ADK_OOI_HEAD_VAT_AMOUNT,
                                ref moms);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_OOI_HEAD_CUSTOMER_REFERENCE_NAME, ref kommentarsFält, 120);
                            logger.ErrorMessage(error);

                            error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_OOI_HEAD_CUSTOMER_NAME,
                                ref kundNamn, 50);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_OOI_HEAD_COUNTRY,
                                ref kundLand, 24);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData, Adk.Api.ADK_OOI_HEAD_CITY,
                                ref kundStad, 24);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_OOI_HEAD_CUSTOMER_REFERENCE_NAME, ref kundReferens, 50);
                            logger.ErrorMessage(error);
                            error = Adk.Api.AdkGetStr(pData,
                                Adk.Api.ADK_OOI_HEAD_CONTRACTNR, ref avtalsNummer, 15);
                            logger.ErrorMessage(error);

                            kFaktura.BeloppExklMoms = totalKostnad;
                            kFaktura.FakturaDatum = fakturaDatum;

                            // Ger alla kundfakturor prefixet "KF-" för att de inte ska kunna ha samma nummer som leverantörsfakturorna
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
                            kFaktura.AvtalsNummer = avtalsNummer;

                            // Hämtar alla fakturarader på kundfakturan
                            GetKundFakturaRad(kFaktura, pData);

                            // Om valutan inte är svenska kronor, så beräknas totalkostnaden utefter fakturaraderna

                            totalKostnad = 0;
                            foreach (var rad in kFaktura.fakturaRader)
                            {
                                totalKostnad += rad.BeloppExklMoms;
                            }

                            kFaktura.BeloppExklMoms = totalKostnad;


                            // Om det är en krediterad faktura görs värdena negativa
                            if (fakturaTyp.ToUpper() == "K")
                            {
                                kFaktura.Moms *= -1;
                                kFaktura.BeloppExklMoms *= -1;
                            }

                            // Skickar kundfakturan till sendData som i sin tur lägger itll den i databasen
                            kundList.Add(kFaktura);
                        }
                    }

                    // Sätter vidare pekaren på nästa instans
                    error = Adk.Api.AdkNext(pData);
                }

                sendData.KundFakturaTillDatabas(kundList);
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex + " | Fel i Kundfakturahanteringen!");
            }
        }

        // Metod som hämtar information om raderna i fakturorna 
        private void GetKundFakturaRad(KundFakturaHuvud kFaktura, int pData)
        {
            Double NROWS = new Double();
            // Hämtar antalet rader på fakturan
            error = Adk.Api.AdkGetDouble(pData, Adk.Api.ADK_OOI_HEAD_NROWS, ref NROWS);
            logger.ErrorMessage(error);

            int radReferens = new int(); // Pekar på rader i fakturaraderna likt pData pekar på ett specifikt fakturahuvud

            // Snurra som går så länge deta finns rader i fakturan
            for (int r = 0; r < NROWS; r++)
            {
                KundFakturaRad enFakturaRad = new KundFakturaRad();
                // Gör så att radreferens pekar mot aktuell fakturarad
                error = Adk.Api.AdkGetData(pData, Adk.Api.ADK_OOI_HEAD_ROWS, r, ref radReferens);
                logger.ErrorMessage(error);

                if (error.lRc == Adk.Api.ADKE_OK)
                {
                    String artikelNummer = new String(' ', 16);
                    error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                    logger.ErrorMessage(error);

                    enFakturaRad.ArtikelNummer = artikelNummer;

                    //Fakturarader som är avtalsperioder tas inte med, dess data hämtas i samband med när avtalen hämtas
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

                        error = Adk.Api.AdkGetDouble(radReferens, Adk.Api.ADK_OOI_ROW_QUANTITY1, ref kvantitet);
                        logger.ErrorMessage(error);
                        enFakturaRad.LevAntal = kvantitet;

                        error = Adk.Api.AdkGetDouble(radReferens, Adk.Api.ADK_OOI_ROW_AMOUNT_DOMESTIC_CURRENCY, ref totalKostnad);
                        logger.ErrorMessage(error);
                        enFakturaRad.BeloppExklMoms = totalKostnad;

                        error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_PROJECT, ref projekt, 6);
                        logger.ErrorMessage(error);
                        enFakturaRad.Projekt = projekt;

                        error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_TEXT, ref benämning, 60);
                        logger.ErrorMessage(error);
                        enFakturaRad.Benämning = benämning;

                        error = Adk.Api.AdkGetDouble(radReferens, Adk.Api.ADK_OOI_ROW_CONTRIBUTION_DEGREE, ref täckningsgrad);
                        logger.ErrorMessage(error);
                        enFakturaRad.TäckningsGrad = täckningsgrad;

                        error = Adk.Api.AdkGetDouble(radReferens, Adk.Api.ADK_OOI_ROW_CONTRIBUTION_MARGIN, ref täckningsBidrag);
                        logger.ErrorMessage(error);
                        enFakturaRad.TäckningsBidrag = täckningsBidrag;

                        error = Adk.Api.AdkGetStr(radReferens, Adk.Api.ADK_OOI_ROW_PROFIT_CENTRE, ref resultatEnhet, 6);
                        logger.ErrorMessage(error);
                        enFakturaRad.ResultatEnhet = resultatEnhet;

                        // Om krediterade faktura
                        if (kFaktura.FakturaTyp.ToUpper() == "K")
                        {
                            enFakturaRad.LevAntal = kvantitet * -1;
                            enFakturaRad.BeloppExklMoms *= -1;
                            enFakturaRad.TäckningsBidrag *= -1;
                        }
                        
                        kFaktura.fakturaRader.Add(enFakturaRad);
                    }
                } 
            }
        }
    }
}
