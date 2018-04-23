using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adk = AdkNetWrapper;
using System.Windows;


namespace UCSTest
{
    class Program
    {
        

        static void Main(string[] args)
        {
            /*
              IF EXISTS (SELECT @dokumentNummer FROM Avtal where dokumentNummer = @dokumentNummer)
	BEGIN
		UPDATE Avtal SET dokumentNummer = @dokumentNummer, avtalsDatum = @avtalsDatum, startDatum = @startDatum, tempSlutDatum = @tempSlutDatum,
		kundNummer = @kundnummer, isActive = @isActive, fakturaIntervall = @fakturaIntervall, periodStart = @periodStart, periodEnd = @periodEnd,
		totalKostnad = @totalKostnad, uppsägningstid = @uppsägningstid, förlängningstid = @förlängningstid, avtalsDatumSlut = @avtalsDatumSlut
	END
	ELSE
	BEGIN
	INSERT INTO Avtal (dokumentNummer, avtalsDatum, startDatum, tempSlutDatum, kundNummer, isActive, fakturaIntervall, periodStart, periodEnd, totalKostnad, uppsägningstid, förlängningstid, avtalsDatumSlut)
	VALUES
	(@dokumentNummer, @avtalsDatum, @startDatum, @tempSlutDatum, @kundnummer, @isActive, @fakturaIntervall, @periodStart, @periodEnd, @totalKostnad, @uppsägningstid, @förlängningstid, @avtalsDatumSlut)
END
             */

            VismaData go = new VismaData();
            // C:\users\sijoh0500\Work Folders\Documents\Github\UCSTest\UCSTest\fakturaDB.mdf
        }



    }
}
