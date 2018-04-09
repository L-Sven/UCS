using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adk = AdkNetWrapper;

namespace UCSTest
{
    class VismaData
    {
        List<FakturaHuvud> Kundfakturor;
        Adk.Api.ADKERROR error;

        public VismaData()
        {
            Kundfakturor = new List<FakturaHuvud>();
            GetFakturaHuvudData();
            
        }

        // Kingig metod som hämtar data från fakturahuvud i visma
        private void GetFakturaHuvudData()
        {
            // oklart vilken sökväg som är korrekt, den översta är tagen från sökvägen som presenteras av Visma på min lokala dator
            // Den andra sökvägen är tagen från API:n "Om installationen är gjord på standardsätt", båda fungerar.

            String ftg = @"C:\ProgramData\SPCS\SPCS Administration\Företag\Ovnbol2000";
            String sys = @"C:\ProgramData\SPCS\SPCS Administration\Gemensamma filer";

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
            //PADK_DATA pData;
            int pData;

            // gör pData till en customer-referens
            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_INVOICE_HEAD);

            // Pekar pData mot första raden i customer-tabellen
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
            // while (error.lRc == Adk.Api.ADKE_OK) // Snurra som borde fortgå så länge det finns fakturor
            for (int i = 0; i < 3; i++)
            {

                
                FakturaHuvud kFaktura = new FakturaHuvud();
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

                // Lägger till fakturan till listan med kundfakturor
                Kundfakturor.Add(kFaktura);

                // Anropar metod som hämtar information om de olika raderna i fakturorna
                GetFakturaRad(kFaktura, pData);

                // Sätter vidare pekaren på nästa instans
                error = AdkNetWrapper.Api.AdkNext(pData);
            }
           

            

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();

            int räknare = 1;
            
            // Loopar igenom kundfakturor och gör testutskrifter
            foreach (var faktura in Kundfakturor)
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

            // Ser till så att konsolen inte stänger av sig så fort programmet har körts
            Console.ReadLine();

        }

        // Kingig metod som hämtar informationen om raderna i fakturorna (Alltså vad som beställts)
        private void GetFakturaRad(FakturaHuvud Faktura, int pData)
        {

            Double NROWS = new Double();
            error = AdkNetWrapper.Api.AdkGetDouble(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_NROWS, ref NROWS);
            int radReferens = new int(); // Pekar på rader i fakturaraderna likt pData pekar på ett specifikt fakturahuvud

            for (int r = 0; r < NROWS; r++)
            {
                FakturaRad ettStyckeRad = new FakturaRad();
                error = AdkNetWrapper.Api.AdkGetData(pData, AdkNetWrapper.Api.ADK_OOI_HEAD_ROWS, r, ref radReferens);

                if (error.lRc == AdkNetWrapper.Api.ADKE_OK)
                {
                    String artikelNummer = new String(' ', 16);
                    error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_ARTICLE_NUMBER, ref artikelNummer, 16);
                    ettStyckeRad.ArtikelNummer = artikelNummer;

                    Double kvantitet = new Double();
                    error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_QUANTITY1, ref kvantitet);

                    // Lägg in kontroll för krediterade varor/tjänster för att sätta kvaniteten till negativ
                    ettStyckeRad.LevAntal = kvantitet;

                    if (kvantitet != 0)
                    {
                        String text = new String(' ', 60);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_TEXT, ref text, 60);
                        ettStyckeRad.Benämning = text.Replace(";", "");

                        Double prisPerStyck = new Double();
                        error = AdkNetWrapper.Api.AdkGetDouble(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_PRICE_EACH_CURRENT_CURRENCY, ref prisPerStyck);
                        ettStyckeRad.StyckPris = prisPerStyck;                       
                        
                        ettStyckeRad.TotalKostnad = kvantitet * prisPerStyck;

                        String enhetsTyp = new String(' ', 4);
                        error = AdkNetWrapper.Api.AdkGetStr(radReferens, AdkNetWrapper.Api.ADK_OOI_ROW_UNIT, ref enhetsTyp, 4);
                        ettStyckeRad.EnhetsTyp = enhetsTyp;
                        
                        // Lägg till enhetstyp, kan vara exempelvis styck/timmar beroende på beställd artikel.
                    }




                }
                Faktura.fakturaRader.Add(ettStyckeRad);
            }




        }
    }
}
