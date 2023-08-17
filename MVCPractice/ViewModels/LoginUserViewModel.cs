using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCPractice.ViewModels
{
    public class LoginUserViewModel
    {

        [Required(ErrorMessage = "Email Giriniz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi veya şifre giriniz.")]


        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Şifre Giriniz.")]
        [DataType(DataType.Password)]


        public string Password { get; set; }

        public int TypeID { get; set; }

    }
}