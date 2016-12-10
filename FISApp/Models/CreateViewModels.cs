using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web;

namespace FISApp.Models
{
    public class CreateEmployeeModel
    {
        //[Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Username")]
        public string username { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Display(Name = "Fullname")]
        [Required]
        public string full_name { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public Nullable<System.DateTime> DOB { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "Position")]
        public string posID { get; set; }

        [Display(Name = "Department")]
        public string department { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Number phone")]
        public string phone { get; set; }
    }

    public class UpdateEmpImageModel
    {
        public HttpPostedFileBase _avatar { get; set; }

        [Display(Name = "Fingerprint Image")]
        public string finger_image_src { get; set; }

        [Display(Name = "Avatar")]
        public string avatar { get; set; }

        [Required]
        public string user_id { get; set; }
    }

    public class CreateDeviceModel
    {
        [Required]
        [Display(Name = "Device name")]
        public string device_name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string description { get; set; }
    }
}
