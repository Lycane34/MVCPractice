using MVCPractice.Models;
using MVCPractice.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MVCPractice.Controllers
{
    [Authorize(Roles = "HR")]
    public class HolidayController : Controller
    {
        LocalDBEntities LDE = new LocalDBEntities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string GetTable()
        {
            var holidays = LDE.LegalHolidayDates.ToList();

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(holidays);
            return json;
        }
        private string GetTurkishDayName(DayOfWeek dayOfWeek)
        {
            CultureInfo turkishCulture = new CultureInfo("tr-TR");
            return turkishCulture.DateTimeFormat.GetDayName(dayOfWeek);
        }

        [HttpPost]
        public ActionResult AddHolidayToTable(string StartDate, string FinishDate, string Name)
        {

            var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
            int userId = int.Parse(userIdClaim.Value);
            if (DateTime.TryParse(StartDate, out DateTime parsedStartDate) && DateTime.TryParse(FinishDate, out DateTime parsedFinishDate))
            {
                LegalHolidayDates legalHolidayDates = new LegalHolidayDates();

                legalHolidayDates.HolidayStartDate = parsedStartDate;
                legalHolidayDates.HolidayFinishDate = parsedFinishDate;
                legalHolidayDates.AddedByUserID = userId;
                legalHolidayDates.Name = Name;
                if (legalHolidayDates.HolidayStartDate.Equals(legalHolidayDates.HolidayFinishDate))
                {
                    legalHolidayDates.Day = GetTurkishDayName(legalHolidayDates.HolidayStartDate.DayOfWeek);
                }
                else
                {
                    legalHolidayDates.Day = GetTurkishDayName(legalHolidayDates.HolidayStartDate.DayOfWeek) + "-" + GetTurkishDayName(legalHolidayDates.HolidayFinishDate.DayOfWeek);
                }

                LDE.LegalHolidayDates.Add(legalHolidayDates);
                LDE.SaveChanges();

                return Json(new { success = true, message = "Holiday added successfully." });
            }
            else
            {
                // Handle parsing failure
            }
            return Json(new { success = false, message = "Holiday not added" });
        }

        [HttpGet]
        public string GetHolidayTableData()
        {
            var HolidayDates = LDE.LegalHolidayDates.ToList();
            var formattedHolidays = HolidayDates.Select(h => new
            {
                h.Name,
                StartDate = h.HolidayStartDate.ToString("yyyy-MM-dd"), // Format the date as yyyy-MM-dd
                FinishDate = h.HolidayFinishDate.ToString("yyyy-MM-dd")

            }).OrderBy(m => m.StartDate).ToList();


            string json = Newtonsoft.Json.JsonConvert.SerializeObject(HolidayDates);
            return json;
        }
    }
}