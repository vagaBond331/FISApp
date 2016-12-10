using FISApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FISApp.Controllers
{
    public class HttpRequestController : Controller
    {
        private FISEntities db = new FISEntities();

        [HttpPost]
        public string PiSendImage(HttpPostedFileBase file, string deviceID)
        {
            if (!String.IsNullOrEmpty(deviceID))
            {
                Device device = db.Devices.Find(deviceID);
                if (device == null || device.device_status == 1) return "reg_fail";
            }
            if (file != null && file.ContentLength > 0)
            {
                string userid = db.TempUsers.Select(m => m.tempuser_id).FirstOrDefault();
                User user = db.Users.Find(userid);
                if (String.IsNullOrEmpty(userid) || db.TempUsers.ToList().Count > 1 || user == null)
                {
                    return "reg_fail";
                }
                var fileName = string.Format("{0}.bmp", userid);
                var path = Path.Combine(Server.MapPath(string.Format("~/Images/User/{0}", userid)), fileName);
                file.SaveAs(path);
                return string.Format("Oke_{0}_{1}", userid, CreateController.RemoveVietnamese(user.full_name));
            }
            return "reg_fail";
        }

        [HttpPost]
        public string PiSendLOG(String userID, String deviceID)
        {
            if (!String.IsNullOrEmpty(deviceID))
            {
                Device device = db.Devices.Find(deviceID);
                if (device == null || device.device_status == 1) return "log_fail";
            }
            try
            {
                //Log
                Attent atten = new Attent()
                {
                    attent_device = deviceID,
                    attent_user = userID,
                    attent_time = DateTime.Now,
                    attent_type = 0
                };
                db.Attents.Add(atten);
                db.SaveChanges();
            }
            catch
            {
                return "log_fail";
            }

            //save LOG to DB
            return "OK";
            //return "LOG_fail";
        }


        [HttpPost]
        public string PiSendBackUp(String one, String tow, String three, String deviceID)
        {
            if (!String.IsNullOrEmpty(deviceID))
            {
                Device device = db.Devices.Find(deviceID);
                if (device == null || device.device_status == 1) return "reg_fail";
            }
            try
            {
                //TODO - add two table
            }
            catch
            {

            }
            //save LOG to DB
            return "OK";
            //return "LOG_fail";
        }
    }
}