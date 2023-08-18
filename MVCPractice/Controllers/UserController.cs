using MVCPractice.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MVCPractice.ViewModels;
using System.Collections;

namespace MVCPractice.Controllers
{
    [Authorize(Roles = "Admin")]
    [HandleError]
    public class UserController : Controller
    {

        LocalDBEntities LDE = new LocalDBEntities();
        //[AllowAnonymous]
        public ActionResult Index()
        {


            return View();
        }

        [HttpPost]
        public JsonResult UserAdd(CreateUserViewModel model)
        {
            var errorMessages = new List<string>();


            if (string.IsNullOrWhiteSpace(model.Name))
            {
                errorMessages.Add("İsim alanı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(model.Surname))
            {
                errorMessages.Add("Soyisim alanı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(model.Username))
            {
                errorMessages.Add("Kullanıcı Adı alanı boş olamaz.");
            }
            else if (LDE.Users.Any(u => u.Username.ToLower() == model.Username.ToLower()))
            {
                errorMessages.Add("Bu Kullanıcı Adı Zaten kayıtlı.");
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                errorMessages.Add("Şifre alanı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(model.Phone))
            {
                errorMessages.Add("Telefon alanı boş olamaz.");
            }
            else if (LDE.Users.Any(u => u.Phone == model.Phone))
            {
                errorMessages.Add("Bu Telefon Numarası Zaten Kayıtlı.");
            }

            if (string.IsNullOrWhiteSpace(model.EmailAddress))
            {
                errorMessages.Add("Email alanı boş olamaz.");
            }
            else if (LDE.Users.Any(u => u.EmailAddress.ToLower() == model.EmailAddress.ToLower()))
            {
                errorMessages.Add("Bu Email Zaten Kayıtlı.");
            }

            if (errorMessages.Count > 0)
            {
                return Json(new { success = false, messages = errorMessages });
            }


            Users entity = new Users();
            entity.EmailAddress = model.EmailAddress;
            entity.Phone = model.Phone;
            entity.Surname = model.Surname;
            entity.Name = model.Name;
            entity.Username = model.Username;
            entity.Password = model.Password;
            entity.SubeKod = 0;
            entity.AccountNumber = "0";
            entity.IdentityNumber = "12345678910";
            entity.IsPersonel = true;
            entity.IsActive = true;
            entity.IsWarehouse = false;
            entity.BankaKodu = 0;
            entity.Birthdate = new DateTime(2001, 12, 30);
            entity.CCSINo = "1";
            entity.CreateAt = DateTime.Now;
            entity.FirmID = 420;
            entity.JobStartDate = new DateTime(2020, 3, 28);
            entity.OwnerID = 1032;
            entity.TaskID = 3010;
            entity.TypeID = 1;
            LDE.Users.Add(entity);
            LDE.SaveChanges();



            return Json(new { success = true, message = model.Name + " Başarıyla Eklendi" });
        }
        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            try
            {
                var userToDelete = LDE.Users.Find(id);

                if (userToDelete != null && userToDelete.IsActive != false)
                {
                    userToDelete.IsActive = false;
                    LDE.SaveChanges();

                    return Json(new { success = true, message = "User deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpGet]
        public JsonResult GetUserData(int id)
        {
            try
            {
                var userData = LDE.Users.Find(id);
                if (userData != null && userData.IsActive != false)
                {
                    return Json(new { success = true, data = userData }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "User not found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateUser(CreateUserViewModel editedUser)
        {
            var editErrorMessages = new List<string>();
            try
            {
                var existingUser = LDE.Users.Find(editedUser.ID);
                if (existingUser != null && existingUser.IsActive != false)
                {
                    if (LDE.Users.Any(u => u.EmailAddress.ToLower() == editedUser.EmailAddress.ToLower() && u.ID != editedUser.ID))
                    {
                        editErrorMessages.Add("Bu Email zaten kullanılıyor.");
                    }
                    if (LDE.Users.Any(u => u.Username.ToLower() == editedUser.Username.ToLower() && u.ID != editedUser.ID))
                    {
                        editErrorMessages.Add("Bu Kullanıcı Adı zaten kullanılıyor.");
                    }
                    if (LDE.Users.Any(u => u.Phone == editedUser.Phone && u.ID != editedUser.ID))
                    {
                        editErrorMessages.Add("Bu Telefon Numarası zaten kullanılıyor.");
                    }
                    if (editErrorMessages.Count > 0)
                    {
                        return Json(new { success = false, messages = editErrorMessages });
                    }
                    existingUser.Name = editedUser.Name;
                    existingUser.Surname = editedUser.Surname;
                    existingUser.Username = editedUser.Username;
                    existingUser.Password = editedUser.Password;
                    existingUser.Phone = editedUser.Phone;
                    existingUser.EmailAddress = editedUser.EmailAddress;

                    LDE.SaveChanges();

                    return Json(new { success = true, message = "User updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpPost]
        public JsonResult GetUserDetails(int id)
        {
            try
            {
                var userData = LDE.Users.Find(id);
                if (userData != null && userData.IsActive != false)
                {
                    return Json(new { success = true, data = userData });
                }
                else
                {
                    return Json(new { success = false, message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public string GetUserTableData()
        {
            var users = (from u in LDE.Users
                         where u.IsActive == true
                         select new User_A_RViewModel
                         {
                             ID = u.ID,
                             Name = u.Name + " " + u.Surname,
                             JobStartDate = u.JobStartDate,
                             EmailAddress = u.EmailAddress,
                             CreateAt = u.CreateAt
                         }
                         ).OrderByDescending(m => m.CreateAt).ToList();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(users);
            return json;
        }
    }

}
