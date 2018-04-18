using System;
using adk = AdkNetWrapper;

namespace DatabasNameReplacer
{
    public class VismaData
    {
        
        private adk.Api.ADKERROR error;
        String ftg = @"C:\ProgramData\SPCS\SPCS Administration\Företag\Ovnbol2000";
        String sys = @"C:\ProgramData\SPCS\SPCS Administration\Gemensamma filer";

        private int pData;
        private int sData;

        public VismaData()
        {
           SkapaDragi();
           UpdateSellerReference();
        }

        public void SkapaDragi()
        {
            //Öppna företag
            error = adk.Api.AdkOpen(ref sys, ref ftg);

            if (error.lRc != adk.Api.ADKE_OK)
            {
                string errortext = new string(' ', 200);
                int errtype = (int)adk.Api.ADK_ERROR_TEXT_TYPE.elRc;
                adk.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_CODE_OF_SELLER);
            error = AdkNetWrapper.Api.AdkFirst(pData);

            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            String nameAdd = "Dragi Spasojevic";
            String sellerCodeAdd = "DS";
            AdkNetWrapper.Api.AdkSetStr(pData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_SIGN, ref sellerCodeAdd);
            AdkNetWrapper.Api.AdkSetStr(pData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_NAME, ref nameAdd);
            

            adk.Api.AdkAdd(pData);
            AdkNetWrapper.Api.AdkClose();
        }
        private void UpdateSellerReference()
        {
            //Öppna företag
            error = adk.Api.AdkOpen(ref sys, ref ftg);

            if (error.lRc != adk.Api.ADKE_OK)
            {
                string errortext = new string(' ', 200);
                int errtype = (int) adk.Api.ADK_ERROR_TEXT_TYPE.elRc;
                adk.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            sData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_CODE_OF_SELLER);
            error = AdkNetWrapper.Api.AdkFirst(sData);

            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            // Snurra som gör att sdata refererar till dragi!
            while (error.lRc == adk.Api.ADKE_OK)
            {
                String namn = new string(' ', 25);
                AdkNetWrapper.Api.AdkGetStr(sData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_NAME, ref namn, 25);
                if (namn == "Dragi Spasojevic")
                {
                    break;
                }

                error = AdkNetWrapper.Api.AdkNext(sData);
            }

            // Sparar dragis information
            string name = new string(' ', 25);
            AdkNetWrapper.Api.AdkGetStr(sData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_NAME, ref name, 25);
            string sign = new string(' ', 10);
            AdkNetWrapper.Api.AdkGetStr(sData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_SIGN, ref sign, 10);


            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_AGREEMENT_HEAD);
            error = AdkNetWrapper.Api.AdkFirst(pData);


            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            

            // Snurra som loopar igenom alla avtal och gör dragi till säljare
            while (error.lRc == adk.Api.ADKE_OK)
            {
                            
                Console.WriteLine(name);
                Console.WriteLine(sign);


                adk.Api.AdkSetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_OUR_REFERENCE_NAME, ref name);
                adk.Api.AdkSetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_SELLER_CODE, ref sign);

                adk.Api.AdkUpdate(pData);


                

                // Sätter vidare pekaren till nästa artikelgrupp
                error = AdkNetWrapper.Api.AdkNext(pData);
            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();
        }

    }
}