using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVCPractice.Models;
using MVCPractice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace MVCPractice.Controllers
{
    [Authorize(Roles = "User,Admin,HR")]
    public class EmployeeController : Controller
    {
        LocalDBEntities LDE = new LocalDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        
        private int CalculateWeekdays(DateTime startDate, DateTime endDate, List<DateTime> holidays)
        {
            int weekdays = 0;
            DateTime currentDate = startDate;

            while (currentDate <= endDate)
            {
                if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                    currentDate.DayOfWeek != DayOfWeek.Sunday &&
                    !holidays.Contains(currentDate.Date))
                {
                    weekdays++;
                }
                currentDate = currentDate.AddDays(1);
            }

            return weekdays;
        }

        [HttpGet]
        public string AnnualDaysList()
        {
            

            var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
            int userId = int.Parse(userIdClaim.Value);
                var user = (from U in LDE.Users
                        join A in LDE.User_AnnualLeaves on U.ID equals A.UserID
                        where U.ID == userId
                        select new User_A_RViewModel
                        {
                            Name = U.Name + " " + U.Surname,
                            GoingDate = A.GoingDate,
                            ReturnDate = A.ReturnDate,
                            

                        }).ToList();

            var holidays = LDE.LegalHolidayDates.ToList();

            var holidayDates = holidays
                .SelectMany(h => Enumerable.Range(0, (int)(h.HolidayFinishDate - h.HolidayStartDate).TotalDays + 1)
                .Select(offset => h.HolidayStartDate.AddDays(offset)))
                .ToList();

            foreach (var u in user)
            {
                u.TotalDays = CalculateWeekdays(u.GoingDate, u.ReturnDate, holidayDates);
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            return json;
        }

        
    }
}