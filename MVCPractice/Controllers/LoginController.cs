using MVCPractice.Models;
using MVCPractice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult Login(LoginUserViewModel loginUserViewModel)
        {
                
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
                    ModelState.AddModelError("", "invalid Email or Password");
                    return View();
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