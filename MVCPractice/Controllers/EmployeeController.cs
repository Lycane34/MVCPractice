using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVCPractice.Models;
using MVCPractice.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
        private int CalculateYearsWorked(DateTime hireDate, DateTime currentDate)
        {
            int years = currentDate.Year - hireDate.Year;

            if (currentDate.Month < hireDate.Month || (currentDate.Month == hireDate.Month && currentDate.Day < hireDate.Day))
            {
                years--;
            }

            return years;
        }

        private int CalculateAnnualDaysUsed(int id)
        {
            if (LDE.User_AnnualLeaves.Any(m => m.UserID == id))
            {
                var AnnualDaysUsed = LDE.User_AnnualLeaves.Where(m => m.UserID == id).Sum(m => m.TotalWorkDay);
                return AnnualDaysUsed;
            }
            else
            {
                return 0;
            }

            
        }

        private int CalculateDaysUsed(DateTime goingDate, DateTime returnDate)
        {
            TimeSpan difference = returnDate - goingDate;
            int daysDifference = (int)difference.TotalDays;
            return daysDifference;
        }

        [HttpGet]
        public string AnnualDaysList()
        {


            var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
            int userId = int.Parse(userIdClaim.Value);
            var userData = (from U in LDE.Users
                            join A in LDE.User_AnnualLeaves on U.ID equals A.UserID
                            where U.ID == userId && A.Status != 99
                            select new
                            {
                                User = U,
                                AnnualLeave = A,
                            }).ToList();

            var holidays = LDE.LegalHolidayDates.ToList();

            var holidayDates = holidays
                .SelectMany(h => Enumerable.Range(0, (int)(h.HolidayFinishDate - h.HolidayStartDate).TotalDays + 1)
                .Select(offset => h.HolidayStartDate.AddDays(offset)))
                .ToList();

            var user = userData.Select(u =>
            {
                var yearsWorked = CalculateYearsWorked(u.User.JobStartDate, DateTime.Now);
                var totalDays = CalculateWeekdays(u.AnnualLeave.GoingDate, u.AnnualLeave.ReturnDate, holidayDates);
                var annualDaysUsed = CalculateAnnualDaysUsed(u.User.ID);
                var numberOfDays = CalculateDaysUsed(u.AnnualLeave.GoingDate, u.AnnualLeave.ReturnDate);

                if (yearsWorked <= 5)
                {
                    totalDays = 14 * yearsWorked;
                }
                else
                {
                    totalDays = 14 * 5 + 20 * (yearsWorked - 5);
                }
                
                return new User_A_RViewModel
                {
                    Name = u.User.Name + " " + u.User.Surname,
                    GoingDate = u.AnnualLeave.GoingDate,
                    ReturnDate = u.AnnualLeave.ReturnDate,
                    Description = u.AnnualLeave.Description,
                    YearsWorked = yearsWorked,
                    NumberOfDays = numberOfDays +1,
                    TotalDays = totalDays,
                    AnnualDayUsed = annualDaysUsed,
                    AnnualDayLeft = totalDays - annualDaysUsed,
                };
            }).ToList();



            
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            return json;
        }

        [HttpPost]
        public JsonResult AnnualDayRequest(AnnualLeaveRequestViewModel ALRModel)
        {
            AnnualLeaveRequests ALR = new AnnualLeaveRequests();
            Users users = new Users();
            var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
            int userId = int.Parse(userIdClaim.Value);
            var errorMessages = new List<string>();
            

            if (ALRModel.RequestGoingDate == DateTime.MinValue)
            {
                errorMessages.Add("Gidiş tarihi alanı boş olamaz.");
            }
            if (ALRModel.RequestReturnDate == DateTime.MinValue)
            {
                errorMessages.Add("Dönüş tarihi alanı boş olamaz.");
            }
            if (string.IsNullOrEmpty(ALRModel.RequestDescription))
            {
                errorMessages.Add("Açıklama Alanı boş olamaz.");
            }
            if (ALRModel.RequestGoingDate > ALRModel.RequestReturnDate)
            {
                errorMessages.Add("Gidiş tarihi , dönüş tarihinden sonra olamaz.");
            }
            if (ALRModel.RequestGoingDate < DateTime.Now)
            {
                errorMessages.Add("Gidiş tarihi bugünden önce olamaz.");
            }
            if (ALRModel.RequestReturnDate < DateTime.Now)
            {
                errorMessages.Add("Dönüş tarihi bugünden önce olamaz.");
            }

            if (errorMessages.Count > 0)
            {
                return Json(new { success = false, messages = errorMessages });
            }

            var holidays = LDE.LegalHolidayDates.ToList();

            var holidayDates = holidays
                 .SelectMany(h => Enumerable.Range(0, (int)(h.HolidayFinishDate - h.HolidayStartDate).TotalDays + 1)
                 .Select(offset => h.HolidayStartDate.AddDays(offset)))
                 .ToList();

            var TotalWorkDays = CalculateWeekdays(ALRModel.RequestGoingDate, ALRModel.RequestReturnDate,holidayDates);
            var totalDays = 0;
            var yearsWorked = CalculateYearsWorked(LDE.Users.Find(userId).JobStartDate, DateTime.Now);


            if (yearsWorked <= 5)
            {
                totalDays = 14 * yearsWorked;
            }
            else
            {
                totalDays = 14 * 5 + 20 * (yearsWorked - 5);
            }
            if (totalDays < TotalWorkDays)
            {
                return Json(new { success = false, message = errorMessages });
            }

            var daysUsed = CalculateAnnualDaysUsed(userId);
            totalDays -= TotalWorkDays;

            ALR.UserID = userId;
            ALR.GoingDate = ALRModel.RequestGoingDate;
            ALR.ReturnDate = ALRModel.RequestReturnDate;
            ALR.Description = ALRModel.RequestDescription;
            ALR.AnnualYear = yearsWorked;
            ALR.TotalAnnualDays = totalDays;
            ALR.Days = (ALRModel.RequestReturnDate - ALRModel.RequestGoingDate).Days +1;
            ALR.WorkDays = TotalWorkDays;
            ALR.CreateDate = DateTime.Now;
            ALR.Status = 0;
            LDE.AnnualLeaveRequests.Add(ALR);
            LDE.SaveChanges();



            return Json(new { success = true, messages = "Başarılı" });

        }

    }
}