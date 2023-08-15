﻿using MVCPractice.Models;
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
    public class UserController : Controller
    {





        Local_TestEntities1 LTE = new Local_TestEntities1();
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
            else if (LTE.Local_User.Any(u => u.Username.ToLower() == model.Username.ToLower()))
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
            else if (LTE.Local_User.Any(u => u.Phone == model.Phone))
            {
                errorMessages.Add("Bu Telefon Numarası Zaten Kayıtlı.");
            }

            if (string.IsNullOrWhiteSpace(model.EmailAddress))
            {
                errorMessages.Add("Email alanı boş olamaz.");
            }
            else if (LTE.Local_User.Any(u => u.EmailAddress.ToLower() == model.EmailAddress.ToLower()))
            {
                errorMessages.Add("Bu Email Zaten Kayıtlı.");
            }

            if (errorMessages.Count > 0)
            {
                return Json(new { success = false, messages = errorMessages });
            }


            Local_User entity = new Local_User();
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
            entity.IsWarehouse = false;
            entity.BankaKodu = 0;
            entity.Birthdate = new DateTime(2001, 12, 30);
            entity.CCSINo = "1";
            entity.CreateAt = DateTime.Now;
            entity.FirmID = 420;
            entity.JobStartDate = new DateTime(2020, 3, 28);
            entity.OwnerID = 1032;
            entity.TaskID = 3010;
            entity.TypeID = 690;
            LTE.Local_User.Add(entity);
            LTE.SaveChanges();



            return Json(new { success = true, message = model.Name + " Başarıyla Eklendi" });
        }
        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            try
            {
                var userToDelete = LTE.Local_User.Find(id);
                if (userToDelete != null)
                {
                    LTE.Local_User.Remove(userToDelete);
                    LTE.SaveChanges();

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
                var userData = LTE.Local_User.Find(id);
                if (userData != null)
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
                var existingUser = LTE.Local_User.Find(editedUser.ID);
                if (existingUser != null)
                {
                    if (LTE.Local_User.Any(u => u.EmailAddress.ToLower() == editedUser.EmailAddress.ToLower() && u.ID != editedUser.ID))
                    {
                        editErrorMessages.Add("Bu Email zaten kullanılıyor.");
                    }
                    if(LTE.Local_User.Any(u => u.Username.ToLower() == editedUser.Username.ToLower() && u.ID != editedUser.ID))
                    {
                        editErrorMessages.Add("Bu Kullanıcı Adı zaten kullanılıyor.");
                    }
                    if(LTE.Local_User.Any(u => u.Phone == editedUser.Phone && u.ID != editedUser.ID))
                    {
                        editErrorMessages.Add("Bu Telefon Numarası zaten kullanılıyor.");
                    }
                    if(editErrorMessages.Count > 0)
                    {
                        return Json(new { success = false, messages = editErrorMessages });
                    }
                    existingUser.Name = editedUser.Name;
                    existingUser.Surname = editedUser.Surname;
                    existingUser.Username = editedUser.Username;
                    existingUser.Password = editedUser.Password;
                    existingUser.Phone = editedUser.Phone;
                    existingUser.EmailAddress = editedUser.EmailAddress;

                    LTE.SaveChanges();

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
                var userData = LTE.Local_User.Find(id);
                if (userData != null)
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


        public JsonResult GetUserTableData()
        {
            var users = (from u in LTE.Local_User
                         select new User_A_RViewModel
                         {
                             ID = u.ID,
                             Name = u.Name + " " + u.Surname,
                             JobStartDate = u.JobStartDate,
                             EmailAddress = u.EmailAddress,
                             CreateAt = u.CreateAt
                         }
                         ).OrderByDescending(m => m.CreateAt).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }
    }

}