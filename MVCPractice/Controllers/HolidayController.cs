using MVCPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPractice.Controllers
{
    [Authorize(Roles = "HR")]
    public class HolidayController : Controller
    {
        Local_TestEntities1 LTE = new Local_TestEntities1();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string GetTable()
        {
            var holidays = LTE.Local_Holiday.ToList();

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(holidays);
            return json;
        }

        public ActionResult AddHoliday()
        {
            return View();
        }
    }
}