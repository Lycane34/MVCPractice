using Microsoft.AspNet.Identity;
using MVCPractice.Models;
using MVCPractice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MVCPractice.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagerController : Controller
    {
        LocalDBEntities LDE = new LocalDBEntities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string GetUsersTable()
        {
            var adminUserIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
            int adminUserId = int.Parse(adminUserIdClaim.Value);
            var usersTable = (from U in LDE.Users
                              join A in LDE.AnnualLeaveRequests on U.ID equals A.UserID
                              where U.AdminID == adminUserId && A.Status == 0
                              group new { U, A } by U.ID into groupedData
                              select new User_A_RViewModel
                              {
                                  ID = groupedData.FirstOrDefault().U.ID,
                                  Name = groupedData.FirstOrDefault().U.Name + " " + groupedData.FirstOrDefault().U.Surname,
                                  LatestAnnualUsed = groupedData.Max(g => g.A.GoingDate),

                              }).ToList();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(usersTable);

            return json;
        }

        [HttpPost]
        public string AnnualRequests(User_A_RViewModel USRA)
        {
            var usersTable = (from U in LDE.Users
                              join A in LDE.AnnualLeaveRequests on U.ID equals A.UserID
                              where U.ID == USRA.ID && A.Status == 0
                              select new User_A_RViewModel
                              {

                                  ID = A.UserID,
                                  Name = U.Name + " " + U.Surname,
                                  GoingDate = A.GoingDate,
                                  ReturnDate = A.ReturnDate,
                                  Description = A.Description,

                              }).ToList();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(usersTable);

            return json;
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

        public JsonResult DenyRequest(int id, string CancelDescription)
        {
            var userRequest = (from u in LDE.Users
                               join ALR in LDE.AnnualLeaveRequests on u.ID equals ALR.UserID
                               where ALR.Status == 0 && ALR.UserID == id
                               select new ConfirmRequestViewModel
                               {
                                   ID = ALR.ID,
                                   UserID = ALR.UserID,
                                   GoingDate = ALR.GoingDate,
                                   ReturnDate = ALR.ReturnDate,
                                   Description = ALR.Description,
                                   Status = ALR.Status,
                                   Days = ALR.Days,
                                   WorkDays = ALR.WorkDays,
                                   TotalAnnualDays = ALR.TotalAnnualDays,
                                   AnnualYears = ALR.AnnualYear,
                               }).ToList();

            var requestToDeny = userRequest.FirstOrDefault();





            if (requestToDeny != null)
            {
                var overlappingRequests = LDE.AnnualLeaveRequests
                    .Where(r =>
                        r.UserID == requestToDeny.UserID &&
                        (r.Status == 0 || r.Status == 1) &&
                        (r.GoingDate <= requestToDeny.ReturnDate && r.ReturnDate >= requestToDeny.GoingDate))
                    .ToList();
                var holidays = LDE.LegalHolidayDates.ToList();

                var holidayDates = holidays
                    .SelectMany(h => Enumerable.Range(0, (int)(h.HolidayFinishDate - h.HolidayStartDate).TotalDays + 1)
                    .Select(offset => h.HolidayStartDate.AddDays(offset)))
                    .ToList();
                var overlappingWorkDays = 0;

                foreach (var overlappingRequest in overlappingRequests)
                {
                    var overlapGoingDate = overlappingRequest.GoingDate > requestToDeny.GoingDate ? overlappingRequest.GoingDate : requestToDeny.GoingDate;
                    var overlapReturnDate = overlappingRequest.ReturnDate < requestToDeny.ReturnDate ? overlappingRequest.ReturnDate : requestToDeny.ReturnDate;

                    var overlapWorkDays = CalculateWeekdays(overlapGoingDate, overlapReturnDate, holidayDates);
                    overlappingWorkDays += overlapWorkDays;
                }

                requestToDeny.WorkDays -= overlappingWorkDays;

                var remainingOverlappingRequests = overlappingRequests
                    .Where(r => r.ID != requestToDeny.ID)
                    .ToList();

                if (remainingOverlappingRequests.Count == 0)
                {
                    var nonOverlappingRequests = LDE.AnnualLeaveRequests.Where(r =>
                             r.UserID == requestToDeny.UserID &&
                             r.Status != 99 &&
                             r.ID != requestToDeny.ID).ToList();

                    foreach (var nonOverlappingRequest in nonOverlappingRequests)
                    {
                        var nonOverlapWorkDays = CalculateWeekdays(nonOverlappingRequest.GoingDate, nonOverlappingRequest.ReturnDate, holidayDates);
                        nonOverlappingRequest.WorkDays = nonOverlapWorkDays;
                    }
                }
                else
                {
                    foreach (var remainingRequest in remainingOverlappingRequests)
                    {
                        var overlapGoingDate = remainingRequest.GoingDate > requestToDeny.GoingDate ? remainingRequest.GoingDate : requestToDeny.GoingDate;
                        var overlapReturnDate = remainingRequest.ReturnDate < requestToDeny.ReturnDate ? remainingRequest.ReturnDate : requestToDeny.ReturnDate;

                        var overlapWorkDays = CalculateWeekdays(overlapGoingDate, overlapReturnDate, holidayDates);
                        remainingRequest.WorkDays = overlapWorkDays;
                        if (remainingRequest.WorkDays == 1)
                        {
                            remainingRequest.WorkDays = overlapWorkDays;
                        }
                        else
                        {
                            remainingRequest.WorkDays = overlapWorkDays+1;

                        }
                        remainingRequest.TotalAnnualDays = overlapWorkDays;
                    }
                }

                var adminUserIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                int adminUserId = int.Parse(adminUserIdClaim.Value);
                requestToDeny.WorkDays = 0;


                AnnualLeaveRequests annualLeaveRequests = LDE.AnnualLeaveRequests.Find(requestToDeny.ID);

                annualLeaveRequests.Status = requestToDeny.Status = 99;
                annualLeaveRequests.TotalAnnualDays = requestToDeny.TotalAnnualDays;
                annualLeaveRequests.WorkDays = requestToDeny.WorkDays;

                annualLeaveRequests.TotalAnnualDays = requestToDeny.TotalAnnualDays;
                User_AnnualLeaves annualLeaves = new User_AnnualLeaves
                {
                    UserID = requestToDeny.UserID,
                    TypeID = 1,
                    AnnualYear = requestToDeny.AnnualYears,
                    CreateDate = DateTime.Now,
                    GoingDate = requestToDeny.GoingDate,
                    ReturnDate = requestToDeny.ReturnDate,
                    TotalDay = requestToDeny.Days,
                    TotalWorkDay = requestToDeny.WorkDays,
                    Description = requestToDeny.Description,
                    ManagerID = adminUserId,
                    ManagerDate = DateTime.Now,
                    DirectorID = adminUserId,
                    DirectorDate = DateTime.Now,
                    AdminID = adminUserId,
                    AdminDate = DateTime.Now,
                    IsCancel = true,
                    CancelMessage = CancelDescription,
                    CancelUserID = adminUserId,
                    CancelDate = DateTime.Now,
                    Status = requestToDeny.Status
                };

                LDE.User_AnnualLeaves.Add(annualLeaves);
                LDE.SaveChanges();



                return Json(new { success = true, message = "Request denied successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Request not found or cannot be denied." });
            }

        }

        [HttpPost]
        public JsonResult ConfirmRequest(int id)
        {
            var userRequest = (from u in LDE.Users
                               join ALR in LDE.AnnualLeaveRequests on u.ID equals ALR.UserID
                               where ALR.Status == 0 && ALR.UserID == id
                               select new ConfirmRequestViewModel
                               {
                                   ID = ALR.ID,
                                   UserID = ALR.UserID,
                                   GoingDate = ALR.GoingDate,
                                   ReturnDate = ALR.ReturnDate,
                                   Description = ALR.Description,
                                   Status = ALR.Status,
                                   Days = ALR.Days,
                                   WorkDays = ALR.WorkDays,
                                   AnnualYears = ALR.AnnualYear,
                                   TotalAnnualDays = ALR.TotalAnnualDays
                               }).ToList();

            var requestToConfirm = userRequest.FirstOrDefault();
            
            if (requestToConfirm != null)
            {




                

                User_AnnualLeaves annualLeaves = new User_AnnualLeaves();
                var adminUserIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                int adminUserId = int.Parse(adminUserIdClaim.Value);
                requestToConfirm.Status = 1;
                var xd = LDE.AnnualLeaveRequests.Find(requestToConfirm.ID);
                var minTotalAnnualDay = LDE.AnnualLeaveRequests.Where(m => m.UserID == id).Min(m => m.TotalAnnualDays);
                xd.Status = 1;
                minTotalAnnualDay -= requestToConfirm.WorkDays;
                xd.TotalAnnualDays = minTotalAnnualDay;
                annualLeaves.UserID = requestToConfirm.UserID;
                annualLeaves.TypeID = 1;
                annualLeaves.AnnualYear = requestToConfirm.AnnualYears;
                annualLeaves.CreateDate = DateTime.Now;
                annualLeaves.GoingDate = requestToConfirm.GoingDate;
                annualLeaves.ReturnDate = requestToConfirm.ReturnDate;
                annualLeaves.TotalDay = requestToConfirm.Days;
                annualLeaves.TotalWorkDay = requestToConfirm.WorkDays;
                annualLeaves.Description = requestToConfirm.Description;
                annualLeaves.ManagerID = adminUserId;
                annualLeaves.ManagerDate = DateTime.Now;
                annualLeaves.DirectorID = adminUserId;
                annualLeaves.DirectorDate = DateTime.Now;
                annualLeaves.AdminID = adminUserId;
                annualLeaves.AdminDate = DateTime.Now;
                annualLeaves.IsCancel = false;
                annualLeaves.CancelMessage = null;
                annualLeaves.CancelUserID = null;
                annualLeaves.CancelDate = null;
                annualLeaves.Status = requestToConfirm.Status;

                LDE.User_AnnualLeaves.Add(annualLeaves);
                LDE.SaveChanges();


                return Json(new { success = true, message = "Request confirmed successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Request not found or already confirmed." });
            }


        }
    }
}