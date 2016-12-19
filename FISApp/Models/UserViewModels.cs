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
        public string DOB { get; set; }

        [Display(Name = "Phone number")]
        public string phone { get; set; }

        [Display(Name = "Department")]
        public string department { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Status")]
        public Nullable<int> status { get; set; }

        public Nullable<int> user_type { get; set; }

        [Display(Name = "Avatar")]
        public string avatar { get; set; }

        [Display(Name = "Fingerprint Image")]
        public string finger_image { get; set; }
    }

    public class ChangePassword
    {
        [Required]
        [Display(Name = "Old password")]
        [DataType(DataType.Password)]
        public string oldPass { get; set; }

        [Display(Name = "New password")]
        [Required]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "Password must be from 6 to 18 characters!")]
        [DataType(DataType.Password)]
        public string newPass { get; set; }

        [Display(Name = "Repeat new password")]
        [Required]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "Password must be from 6 to 18 characters!")]
        [DataType(DataType.Password)]
        public string newPass2 { get; set; }
    }
}
