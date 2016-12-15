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
                    model.empListID.Add(item.user_id);
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

            model.countAttent = countAttent();
            model.countAbsent = countAbsent(model.countAttent);
            model.countDate = countDate();

            return View(model);
        }

        public string[] countDate()
        {
            string[] countDate = new string[10];
            for (int i = 0; i < 10; i++)
            {
                DateTime day = DateTime.Today.AddDays(-i);
                countDate[i] = day.Month + "." + day.Day;
            }
            return countDate;
        }

        public int[] countAttent()
        {
            int[] at = new int[10];
            List<Attent> list = db.Attents.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                Attent item = list[i];
                if (db.Users.Find(item.attent_user).user_type == 1)
                {
                    list.Remove(item);
                    i--;
                }
            }

            for (int i = 0; i < at.Length; i++)
            {
                DateTime day = DateTime.Today.AddDays(-i);
                List<Attent> list2 = new List<Attent>();
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].attent_time.ToShortDateString().Equals(day.ToShortDateString())) list2.Add(list[j]);
                }
                at[i] = list2.GroupBy(o => o.attent_user).Select(g => new { Name = g.Key, Count = g.Count() }).Count();
            }
            return at;
        }

        public int[] countAbsent(int[] at)
        {
            int[] ab = new int[10];
            int emp = db.Users.Where(o => o.user_type != 1).ToList().Count();
            for (int i = 0; i < 10; i++)
            {
                ab[i] = emp - at[i];
            }

            return ab;
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
            if (Session["logUserType"].Equals("3"))
            {
                foreach (var item in atList)
                {
                    if (!item.attent_user.Equals(Session["logUserID"])) atList.Remove(item);
                }
            }
            String csv = "ID,Name,Time,Location";
            foreach (var item in atList)
            {
                csv += Environment.NewLine + item.attent_user + "," + CreateController.RemoveVietnamese(db.Users.Find(item.attent_user).full_name) + "," + item.attent_time + "," + item.attent_device;
            }
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Report.csv");
        }

    }
}