using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UcsVismaTid
{
    public class VismaTidData
    {
        private ErrorLogger logger = new ErrorLogger();
        private SkickaData sendData = new SkickaData();
        private SqlConnection _sqlCon = new SqlConnection(@"Data Source=LAPTOP-B65T75P1\VISMA;Initial Catalog=SPC"); //new SqlConnection(@ConfigurationManager.AppSettings["dbPath"]);
        
        VismaTidDataDataContext db = new VismaTidDataDataContext();

        public VismaTidData()
        {
            GetTimeReport();
            Console.WriteLine("Tidsrapport klar!");
            GetProgramUsers();
            Console.WriteLine("Anställda klar!");
            GetProgramUsersGroup();
            Console.WriteLine("Anställdagrupper klar!");
            GetProject();
            Console.WriteLine("Projekt klar!");
            GetTimeCode();
            Console.WriteLine("Tidskoder klar!");
            GetPricing();
            Console.WriteLine("Priser klar!");
            GetPriceList();
            Console.WriteLine("Prislistor klar!");
            GetPriceListPeriod();
            Console.WriteLine("Prislistaperioder klar!");
            GetParticipants();
            Console.WriteLine("Deltagare klar!");
            GetProgramUserCalcPrice();
            Console.WriteLine("AnställdaKalkpris klar!");
            GetCustomer();
            Console.WriteLine("Kunder klar");
            GetCustomerCategory();
            Console.WriteLine("Kundkategori klar!");
            GetProjectCategory();
            Console.WriteLine("Projektkategori klar!");
            GetResultUnit();
            Console.WriteLine("Resultatenhet klar!");
        }

        private void GetTimeReport()
        {
            TimeReports timeReports = new TimeReports();

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

                    sendData.TimeReportTillDatabas(timeReports);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        private void GetProgramUsers()
        {
            ProgramUsers programUsers = new ProgramUsers();

            try
            {
                var programUser = from user in db.ProgramUsers
                    select user;

                foreach (var element in programUser)
                {
                    programUsers.ProgramUserId = element.ProgramUserId;
                    
                    programUsers.ProgramUserFirstName = element.FirstName;
                    programUsers.ProgramUserGroupId = element.ProgramUserGroupId;
                    programUsers.ResultUnitId = element.ResUnitId;
                    programUsers.ProgramUserLastName = element.Name;
                    programUsers.PersonalNo = element.PersonalNo;

                    sendData.ProgramUsersTillDatabas(programUsers);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
        }

        private void GetProgramUsersGroup()
        {
            ProgramUserGroups programUserGroup = new ProgramUserGroups();

            try
            {
                var programUserGroups = from groups in db.ProgramUserGroups
                    select groups;

                foreach (var element in programUserGroups)
                {
                    programUserGroup.ProgramUserGroupId = element.ProgramUserGroupId;
                    programUserGroup.ProgramUserGroupName = element.ProgramUserGroupName;

                    sendData.ProgramUserGroupTillDatabas(programUserGroup);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
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
            Activities activity = new Activities();

            try
            {
                var activities = from a in db.Activities
                    select a;

                foreach (var el in activities)
                {
                    activity.ActivityId = el.ActivityId;
                    activity.ActivityName = el.ActivityName;
                    activity.Code = el.Code;
                    activity.TimeCodeId = el.TimeCodeId;

                    sendData.ActivityTillDatabas(activity);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
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
    }
}