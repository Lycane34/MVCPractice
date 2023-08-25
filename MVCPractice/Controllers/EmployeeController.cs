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





        private string CalculateOverallStatus(User_A_RViewModel userViewModels)
        {
            bool hasPendingRequests = userViewModels.Status.Equals(0);
            bool hasApprovedRequests = userViewModels.Status.Equals(1);
            bool hasRejectedRequests = userViewModels.Status.Equals(99);

            if (hasPendingRequests)
            {
                return "Onay Bekliyor";
            }
            else if (hasApprovedRequests && !hasPendingRequests)
            {
                return "Onaylandı";
            }
            else if (hasRejectedRequests && !hasPendingRequests && !hasApprovedRequests)
            {
                return "Reddedildi";
            }

            return string.Empty;
        }

        [HttpGet]
        public string AnnualDaysList()
        {


            var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
            int userId = int.Parse(userIdClaim.Value);
            var userData = (from U in LDE.Users
                            join A in LDE.AnnualLeaveRequests on U.ID equals A.UserID
                            where U.ID == userId
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
                int totalDays;
                var totalWorkDays = CalculateWeekdays(u.AnnualLeave.GoingDate, u.AnnualLeave.ReturnDate, holidayDates);


                var StatusMsg = string.Empty;


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
                    TotalWorkDays = u.AnnualLeave.WorkDays,
                    TotalDays = u.AnnualLeave.TotalAnnualDays,
                    Status = u.AnnualLeave.Status,
                    StatusMsg = StatusMsg,
                };
            }).ToList();


            foreach (var userViewModel in user)
            {
                userViewModel.StatusMsg = CalculateOverallStatus(userViewModel);
            }


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


            var TotalWorkDays = CalculateWeekdays(ALRModel.RequestGoingDate, ALRModel.RequestReturnDate, holidayDates);
            var yearsWorked = CalculateYearsWorked(LDE.Users.Find(userId).JobStartDate, DateTime.Now);

            var overlappingRequests = LDE.AnnualLeaveRequests
                .Where(r =>
                    r.UserID == userId &&
                    (r.Status == 0 || r.Status == 1 || r.Status == 99) &&
                    (r.GoingDate <= ALRModel.RequestReturnDate && r.ReturnDate >= ALRModel.RequestGoingDate))
                .ToList();

            var overlappingWorkDays = overlappingRequests.Sum(r =>
            {
                if (r.Status == 99)
                {
                    return 0;
                }
                else if (r.GoingDate >= ALRModel.RequestGoingDate && r.ReturnDate <= ALRModel.RequestReturnDate)
                {
                    return r.WorkDays;
                }
                else if (r.GoingDate >= ALRModel.RequestGoingDate)
                {
                    return CalculateWeekdays(r.GoingDate, ALRModel.RequestReturnDate, holidayDates);
                }
                else if (r.ReturnDate <= ALRModel.RequestReturnDate)
                {
                    return CalculateWeekdays(ALRModel.RequestGoingDate, r.ReturnDate, holidayDates);
                }
                else
                {
                    return 0;
                }
            });

            var totalWorkDaysExcludingOverlapping = TotalWorkDays - overlappingWorkDays;

            if (totalWorkDaysExcludingOverlapping < 0)
            {
                totalWorkDaysExcludingOverlapping = 0;
            }

            var userAnnualLeaveRequests = LDE.AnnualLeaveRequests.Where(m => m.UserID == userId);



            if (LDE.AnnualLeaveRequests.Where(m => m.UserID == userId).Any()== false)
            {
                if (yearsWorked <= 5)
                {
                    ALR.TotalAnnualDays = 14 * yearsWorked;
                }
                else
                {
                    ALR.TotalAnnualDays = 14 * 5 + 20 * (yearsWorked - 5);
                }

            }
            else
            {
                ALR.TotalAnnualDays = LDE.AnnualLeaveRequests.Select(m => m.TotalAnnualDays).Min();

            }

           

            if (ALR.TotalAnnualDays < TotalWorkDays)
            {
                return Json(new { success = false, message = errorMessages });
            }



            ALR.UserID = userId;
            ALR.GoingDate = ALRModel.RequestGoingDate;
            ALR.ReturnDate = ALRModel.RequestReturnDate;
            ALR.Description = ALRModel.RequestDescription;
            ALR.AnnualYear = yearsWorked;
            ALR.Days = (ALRModel.RequestReturnDate - ALRModel.RequestGoingDate).Days + 1;
            ALR.WorkDays = totalWorkDaysExcludingOverlapping;
            ALR.CreateDate = DateTime.Now;
            ALR.Status = 0;
            LDE.AnnualLeaveRequests.Add(ALR);
            LDE.SaveChanges();



            return Json(new { success = true, messages = "Başarılı" });

        }

    }
}