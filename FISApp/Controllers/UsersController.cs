using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FISApp.Models;

namespace FISApp.Controllers
{
    public class UsersController : Controller
    {
        private FISEntities db = new FISEntities();

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult CheckLogin(LoginUser logUser)
        {
            List<User> list = db.Users.ToList();

            int ck = 0;
            User us = new User();

            foreach (var item in list)
            {
                if (item.user_id.Equals(logUser.UserID) && item.password.Equals(logUser.Password))
                {
                    us = item;
                    ck = us.user_type;
                    break;
                }
            }

            if (us.status == 0) return RedirectToAction("Login", "Users");
            else
            {
                Session["logUserID"] = us.user_id;
                Session["logUserType"] = us.user_type;
                Session["logUserName"] = us.full_name;
                if (ck == 1 || ck == 2) return RedirectToAction("Index", "Admin");
                else return RedirectToAction("Index", "Employee");
            }
        }

        public ActionResult Logout()
        {
            Session["logUserID"] = null;
            Session["logUserType"] = null;
            Session["logUserName"] = null;
            return RedirectToAction("Login", "Users");
        }

        public ActionResult ChangePassword()
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword model)
        {
            ViewBag.Messages = "";
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");

            string oldPass = db.Users.Find(Session["logUserID"]).password;

            if (model.newPass.Length < 6 || model.newPass.Length > 18 || model.newPass2.Length < 6 || model.newPass2.Length > 18)
            {
                return View(model);
            }

            if (model.oldPass.Equals(oldPass))
            {
                if (model.newPass.Equals(model.newPass2))
                {
                    db.Users.Find(Session["logUserID"]).password = model.newPass;
                    db.SaveChanges();
                    ViewBag.Messages = "Change success!";
                    return View(model);
                }
                else
                {
                    ViewBag.Messages = "Two new passwords are not same!";
                    return View(model);
                }
            }
            else
            {
                ViewBag.Messages = "Old password is wrong!";
                return View(model);
            }
        }

        // GET: Users
        public ActionResult Profile(string userID)
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");
            User logUser = new User();
            if (userID == null) logUser = db.Users.Find(Session["logUserID"]);
            else logUser = db.Users.Find(userID);

            return View(userToProfile(logUser));
        }

        public ActionResult ListProfile()
        {
            List<User> listUser = db.Users.Where(u => u.user_type != 1).ToList();
            List<Profile> listProfile = new List<Profile>();

            foreach (var item in listUser)
            {
                listProfile.Add(userToProfile(item));
            }

            return View(listProfile);
        }

        public ActionResult ChangeStatus(string id)
        {
            if (db.Users.Find(id).status == 1)
            {
                db.Users.Find(id).status = 0;
                foreach (var item in db.Attents.Where(o => o.attent_user.Equals(id)))
                {
                    item.attent_type = 0;
                }
            }
            else
            {
                db.Users.Find(id).status = 1;
                foreach (var item in db.Attents.Where(o => o.attent_user.Equals(id)))
                {
                    item.attent_type = 1;
                }
            }

            db.SaveChanges();
            return RedirectToAction("ListProfile", "Users");
        }

        public ActionResult EditProfile(string userID)
        {
            User us = db.Users.Find(userID);
            CreateEmployeeModel model = new CreateEmployeeModel();
            model.userID = us.user_id;
            model.full_name = us.full_name;
            model.Email = us.mail;
            model.DOB = us.DOB;
            model.address = us.address;
            model.department = us.department;
            model.phone = us.phone;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditProfile(CreateEmployeeModel mod, string id)
        {
            User us = db.Users.Find(id);
            us.full_name = mod.full_name;
            us.mail = mod.Email;
            us.DOB = mod.DOB ?? DateTime.Now;
            us.address = mod.address;
            us.department = mod.department;
            us.phone = mod.phone;
            db.SaveChanges();

            return RedirectToAction("Profile", "Users", new { userID = id });
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
            pr.avatar = String.IsNullOrEmpty(logUser.avatar) ? "/Images/avatar.jpg" : logUser.avatar;

            return pr;
        }
    }
}
