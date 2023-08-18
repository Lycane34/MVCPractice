using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVCPractice.Models;
using MVCPractice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

                var validUser = LTE.Local_User.FirstOrDefault(user =>
                user.EmailAddress.ToLower() == loginUserViewModel.EmailAddress.ToLower() &&
                user.Password == loginUserViewModel.Password);
                if (validUser != null)
                {
                    loginUserViewModel.TypeID = validUser.TypeID;

                    var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);

                    identity.AddClaim(new Claim("UserId", validUser.ID.ToString()));
                    if (loginUserViewModel.TypeID == 1)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
                    }
                    else if (loginUserViewModel.TypeID == 2)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                    }
                    else if(loginUserViewModel.TypeID == 3)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, "HR"));
                    }
                    var authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignIn(new AuthenticationProperties
                    {

                        IsPersistent = false
                    }, identity);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", " Email veya şifre yanlış");
                    return View("Index");
                }
            }

        }


        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Index", "Login");
        }

    }
}
