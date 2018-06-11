using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog;

namespace UcsVismaTid
{
    class SkickaData
    {
        //Skapar en sql-connection mot databasen
        public SqlConnection sqlConTid = new SqlConnection(@ConfigurationManager.AppSettings["dbPath"]);

        ErrorLogger logger = new ErrorLogger();

        public void EmptyRowsInWorkdays()
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            try
            {
                string sqlTrunc = "TRUNCATE TABLE VismaTid_RedDay";
                SqlCommand cmdEmptyRowsInWorkdays = new SqlCommand(sqlTrunc, sqlCon2);
                sqlCon2.Open();
                cmdEmptyRowsInWorkdays.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
            
        }

        public void RedDaysTillDatabas(List<Arbetsdagar> arbetsDagar)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddHoliday = new SqlCommand("sp_VismaTid_add_redday", sqlCon2);

            try
            {
                sqlCon2.Open();
                ;
                cmdAddHoliday.CommandType = CommandType.StoredProcedure;
                foreach (var aD in arbetsDagar)
                {
                    cmdAddHoliday.Parameters.Add(new SqlParameter("@arbetsDagar", aD.ArbetsDagar));
                    cmdAddHoliday.Parameters.Add(new SqlParameter("@datum", aD.Datum));
                    cmdAddHoliday.ExecuteNonQuery();
                    cmdAddHoliday.Parameters.Clear();
                }

            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }
        
        public void TimeReportTillDatabas(TimeReports t)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddTimeReport = new SqlCommand("sp_VismaTid_add_tidsrapportering", sqlCon2);

            try
            {
                cmdAddTimeReport.CommandType = CommandType.StoredProcedure;

                cmdAddTimeReport.Parameters.Add(
                    t.TimeCodeId == null ? new SqlParameter("@TidskodID", DBNull.Value) : new SqlParameter("@TidskodID", t.TimeCodeId));
                cmdAddTimeReport.Parameters.Add(
                    t.ProjectId == null ? new SqlParameter("@ProjektID", DBNull.Value) : new SqlParameter("@ProjektID", t.ProjectId));
                cmdAddTimeReport.Parameters.Add(
                    t.HourToInvoice == null ? new SqlParameter("@FaktureradeTimmar", DBNull.Value) : new SqlParameter("@FaktureradeTimmar", t.HourToInvoice));
                cmdAddTimeReport.Parameters.Add(
                    t.HourOfReport == null ? new SqlParameter("@AntalArbetadeTimmar", DBNull.Value) : new SqlParameter("@AntalArbetadeTimmar", t.HourOfReport));
                cmdAddTimeReport.Parameters.Add(
                    t.ProgramUserId == null ? new SqlParameter("@AnställdID", DBNull.Value) : new SqlParameter("@AnställdID", t.ProgramUserId));
                cmdAddTimeReport.Parameters.Add(
                    t.DateOfReport == null ? new SqlParameter("@DatumFörRapport", DBNull.Value) : new SqlParameter("@DatumFörRapport", t.DateOfReport));
                cmdAddTimeReport.Parameters.Add(new SqlParameter("@TidsRapportID", t.TimeReportId));
                cmdAddTimeReport.Parameters.Add(t.ActivityId == null ? new SqlParameter("@AktivitetsID", DBNull.Value) : new SqlParameter("@AktivitetsID", t.ActivityId));
                cmdAddTimeReport.Parameters.Add(t.AmountToInvoice == null ? new SqlParameter("@AmountToInvoice", DBNull.Value) : new SqlParameter("@AmountToInvoice", t.AmountToInvoice));

                sqlCon2.Open();
                cmdAddTimeReport.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }

        }

        public void ProgramUsersTillDatabas(ProgramUsers pU)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddProgramUsers = new SqlCommand("sp_VismaTid_add_anställd", sqlCon2);

