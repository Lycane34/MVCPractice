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
    [Authorize(Roles ="Admin")]
    public class ManagerController : Controller
    {
        LocalDBEntities LDE = new LocalDBEntities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string GetUsersTable() {
            var adminUserIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
            int adminUserId = int.Parse(adminUserIdClaim.Value);
            var usersTable = (from U in LDE.Users 
                             join A in LDE.AnnualLeaveRequests on U.ID equals A.UserID
                             where U.AdminID == adminUserId && A.Status == 0
                             group new { U, A} by U.ID into groupedData
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
        public string AnnualRequests(int id)
        {
            var usersTable = (from U in LDE.Users
                              join A in LDE.AnnualLeaveRequests on U.ID equals A.UserID
                              where U.ID == id && A.Status == 0
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

        public JsonResult DenyRequest(int id,string CancelDescription)
        {
            var userRequest = (from u in LDE.Users
                              join ALR in LDE.AnnualLeaveRequests on u.ID equals ALR.UserID
                              where ALR.Status == 0
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
                              }).ToList();

            var requestToConfirm = userRequest.FirstOrDefault();

            if (requestToConfirm != null)
            {
                User_AnnualLeaves annualLeaves = new User_AnnualLeaves();
                var adminUserIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                int adminUserId = int.Parse(adminUserIdClaim.Value);
                requestToConfirm.Status = 99;
                var xd = LDE.AnnualLeaveRequests.Find(requestToConfirm.ID);
                xd.Status = 99;
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
                annualLeaves.IsCancel = true;
                annualLeaves.CancelMessage = CancelDescription;
                annualLeaves.CancelUserID = adminUserId;
                annualLeaves.CancelDate = DateTime.Now;
                annualLeaves.Status = requestToConfirm.Status;

                LDE.User_AnnualLeaves.Add(annualLeaves);
                LDE.SaveChanges();
                

                return Json(new { success = true, message = "Request denied successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Request not found or already confirmed." });
            }
        }


        [HttpPost]
        public JsonResult ConfirmRequest(int id)
        {
            var userRequest = (from u in LDE.Users
                              join ALR in LDE.AnnualLeaveRequests on u.ID equals ALR.UserID
                              where ALR.Status == 0
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
                              }).ToList();

            var requestToConfirm = userRequest.FirstOrDefault();

            if (requestToConfirm != null)
            {
                User_AnnualLeaves annualLeaves = new User_AnnualLeaves();
                var adminUserIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
                int adminUserId = int.Parse(adminUserIdClaim.Value);
                requestToConfirm.Status = 1;
                var xd = LDE.AnnualLeaveRequests.Find(requestToConfirm.ID);
                xd.Status = 1;
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