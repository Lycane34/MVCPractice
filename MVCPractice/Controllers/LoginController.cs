using MVCPractice.Models;
using MVCPractice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVCPractice.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(LoginUserViewModel loginUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", loginUserViewModel); // Return to the same view with validation errors
            }

            using (Local_TestEntities1 LTE = new Local_TestEntities1())
            {
                bool IsValidUser = LTE.Local_User.Any(user => user.EmailAddress.ToLower() ==
                     loginUserViewModel.EmailAddress.ToLower() && user.Password == loginUserViewModel.Password);
                if (IsValidUser)
                {
                    FormsAuthentication.SetAuthCookie(loginUserViewModel.EmailAddress, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "");
                    return View("Index");
                }
            }

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }
    }
}