            try
            {
                cmdAddProgramUsers.CommandType = CommandType.StoredProcedure;

                cmdAddProgramUsers.Parameters.Add(new SqlParameter("@AnställdID", pU.ProgramUserId));
                cmdAddProgramUsers.Parameters.Add(new SqlParameter("@Personalnummer", pU.PersonalNo));
                cmdAddProgramUsers.Parameters.Add(
                    pU.ProgramUserFirstName == null ? new SqlParameter("@Förnamn", DBNull.Value) : new SqlParameter("@Förnamn", pU.ProgramUserFirstName));
                cmdAddProgramUsers.Parameters.Add(
                    pU.ProgramUserLastName == null ? new SqlParameter("@Efternamn", DBNull.Value) : new SqlParameter("@Efternamn", pU.ProgramUserLastName));
                cmdAddProgramUsers.Parameters.Add(
                    pU.ProgramUserGroupId == null ? new SqlParameter("@AnvändarGruppID", DBNull.Value) : new SqlParameter("@AnvändarGruppID", pU.ProgramUserGroupId));
                cmdAddProgramUsers.Parameters.Add(
                    pU.ResultUnitId == null ? new SqlParameter("@ResultatEnhetId", DBNull.Value) : new SqlParameter("@ResultatEnhetId", pU.ResultUnitId));

                sqlCon2.Open();
                cmdAddProgramUsers.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void ProjectTillDatabas(Projects p)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddProject = new SqlCommand("sp_VismaTid_add_projekt", sqlCon2);

            try
            {
                cmdAddProject.CommandType = CommandType.StoredProcedure;

                cmdAddProject.Parameters.Add(new SqlParameter("@AnställdID", p.ProgramUserId));
                cmdAddProject.Parameters.Add(new SqlParameter("@KundID", p.CustomerId));
                cmdAddProject.Parameters.Add(new SqlParameter("@DatumStart", p.DateStart));
                cmdAddProject.Parameters.Add(new SqlParameter("@ProjektID", p.ProjectId));

                cmdAddProject.Parameters.Add(
                    p.DateEnd == null ? new SqlParameter("@DatumSlut", DBNull.Value) : new SqlParameter("@DatumSlut", p.DateEnd));
                cmdAddProject.Parameters.Add(
                    p.NoOfHours == null ? new SqlParameter("@AntalTimmar", DBNull.Value) : new SqlParameter("@AntalTimmar", p.NoOfHours));
                cmdAddProject.Parameters.Add(
                    p.ProjectCategoryId == null ? new SqlParameter("@ProjektKategoriID", DBNull.Value) : new SqlParameter("@ProjektKategoriID", p.ProjectCategoryId));
                cmdAddProject.Parameters.Add(
                    p.StdPriceListId == null ? new SqlParameter("@StdPrisListaId", DBNull.Value) : new SqlParameter("@StdPrisListaId", p.StdPriceListId));

                sqlCon2.Open();
                cmdAddProject.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void TimeCodeTillDatabas(TimeCodes tC)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddTimeCode = new SqlCommand("sp_VismaTid_add_tidskod", sqlCon2);

            try
            {
                cmdAddTimeCode.CommandType = CommandType.StoredProcedure;

                cmdAddTimeCode.Parameters.Add(new SqlParameter("@TidsKodID", tC.TimeCodeId));
                cmdAddTimeCode.Parameters.Add(new SqlParameter("@TidsKodNamn", tC.TimeCodeName));
                cmdAddTimeCode.Parameters.Add(new SqlParameter("@Kod", tC.Code));
                cmdAddTimeCode.Parameters.Add(
                    tC.AbsenceType == null ? new SqlParameter("@FrånvaroTyp", DBNull.Value) : new SqlParameter("@FrånvaroTyp", tC.AbsenceType));

                sqlCon2.Open();
                cmdAddTimeCode.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void PricingTillDatabas(Pricings p)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddPricing = new SqlCommand("sp_VismaTid_add_pris", sqlCon2);

            try
            {
                cmdAddPricing.CommandType = CommandType.StoredProcedure;

                cmdAddPricing.Parameters.Add(new SqlParameter("@PrisID", p.PriceId));
                cmdAddPricing.Parameters.Add(
                    p.ActivityId == null ? new SqlParameter("@AktivitetsID", DBNull.Value) : new SqlParameter("@AktivitetsID", p.ActivityId));
                cmdAddPricing.Parameters.Add(
                    p.ProjectId == null ? new SqlParameter("@ProjektID", DBNull.Value) : new SqlParameter("@ProjektID", p.ProjectId));
                cmdAddPricing.Parameters.Add(
                    p.ProgramUserId == null ? new SqlParameter("@AnställdID", DBNull.Value) : new SqlParameter("@AnställdID", p.ProgramUserId));
                cmdAddPricing.Parameters.Add(
                    p.Price == null ? new SqlParameter("@Pris", DBNull.Value) : new SqlParameter("@Pris", p.Price));
                cmdAddPricing.Parameters.Add(
                    p.ProgramUserGroupId == null ? new SqlParameter("@AnvändarGruppID", DBNull.Value) : new SqlParameter("@AnvändarGruppID", p.ProgramUserGroupId));
                cmdAddPricing.Parameters.Add(
                    p.ProgramUserGroupId == null ? new SqlParameter("@PrisListaPeriodID", DBNull.Value) : new SqlParameter("@PrisListaPeriodID", p.PriceListPeriodId));

                sqlCon2.Open();
                cmdAddPricing.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void PriceListTillDatabas(PriceLists pL)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAttPriceList = new SqlCommand("sp_VismaTid_add_prislista", sqlCon2);

            try
            {
                cmdAttPriceList.CommandType = CommandType.StoredProcedure;

                cmdAttPriceList.Parameters.Add(new SqlParameter("@PrisListaID", pL.PriceListId));
                cmdAttPriceList.Parameters.Add(new SqlParameter("@PrisListaNamn", pL.PriceListName));
                cmdAttPriceList.Parameters.Add(new SqlParameter("@StdPrisLista", pL.StdPriceList));

                sqlCon2.Open();
                cmdAttPriceList.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void PriceListPeriodsTillDatabas(PriceListPeriods plp)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAttPriceListPeriods = new SqlCommand("sp_VismaTid_add_prislistaperiod", sqlCon2);

            try
            {
                cmdAttPriceListPeriods.CommandType = CommandType.StoredProcedure;

                cmdAttPriceListPeriods.Parameters.Add(new SqlParameter("@PrisListaID", plp.PriceListId));
                cmdAttPriceListPeriods.Parameters.Add(new SqlParameter("@PrisListaPeriodID", plp.PriceListPeriodId));
                cmdAttPriceListPeriods.Parameters.Add(new SqlParameter("@DatumStart", plp.DateStart));
                cmdAttPriceListPeriods.Parameters.Add(
                    plp.DateEnd == null ? new SqlParameter("@DatumSlut", DBNull.Value) : new SqlParameter("@DatumSlut", plp.DateEnd));

                sqlCon2.Open();
                cmdAttPriceListPeriods.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void ParticipantsTillDatabas(Participants p)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddParticipants = new SqlCommand("sp_VismaTid_add_deltagare", sqlCon2);

            try
            {
                cmdAddParticipants.CommandType = CommandType.StoredProcedure;

                cmdAddParticipants.Parameters.Add(new SqlParameter("@AnställdID", p.ProgramUserId));
                cmdAddParticipants.Parameters.Add(new SqlParameter("@ProjektID", p.ProjectId));

                sqlCon2.Open();
                cmdAddParticipants.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void CustomerTillDatabas(Customers c)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddCustomer = new SqlCommand("sp_VismaTid_add_kund", sqlCon2);

            try
            {
                cmdAddCustomer.CommandType = CommandType.StoredProcedure;

                cmdAddCustomer.Parameters.Add(new SqlParameter("@KundID", c.CustomerId));
                cmdAddCustomer.Parameters.Add(new SqlParameter("@KundNummer", c.CustomerNo));
                cmdAddCustomer.Parameters.Add(new SqlParameter("@KundNamn", c.CustomerName));
                cmdAddCustomer.Parameters.Add(new SqlParameter("@OrganisationsNummer", c.OrganisationNo));
                cmdAddCustomer.Parameters.Add(
                    c.PriceListId == null ? new SqlParameter("@PrisListaID", DBNull.Value) : new SqlParameter("@PrisListaID", c.PriceListId));

                sqlCon2.Open();
                cmdAddCustomer.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void CustomerCategoryTillDatabas(CustomerCategories cG)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddCustomerCategory = new SqlCommand("sp_VismaTid_add_kundkategori", sqlCon2);

            try
            {
                cmdAddCustomerCategory.CommandType = CommandType.StoredProcedure;

                cmdAddCustomerCategory.Parameters.Add(new SqlParameter("@KundKategoriID", cG.CustomerCategoryId));
                cmdAddCustomerCategory.Parameters.Add(new SqlParameter("@KundKategori", cG.CustomerCategory));
                cmdAddCustomerCategory.Parameters.Add(new SqlParameter("@Beskrivning", cG.Description));

                sqlCon2.Open();
                cmdAddCustomerCategory.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void FreeDayTillDatabas(FreeDays fD)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddFreeDay = new SqlCommand("sp_VismaTid_add_fridag", sqlCon2);

            try
            {
                cmdAddFreeDay.CommandType = CommandType.StoredProcedure;

                cmdAddFreeDay.Parameters.Add(new SqlParameter("@FriDagID", fD.FreeDayId));
                cmdAddFreeDay.Parameters.Add(new SqlParameter("@Dag", fD.Day));
                cmdAddFreeDay.Parameters.Add(new SqlParameter("@Beskrivning", fD.Note));

                cmdAddFreeDay.Parameters.Add(
                    fD.Hours == null ? new SqlParameter("@Timmar", DBNull.Value) : new SqlParameter("@Timmar", fD.Hours));

                sqlCon2.Open();
                cmdAddFreeDay.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void ActivityTillDatabas(Activities a)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddActivity = new SqlCommand("sp_VismaTid_add_aktivitet", sqlCon2);

            try
            {
                cmdAddActivity.CommandType = CommandType.StoredProcedure;

                cmdAddActivity.Parameters.Add(new SqlParameter("@AktivitetsID", a.ActivityId));
                cmdAddActivity.Parameters.Add(new SqlParameter("@AktivitetsNamn", a.ActivityName));
                cmdAddActivity.Parameters.Add(new SqlParameter("@AktivitetsKod", a.Code));
                cmdAddActivity.Parameters.Add(
                    a.TimeCodeId == null ? new SqlParameter("@TidsKodID", DBNull.Value) : new SqlParameter("@TidsKodID", a.TimeCodeId));
                cmdAddActivity.Parameters.Add(a.ProjectId == null ? new SqlParameter("@ProjektID", DBNull.Value) : new SqlParameter("@ProjektID", a.ProjectId));

                sqlCon2.Open();
                cmdAddActivity.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void ProgramUserGroupTillDatabas(ProgramUserGroups pug)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddProgramUserGroup = new SqlCommand("sp_VismaTid_add_användargrupp", sqlCon2);

            try
            {
                cmdAddProgramUserGroup.CommandType = CommandType.StoredProcedure;

                cmdAddProgramUserGroup.Parameters.Add(new SqlParameter("@AnvändarGruppID", pug.ProgramUserGroupId));
                cmdAddProgramUserGroup.Parameters.Add(new SqlParameter("@AnvändarGruppNamn", pug.ProgramUserGroupName));

                sqlCon2.Open();
                cmdAddProgramUserGroup.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void ProgramUserCalcPriceTillDatabas(ProgramUserCalcPrices pucp)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddProgramUserCalcPrice = new SqlCommand("sp_VismaTid_add_anställdkalkpris", sqlCon2);

            try
            {
                cmdAddProgramUserCalcPrice.CommandType = CommandType.StoredProcedure;

                cmdAddProgramUserCalcPrice.Parameters.Add(new SqlParameter("@AnställdKalkPrisID", pucp.ProgramUserCalcPriceId));
                cmdAddProgramUserCalcPrice.Parameters.Add(
                    pucp.CalcPrice == null ? new SqlParameter("@Kalkpris", DBNull.Value) : new SqlParameter("@KalkPris", pucp.CalcPrice));
                cmdAddProgramUserCalcPrice.Parameters.Add(new SqlParameter("@AnställdID", pucp.ProgramUserId));
                cmdAddProgramUserCalcPrice.Parameters.Add(new SqlParameter("@StartDatum", pucp.StartDate));
                cmdAddProgramUserCalcPrice.Parameters.Add(
                    pucp.EndDate == null ? new SqlParameter("@SlutDatum", DBNull.Value) : new SqlParameter("@SlutDatum", pucp.EndDate));

                sqlCon2.Open();
                cmdAddProgramUserCalcPrice.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void ProjectCategoryTillDatabas(ProjectCategories pC)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddProjectCategory = new SqlCommand("sp_VismaTid_add_projektkategori", sqlCon2);

            try
            {
                cmdAddProjectCategory.CommandType = CommandType.StoredProcedure;

                cmdAddProjectCategory.Parameters.Add(new SqlParameter("@ProjektKategoriID", pC.ProjectCategoryId));
                cmdAddProjectCategory.Parameters.Add(new SqlParameter("@ProjektKategori", pC.ProjectCategory));
                cmdAddProjectCategory.Parameters.Add(new SqlParameter("@Beskrivning", pC.Description));
                cmdAddProjectCategory.Parameters.Add(new SqlParameter("@Aktiv", pC.Active));

                sqlCon2.Open();
                cmdAddProjectCategory.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void ResultUnitTillDatabas(ResultUnit r)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddResultatenhet = new SqlCommand("sp_VismaTid_add_resultatenheter", sqlCon2);

            try
            {
                cmdAddResultatenhet.CommandType = CommandType.StoredProcedure;

                cmdAddResultatenhet.Parameters.Add(new SqlParameter("@ResultatEnhetID", r.ResultUnitId));
                cmdAddResultatenhet.Parameters.Add(new SqlParameter("@ResultatEnhet", r.ResultUnitName));
                cmdAddResultatenhet.Parameters.Add(new SqlParameter("@EnhetsNamn", r.Description));

                sqlCon2.Open();
                cmdAddResultatenhet.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void MonthlyWorkHourForecastTillDatabas(Arbetsdagar month, decimal? workingHours, int ProgramUserId)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);
            SqlCommand cmdAddWorkHourForecast = new SqlCommand("sp_VismaTid_add_workhourforecast", sqlCon2);

            try
            {
                cmdAddWorkHourForecast.CommandType = CommandType.StoredProcedure;
                cmdAddWorkHourForecast.Parameters.Add(new SqlParameter("@datum", month.Datum));
                cmdAddWorkHourForecast.Parameters.Add(new SqlParameter("@användarID", ProgramUserId));
                cmdAddWorkHourForecast.Parameters.Add(
                    workingHours == null
                        ? new SqlParameter("@arbetsTimmar", DBNull.Value)
                        : new SqlParameter("@arbetsTimmar", workingHours));

                sqlCon2.Open();
                cmdAddWorkHourForecast.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }

        public void consultForecastTillDatabas(int projectId, decimal? price, int userId)
        {
            var sqlCon2 = new SqlConnection(sqlConTid.ConnectionString);

            using (SqlCommand cmdAddForecastConsult = new SqlCommand("sp_VismaTid_add_forecastCOnsultPrice", sqlCon2))
            {
                try
                {
                    cmdAddForecastConsult.CommandType = CommandType.StoredProcedure;
                    cmdAddForecastConsult.Parameters.Add(new SqlParameter("användarId", userId));
                    cmdAddForecastConsult.Parameters.Add(new SqlParameter("projectId", projectId));
                    cmdAddForecastConsult.Parameters.Add(new SqlParameter("timpris", price));
                    sqlCon2.Open();
                    cmdAddForecastConsult.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    logger.ErrorMessage(ex);
                }
                finally
                {
                    sqlCon2.Close();;
                }
            }
        }
    }
}
