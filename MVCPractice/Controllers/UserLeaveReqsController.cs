using MVCPractice.Models;
using MVCPractice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPractice.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserLeaveReqsController : Controller
    {   LocalDBEntities LDE = new LocalDBEntities();
        public ActionResult Index()
        {
            return View();
        }


        public string ALRUsers()
        {

            //Local_TestEntities1 LTE = new Local_TestEntities1();




            DateTime today = DateTime.Today;
            DateTime FDOTY = new DateTime(today.Year, 1, 1);
            DateTime LDOTY = new DateTime(today.Year, 12, 31);


            var oList = (from u in LDE.Users
                         join a in LDE.User_AnnualLeaves on u.ID equals a.UserID
                         group new { u, a } by a.UserID into Grup1
                         where Grup1.Any(m => m.a.GoingDate <= LDOTY &&
                         m.a.GoingDate >= FDOTY)
                         select new User_A_RViewModel
                         {

                             ID = Grup1.FirstOrDefault().u.ID,
                             Name = Grup1.FirstOrDefault().u.Name + " " + Grup1.FirstOrDefault().u.Surname,
                             JobStartDate = Grup1.FirstOrDefault().u.JobStartDate,
                             AnnualDayLeft = ((14 * (DateTime.Now.Year - Grup1.FirstOrDefault().u.JobStartDate.Year)) - (Grup1.Where(m => m.a.GoingDate >= FDOTY && m.a.GoingDate <= LDOTY).Sum(mbox => mbox.a.TotalDay))),
                             AnnualDayUsed = Grup1.Where(m => m.a.GoingDate >= FDOTY && m.a.GoingDate <= LDOTY).Sum(mbox => mbox.a.TotalDay),
                             TotalAnnualDayUsed = Grup1.Where(m => m.a.GoingDate >= new DateTime(DateTime.MinValue.Ticks) && m.a.GoingDate <= new DateTime(DateTime.MaxValue.Ticks)).Sum(mbox => mbox.a.TotalDay),
                             YearsWorked = DateTime.Now.Year - Grup1.FirstOrDefault().u.JobStartDate.Year,
                         }).ToList();





            string json = Newtonsoft.Json.JsonConvert.SerializeObject(oList);
            return json;
        }

        public ActionResult Details(int? id)
        {


            //Local_TestEntities1 LTE = new Local_TestEntities1();

            var oTest = (from u in LDE.Users
                         join a in LDE.User_AnnualLeaves on u.ID equals a.UserID
                         where a.UserID == id
                         select new User_A_RViewModel
                         {
                             Name = u.Name + " " + u.Surname,
                             TypeID = a.TypeID,
                             GoingDate = a.GoingDate,
                             ReturnDate = a.ReturnDate,
                             Description = a.Description,
                             Status = a.Status,
                         }).OrderByDescending(m => m.GoingDate).ToList();

         
            ViewBag.oTest = oTest;


            return View();
        }

        [HttpPost]
        public string DetailsPopUp(int? id)
        {
            //Local_TestEntities1 LTE = new Local_TestEntities1();

            var oTest = (from u in LDE.Users
                         join a in LDE.User_AnnualLeaves on u.ID equals a.UserID
                         where a.UserID == id
                         select new UserDetailsViewModel
                         {
                             Name = u.Name + " " + u.Surname,
                             TypeID = a.TypeID,
                             GoingDate = a.GoingDate,
                             ReturnDate = a.ReturnDate,
                             Description = a.Description,
                             Status = a.Status,
                         }).OrderByDescending(m => m.GoingDate).ToList();


            string json = Newtonsoft.Json.JsonConvert.SerializeObject(oTest);
            return json;
        }
    }
}