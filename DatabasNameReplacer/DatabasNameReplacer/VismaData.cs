using System;
using adk = AdkNetWrapper;

namespace DatabasNameReplacer
{
    public class VismaData
    {
        SendData sendData = new SendData();
        private adk.Api.ADKERROR error;
        String ftg = @"C:\ProgramData\SPCS\SPCS Administration\Företag\Ovnbol2000";
        String sys = @"C:\ProgramData\SPCS\SPCS Administration\Gemensamma filer";

        private int pData;

        public VismaData()
        {
            GetAvtalData();
        }

        private void GetAvtalData()
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

            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_AGREEMENT_HEAD);
            error = AdkNetWrapper.Api.AdkFirst(pData);

            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                Console.WriteLine(errortext);
            }

            while (error.lRc == adk.Api.ADKE_OK)
            {
                string nameReplace = new string(' ', 25);

                AdkNetWrapper.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_OUR_REFERENCE_NAME, ref nameReplace, 25);

                if (nameReplace != "Dragi Spasojevic")
                {
                    nameReplace = "Dragi Spasojevic";

                    adk.Api.AdkSetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_OUR_REFERENCE_NAME, ref nameReplace);

                    adk.Api.AdkUpdate(pData);


                }

                // Sätter vidare pekaren till nästa artikelgrupp
                error = AdkNetWrapper.Api.AdkNext(pData);
            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();
        }

    }
}