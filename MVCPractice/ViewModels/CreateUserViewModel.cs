using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCPractice.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "{0} alanı gereklidir.")]
        public int ID { get; set; }
        [Required(ErrorMessage = "{0} alanı gereklidir.")]

        public string Username { get; set; }
        [Required(ErrorMessage = "{0} alanı gereklidir.")]

        public string Password { get; set; }
        [Required(ErrorMessage = "{0} alanı gereklidir.")]

        public string Name { get; set; }
        [Required(ErrorMessage = "{0} alanı gereklidir.")]

        public string Surname { get; set; }

        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "{0} alanı gereklidir.")]

        public string Phone { get; set; }

        public int SelectedAdminID { get; set; }

    }
}
