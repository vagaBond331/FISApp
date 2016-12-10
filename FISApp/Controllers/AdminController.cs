using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FISApp.Models;

namespace FISApp.Controllers
{
    public class AdminController : Controller
    {
        private FISEntities db = new FISEntities();

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");

            AdminViewModels model = new AdminViewModels();
            model.logUser = db.Users.Find(Session["logUserID"]);
            foreach (var item in db.Users)
            {
                if (item.user_type != 1)
                {
                    model.empListName.Add(item.full_name);
                    model.monthAttend.Add(checkAttend(item, DateTime.Now.Month));
                }
            }
            model.listDevice = db.Devices.ToList();

            List<Attent> atList = db.Attents.OrderBy(t => t.attent_time).ToList();
            atList = Enumerable.Reverse(atList).Take(5).ToList();

            foreach (var item in atList)
            {
                AttendViewModel at = new AttendViewModel();
                User us = db.Users.Find(item.attent_user);
                at.user_id = item.attent_user;
                at.fullname = us.full_name;
                at.log_time = item.attent_time;
                at.location = db.Devices.Find(item.attent_device).description;
                model.listAttent.Add(at);
            }

            return View(model);
        }

        public bool[] checkAttend(User us, int month)
        {
            //listMonth is attend of user for a month (31 days)
            //1st == listMonth[1]
            bool[] listMonth = new bool[DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + 1];

            List<Attent> list = db.Attents.Where(o => o.attent_user.Equals(us.user_id)).ToList();

            //save to bool[] 
            for (int i = 0; i < list.Count; i++)
            {
                int day = list[i].attent_time.Day;
                if (month == list[i].attent_time.Month)
                {
                    listMonth[day] = true;
                }
            }
            return listMonth;
        }

        public ActionResult activeDevice(string item_id)
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");

            Device de = db.Devices.Find(item_id);
            if (de.device_status == 1) de.device_status = 2;
            else de.device_status = 1;

            //when got error on update
            db.Entry(de).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        public ActionResult ExportLOGPage()
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");
            return View();
        }
        public FileContentResult ExportCSV(ExportLOGModel model)
        {
            List<Attent> atList = db.Attents.Where(t => (t.attent_time >= model.startDate && t.attent_time <= model.endDate)).ToList();
            String csv = "ID,Name,Time,Location";
            foreach (var item in atList)
            {
                csv += Environment.NewLine + item.attent_user + "," + CreateController.RemoveVietnamese(db.Users.Find(item.attent_user).full_name) + "," + item.attent_time + "," + item.attent_device;
            }
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Report.csv");
        }

    }
}