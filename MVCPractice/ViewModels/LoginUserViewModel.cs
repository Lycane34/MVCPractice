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

        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Şifre Giriniz.")]

        public string Password { get; set; }

    }
}