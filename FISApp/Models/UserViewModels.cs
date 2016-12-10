using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace FISApp.Models
{
    public class LoginUser
    {
        [Required]
        [Display(Name = "Username")]
        [EmailAddress]
        public string UserID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class Profile
    {
        [Display(Name = "ID")]
        public string userID { get; set; }

        [Display(Name = "Position")]
        public string position { get; set; }

        [Display(Name = "Full name")]
        public string name { get; set; }

        [Display(Name = "DOB")]
        public Nullable<System.DateTime> DOB { get; set; }

        [Display(Name = "Phone number")]
        public string phone { get; set; }

        [Display(Name = "Department")]
        public string department { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        public Nullable<int> user_type { get; set; }

        [Display(Name = "Avatar")]
        public string avatar { get; set; }

        [Display(Name = "Fingerprint Image")]
        public string finger_image { get; set; }

    }
}
