using FISApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FISApp.Controllers
{
    public class CreateController : Controller
    {
        private FISEntities db = new FISEntities();

        public ActionResult CreateEmployee()
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");
            User logUser = db.Users.Find(Session["logUserID"]);

            List<Position> listPos = db.Positions.ToList();
            listPos = listPos.OrderBy(o => o.pos_type).ToList();

            if (logUser.user_type == 2)
            {
                listPos = listPos.Where(o => o.pos_type == 3).ToList();
            }

            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (var item in listPos)
            {
                if (!item.pos_id.Equals("AD"))
                    listItems.Add(new SelectListItem { Text = item.pos_displayed, Value = item.pos_id });
            }

            ViewBag.PosList = listItems;

            int temp = db.TempUsers.ToList().Count;
            if (temp > 0 || !ModelState.IsValid)
            {
                if (ModelState.IsValid)
                    ModelState.AddModelError("", "<p style='color:red'>Please update fingerprint for id : " + db.TempUsers.Select(m => m.tempuser_id).FirstOrDefault() + " before add new user!</p>"
                        + "<a style='color:red' href=\"/Create/CleanTempUser\")\">Clear</a> or <a style='color:red' href=\"/Create/UpdateEmpImage\")\">Update</a></br>");

            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateEmployee(CreateEmployeeModel model)
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");
            int temp = db.TempUsers.ToList().Count;
            if (temp > 0 || !ModelState.IsValid)
            {
                return CreateEmployee();
            }

            try
            {
                addEmpToDB(model);
                return RedirectToAction("UpdateEmpImage", "Create");
            }
            catch (Exception ex)
            {
                return Content("Error! TODO Update Error view later");
            }
        }

        public void addEmpToDB(CreateEmployeeModel model)
        {
            Position pos = db.Positions.Find(model.posID);
            User newUser = new User();
            TempUser tempuser = new TempUser();
            string username = RemoveVietnamese(model.full_name.ToLower());

            string[] name = username.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < name.Count() - 1; i++)
            {
                name[name.Count() - 1] += name[i].First();
            }
            string namei = name[name.Count() - 1];
            int c = db.Users.Where(u => u.user_id.Contains(namei)).ToList().Count();
            newUser.user_id = (c == 0) ? namei : namei + c;

            newUser.password = Constants.defaultPassword;
            tempuser.tempuser_id = newUser.user_id;
            tempuser.descriptions = "Nothing";
            newUser.status = 1;
            newUser.user_type = pos.pos_type;
            newUser.full_name = model.full_name;
            newUser.mail = model.Email;
            newUser.DOB = model.DOB ?? DateTime.Now;
            newUser.address = model.address;
            newUser.pos_id = pos.pos_id;
            newUser.department = model.department;
            newUser.phone = model.phone;

            db.Users.Add(newUser);
            db.TempUsers.Add(tempuser);
            db.SaveChanges();
            string pathString = Path.Combine(Server.MapPath("~/Images/User"), newUser.user_id);
            Directory.CreateDirectory(pathString);
        }

        public ActionResult CreateDevice()
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");
            return View();
        }

        public ActionResult UpdateEmpImage()
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");
            if (db.TempUsers.ToList().Count == 0) return RedirectToAction("Index", "Admin");

            UpdateEmpImageModel model = new UpdateEmpImageModel();
            User user = db.Users.Find(db.TempUsers.Select(m => m.tempuser_id).FirstOrDefault());
            if (user == null) return Content("Error");
            model.avatar = String.IsNullOrEmpty(user.avatar) ? "/Images/avatar.jpg" : user.avatar;
            model.finger_image_src = "/Images/User/default.jpg";
            model.user_id = user.user_id;
            model.profile = userToProfile(user);
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateEmpImage(UpdateEmpImageModel model)
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");
            User user = db.Users.Find(db.TempUsers.Select(m => m.tempuser_id).FirstOrDefault());
            if (ModelState.IsValid)
            {
                if (user == null) return Content("Error");
                user.avatar = "/Images/avatar.jpg";
                user.finger_image = model.finger_image_src;
                if (model._avatar != null && model._avatar.ContentLength > 0)
                {
                    var fileName = String.Format("{0}-{1}.jpg", model.user_id, "avatar");
                    var path = Path.Combine(Server.MapPath(String.Format("~/Images/User/{0}", model.user_id)), fileName);
                    model._avatar.SaveAs(path);
                    user.avatar = String.Format("/Images/User/{0}/{1}", model.user_id, fileName);
                }
                db.Entry(user).State = EntityState.Modified;
                db.Entry(db.TempUsers.FirstOrDefault()).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                if (user == null) return Content("Error");
                model.finger_image_src = "/Images/User/default.jpg";
            }
            if (user == null) return Content("Error");
            model.avatar = String.IsNullOrEmpty(user.avatar) ? "/Images/avatar.jpg" : user.avatar;
            model.finger_image_src = "/Images/User/default.jpg";
            model.user_id = user.user_id;
            model.profile = userToProfile(user);
            return View(model);
        }


        [HttpPost]
        public JsonResult RequestFingerImage()
        {
            try
            {
                string userid = db.TempUsers.Select(m => m.tempuser_id).FirstOrDefault();
                var absolutePath = Server.MapPath(String.Format("~/Images/User/{0}/{1}.bmp", userid, userid));
                if (System.IO.File.Exists(absolutePath))
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return new JsonResult() { Data = new { Success = true, Path = String.Format("/Images/User/{0}/{1}.bmp", userid, userid) } };
                }
            }
            catch { }
            return new JsonResult
            {
                Data = new { Success = false },
                ContentEncoding = System.Text.Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }

        public ActionResult AddNewDevice(CreateDeviceModel model)
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");
            Device newDevice = new Device();

            List<Device> listDevice = db.Devices.ToList();
            int newID = int.Parse(listDevice.Last().device_id.Substring(1)) + 1;

            newDevice.device_id = "P" + newID;
            newDevice.device_name = model.device_name;
            newDevice.description = model.description;
            newDevice.device_status = 2;

            db.Devices.Add(newDevice);
            db.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult CleanTempUser()
        {
            TempUser tp = db.TempUsers.ToList()[0];
            User us = db.Users.Find(tp.tempuser_id);

            db.Users.Remove(us);
            db.TempUsers.Remove(tp);
            db.SaveChanges();
            return RedirectToAction("CreateEmployee", "Create");
        }

        public static string RemoveVietnamese(string str)
        {
            for (int i = 1; i < Constants.VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < Constants.VietnameseSigns[i].Length; j++)
                    str = str.Replace(Constants.VietnameseSigns[i][j], Constants.VietnameseSigns[0][i - 1]);
            }
            return str;
        }

        public Profile userToProfile(User logUser)
        {
            Profile pr = new Profile();
            pr.userID = logUser.user_id;
            pr.position = db.Positions.Find(logUser.pos_id).pos_displayed;
            pr.name = logUser.full_name;
            pr.DOB = logUser.DOB.ToShortDateString();
            pr.phone = logUser.phone;
            pr.department = logUser.department;
            pr.address = logUser.address;
            pr.email = logUser.mail;
            pr.user_type = logUser.user_type;
            pr.status = logUser.status;

            return pr;
        }
    }
}