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
    [Authorize(Roles = "User,Admin")]
    public class EmployeeController : Controller
    {
        Local_TestEntities1 LTE = new Local_TestEntities1();

        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public string AnnualDaysList()
        {
            var userIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("UserId");
            int userId = int.Parse(userIdClaim.Value);
            var user = (from U in LTE.Local_User
                        join A in LTE.Local_User_AnnualLeaveRequests on U.ID equals A.UserID
                        where U.ID == userId
                        select new User_A_RViewModel
                        {
                            Name = U.Name + " " + U.Surname

                        }).ToList();



            string json = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            return json;
        }
    }
}