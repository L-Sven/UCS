﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UcsVismaTid
{
    public class VismaTidData
    {
        private ErrorLogger logger = new ErrorLogger();
        private SkickaData sendData = new SkickaData();
        private static SqlConnection _sqlCon = new SqlConnection(@ConfigurationManager.AppSettings["vismaTidPath"]);

        List<Arbetsdagar> arbetsdagarList = new List<Arbetsdagar>();
        VismaTidDataDataContext db = new VismaTidDataDataContext(_sqlCon.ConnectionString);

        public VismaTidData()
        {
            //GetActivity();
            //Console.WriteLine("Aktiviteter klar!");
            //GetWorkdaysWholeYear();
            //Console.WriteLine("Arbetsdagar klar!");
            GetTimeReport();
            Console.WriteLine("Tidsrapport klar!");
            //GetProgramUsers();
            //Console.WriteLine("Anställda klar!");
            //GetProgramUsersGroup();
            //Console.WriteLine("Anställdagrupper klar!");
            //GetProject();
            //Console.WriteLine("Projekt klar!");
            //GetTimeCode();
            //Console.WriteLine("Tidskoder klar!");
            //GetPricing();
            //Console.WriteLine("Priser klar!");
            //GetPriceList();
            //Console.WriteLine("Prislistor klar!");
            //GetPriceListPeriod();
            //Console.WriteLine("Prislistaperioder klar!");
            //GetParticipants();
            //Console.WriteLine("Deltagare klar!");
            //GetProgramUserCalcPrice();
            //Console.WriteLine("AnställdaKalkpris klar!");
            //GetCustomer();
            //Console.WriteLine("Kunder klar");
            //GetCustomerCategory();
            //Console.WriteLine("Kundkategori klar!");
            //GetProjectCategory();
            //Console.WriteLine("Projektkategori klar!");
            //GetResultUnit();
            //Console.WriteLine("Resultatenhet klar!");

            Console.WriteLine("Programmet har kört färdig!");
        }

        private void CalculateNextMonthWorkinghours(ProgramUser user)
        {
            try
            {
                //Räkna ut användarens arbetstimmar, dvs jobbar hen 100% eller 50%?
                var workTimeSchedule = user.ProgramUserSchedules
                    .Where(x => x.ProgramUserId == user.ProgramUserId &&
                                (x.EndDate.ToString() == "" || x.EndDate == null)).Select(y => y.Schedule)
                    .Select(t => t.WorkingProc).Single();

                foreach (var month in arbetsdagarList)
                {
                    //Antal arbetstimmar i månaden räknas ut enligt dagar *8 timmar om dagen* arbetstid i decimal(ex: 0, 80 för 80 %).
                    var workingHours = month.ArbetsDagar * 8 * (workTimeSchedule / 100);

                    sendData.MonthlyWorkHourForecastTillDatabas(month, workingHours, user.ProgramUserId);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex + "User: " + user.ProgramUserId);
            }
            
        }

        private void GetWorkdaysWholeYear()
        {
            //Vi tömmer tablen för alltid i förhand!
            sendData.EmptyRowsInWorkdays();
            
            //Dagens datum är inte alltid den första så vi ta bort dagarna och lägger värdet 01 som dag
            string idag = DateTime.Today.ToShortDateString().Substring(0, 8) + "01";
            int antalDagarMånad = DateTime.DaysInMonth(DateTime.Parse(idag).Year,DateTime.Parse(idag).Month);

            //Vi kan lägga till en hel månad, men då blir uträkningen fel och vi får dubletter på den första dagen varje månad.
            //Istället lägger vi till antalet dagar i den månaden till idag variablen. T.ex. 2018-06-01 + 30 dagar är lika med 2018-06-30 istället för 2018-07-01
            string idagOmEnMånad = idag.Substring(0, 8) + antalDagarMånad;
            int arbetsDagar;

            for (int i = 0; i < 12; i++)
            {
                Arbetsdagar aD = new Arbetsdagar();
                
                //Vi anropar www.arbetsdag.se/api genom xmlConString. Uppstår det problem med API se den hemsidan!
                var xmlConString = "http://api.arbetsdag.se/v1/dagar.xml?fran=" + idag + "&till=" + idagOmEnMånad + "&key=7edead340d36f038593fc88686b454ac6a2d7683&id=1234";
                XElement rödaDagar = XElement.Load(xmlConString);

                arbetsDagar = int.Parse(rödaDagar.Element("antal_arbetsdagar").Value);
                var datum = idag.Substring(0, 10);    //Sparar datum i format yyyymmdd

                aD.ArbetsDagar = arbetsDagar;
                aD.Datum = datum;
                arbetsdagarList.Add(aD);

                //Inkrementerar med 1 månad.
                idag = DateTime.Parse(idag).AddMonths(1).ToShortDateString();
                antalDagarMånad = DateTime.DaysInMonth(DateTime.Parse(idag).Year, DateTime.Parse(idag).Month);
                idagOmEnMånad = idag.Substring(0, 8) + antalDagarMånad;
            }

            sendData.RedDaysTillDatabas(arbetsdagarList);
        }

        private void GetTimeReport()
        {
            TimeReports timeReports = new TimeReports();
            List<TimeReports> tList = new List<TimeReports>();
            try
            {
                var timeReport = from report in db.TimeReports
                    select report;

                foreach (var element in timeReport)
                {
                    timeReports.TimeReportId = element.TimeReportId;
                    timeReports.ProjectId = element.ProjectId;
                    timeReports.HourToInvoice = element.HourToInvoice;
                    timeReports.HourOfReport = element.HourOfReport;
                    timeReports.ProgramUserId = element.ProgramUserId;
                    timeReports.TimeCodeId = element.TimeCodeId;
                    timeReports.DateOfReport = element.DateOfReport;
                    timeReports.ResultUnitId = element.BookResultUnitId;
                    timeReports.ActivityId = element.ActivityId;
                    timeReports.AmountToInvoice = CalculateAmountToInvoice(element);

                    if (element.ProgramUserId == 160297)
                    {
                        Console.WriteLine("Hej!");
                    }

                    tList.Add(timeReports);
                    //sendData.TimeReportTillDatabas(timeReports);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            sendData.TimeReportTillDatabas(tList);
        }

        private decimal? CalculateAmountToInvoice(TimeReport element)
        {
            //Vi räknar ut priset för tidsrapporten direkt här.
            decimal? price = 0.00M;

            if(element.ProjectId != null)
            {
                if (element.Project.HourlyPrice != null)
                {
                    price = element.Project.HourlyPrice.Value;
                }
                else if (element.Project.Customer.HourlyPrice != null)
                {
                    price = element.Project.Customer.HourlyPrice.Value;
                }
            }
            else if (element.CustomerId != null)
            {
                if (element.Customer.PriceListId != null)
                {
                    var pricelistID = element.Customer.PriceListId;
                    price = db.Pricings.Where(x => x.PriceListPeriod.PriceListId == pricelistID).Select(x => x.Price)
                        .Single();
                }
            }
            
            if(price == null)
            {
                var items = db.InvoiceSettingRowDetails.Where(x => x.InvoiceId == element.Invoice.InvoiceId)
                    .Select(x => x.ItemNo);

                foreach (var item in items)
                {
                    price = db.Items.Where(x => x.ItemNo == item).Select(x => x.CalcPriceUnit).Single();
                    return price = price * element.Quantity;
                }
            }
            
            if (element.HourToInvoice != null)
                price = price * element.HourToInvoice;

            return price;
        }
        
        private void GetProgramUsers()
        {
            var programUser = from user in db.ProgramUsers
                              select user;

            foreach (var user in programUser)
            {
                if (user.Active)
                {
                    ProgramUsers programUsers = new ProgramUsers();
                    try
                    {
                        programUsers.ProgramUserId = user.ProgramUserId;

                        programUsers.ProgramUserFirstName = user.FirstName;
                        programUsers.ProgramUserGroupId = user.ProgramUserGroupId;
                        programUsers.ResultUnitId = user.ResUnitId;
                        programUsers.ProgramUserLastName = user.Name;
                        programUsers.PersonalNo = user.PersonalNo;

                        sendData.ProgramUsersTillDatabas(programUsers);

                        CalculateNextMonthWorkinghours(user);
                        CalculateForecastForConsult(user);
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorMessage(ex);
                    }
                }
            }
        }

        private void GetProgramUsersGroup()
        {
            var programUserGroups = from groups in db.ProgramUserGroups
                                    select groups;
            
            foreach (var element in programUserGroups)
            {
                ProgramUserGroups programUserGroup = new ProgramUserGroups();

                try
                {
                    programUserGroup.ProgramUserGroupId = element.ProgramUserGroupId;
                    programUserGroup.ProgramUserGroupName = element.ProgramUserGroupName;

                    sendData.ProgramUserGroupTillDatabas(programUserGroup);
                }
                catch (Exception ex)
                {
                    logger.ErrorMessage(ex);
                }
            }

        }

        private void GetProject()
        {
            Projects projects = new Projects();

            try
            {
                var project = from _project in db.Projects
                    select _project;

                foreach (var element in project)
                {
                    projects.ProjectId = element.ProjectId;
                    projects.NoOfHours = element.NoOfHours;
                    projects.CustomerId = element.CustomerId;
                    projects.ProgramUserId = element.ProgramUserId;
                    projects.DateStart = element.DateStart;
                    projects.DateEnd = element.DateEnd;
                    projects.StdPriceListId = element.StdPriceListId;
                    projects.ProjectCategoryId = element.ProjectCategoryId;

                    sendData.ProjectTillDatabas(projects);
                }
            }
            catch(Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        private void GetTimeCode()
        {
            TimeCodes timeCodes = new TimeCodes();

            try
            {
                var timeCode = from code in db.TimeCodes
                    select code;

                foreach (var element in timeCode)
                {
                    timeCodes.AbsenceType = element.AbsenceType;
                    timeCodes.TimeCodeId = element.TimeCodeId;
                    timeCodes.TimeCodeName = element.TimeCodeName;
                    timeCodes.Code = element.Code;

                    sendData.TimeCodeTillDatabas(timeCodes);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        private void GetPricing()
        {
            Pricings pricings = new Pricings();

            try
            {
                var pricing = from price in db.Pricings
                    select price;

                foreach (var element in pricing)
                {
                    pricings.ProgramUserGroupId = element.ProgramUserGroupId;
                    pricings.Price = element.Price;
                    pricings.PriceListPeriodId = element.PriceListPeriodId;
                    pricings.ProgramUserId = element.ProgramUserId;
                    pricings.ProjectId = element.ProjectId;
                    pricings.ActivityId = element.ActivityId;
                    pricings.PriceId = element.PricingId;

                    sendData.PricingTillDatabas(pricings);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }
        
        private void GetPriceList()
        {
            PriceLists priceLists = new PriceLists();

            try
            {
                var priceList = from pl in db.PriceLists
                    select pl;

                foreach (var element in priceList)
                {
                    priceLists.PriceListId = element.PriceListId;
                    priceLists.PriceListName = element.PriceListName;
                    priceLists.StdPriceList = element.StdPriceList;

                    sendData.PriceListTillDatabas(priceLists);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        private void GetPriceListPeriod()
        {
            PriceListPeriods priceListPeriod = new PriceListPeriods();

            try
            {
                var priceListPeriods = from plp in db.PriceListPeriods
                    select plp;

                foreach (var element in priceListPeriods)
                {
                    priceListPeriod.PriceListPeriodId = element.PriceListPeriodId;
                    priceListPeriod.PriceListId = element.PriceListId;
                    priceListPeriod.DateStart = element.DateStart;
                    priceListPeriod.DateEnd = element.DateEnd;

                    sendData.PriceListPeriodsTillDatabas(priceListPeriod);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        private void GetParticipants()
        {
            Participants participant = new Participants();

            try
            {
                var participants = from p in db.Participants
                    select p;

                foreach (var el in participants)
                {
                    participant.ProjectId = el.ProjectId;
                    participant.ProgramUserId = el.ProgramUserId;

                    sendData.ParticipantsTillDatabas(participant);
                }
            }
            catch(Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        private void GetProgramUserCalcPrice()
        {
            ProgramUserCalcPrices calcPrice = new ProgramUserCalcPrices();

            try
            {

                var calcPrices = from cP in db.ProgramUserCalcPrices
                    select cP;

                foreach (var el in calcPrices)
                {
                    calcPrice.ProgramUserCalcPriceId = el.ProgramUserCalcPriceId;
                    calcPrice.ProgramUserId = el.ProgramUserId;
                    calcPrice.CalcPrice = el.CalcPrice;
                    calcPrice.StartDate = el.StartDate;
                    calcPrice.EndDate = el.EndDate;

                    sendData.ProgramUserCalcPriceTillDatabas(calcPrice);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        public void GetFreeDay()
        {
            FreeDays freeDay = new FreeDays();

            try
            {
                var freeDays = from fD in db.FreeDays
                    select fD;

                foreach (var el in freeDays)
                {
                    freeDay.FreeDayId = el.FreeDaysId;
                    freeDay.Day = el.Day;
                    freeDay.Hours = el.Hours;
                    freeDay.Note = el.Note;

                    sendData.FreeDayTillDatabas(freeDay);
                }
            }
            catch(Exception ex)
            {
                logger.ErrorMessage(ex);   
            }
        }

        public void GetCustomerCategory()
        {
            CustomerCategories customerCategory = new CustomerCategories();

            try
            {
                var customerCategories = from cG in db.CustomerCategories
                    select cG;

                foreach (var el in customerCategories)
                {
                    customerCategory.CustomerCategoryId = el.CustomerCategoryId;
                    customerCategory.CustomerCategory = el.CustomerCategory1;
                    customerCategory.Description = el.Description;

                    sendData.CustomerCategoryTillDatabas(customerCategory);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        public void GetCustomer()
        {
            Customers customer = new Customers();

            try
            {
                var customers = from c in db.Customers
                    select c;

                foreach (var el in customers)
                {
                    customer.CustomerId = el.CustomerId;
                    customer.CustomerNo = el.CustomerNo;
                    customer.CustomerName = el.CustomerName;
                    customer.OrganisationNo = el.OrganisationNo;
                    customer.PriceListId = el.PriceListId;

                    sendData.CustomerTillDatabas(customer);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        public void GetActivity()
        {
            var activities = from a in db.Activities
                             select a;

            foreach (var el in activities)
            {
                Activities activity = new Activities();
                try
                {
                    activity.ActivityId = el.ActivityId;
                    activity.ActivityName = el.ActivityName;
                    activity.Code = el.Code;
                    activity.TimeCodeId = el.TimeCodeId;
                    activity.ProjectId = el.ProjectId;

                    sendData.ActivityTillDatabas(activity);
                }
                catch (Exception ex)
                {
                    logger.ErrorMessage(ex);
                }
            }
        }

        public void GetProjectCategory()
        {
            ProjectCategories projectCategory = new ProjectCategories();

            try
            {
                var categories = from c in db.ProjectCategories
                    select c;

                foreach (var el in categories)
                {
                    projectCategory.ProjectCategoryId = el.ProjectCategoryId;
                    projectCategory.ProjectCategory = el.ProjectCategory1;
                    projectCategory.Description = el.Description;
                    projectCategory.Active = el.Active;

                    sendData.ProjectCategoryTillDatabas(projectCategory);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        public void GetResultUnit()
        {
            ResultUnit resultUnit = new ResultUnit();

            try
            {
                var resultUnits = from r in db.BookResultUnits
                    select r;

                foreach (var el in resultUnits)
                {
                    resultUnit.ResultUnitId = el.BookResultUnitId;
                    resultUnit.ResultUnitName = el.BookResultUnit1;
                    resultUnit.Description = el.Description;

                    sendData.ResultUnitTillDatabas(resultUnit);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        public void CalculateForecastForConsult(ProgramUser user)
        {
            //Gets all projects where this consult is active on.
            var projects = from proj in db.Projects
                where proj.ProgramUserId == user.ProgramUserId
                select proj;

            foreach (var project in projects)
            {
                if (project.ProjectId > 2 && project.Active && project.CustomerId > 2)
                {
                    var programUserGroup = project.ProgramUser.ProgramUserGroup;
                    var customer = project.Customer;

                    var priceToCustomer = from price in db.Pricings
                        where price.PriceListPeriod.PriceListId == customer.PriceListId &&
                              !price.PriceListPeriod.DateEnd.HasValue &&
                              price.ProgramUserGroup == programUserGroup
                        select price.Price;

                    var test = db.Pricings.Where(x => x.PriceListPeriod.PriceListId == customer.PriceListId
                                                      && !x.PriceListPeriod.DateEnd.HasValue &&
                                                      x.ProgramUserGroup == programUserGroup)
                                                      .Select(x => x.Price).Single();

                    sendData.consultForecastTillDatabas(project.ProjectId, test, user.ProgramUserId);
                }
            }

        }

        public void GetInvoiceData()
        {
            
        }
    }
}