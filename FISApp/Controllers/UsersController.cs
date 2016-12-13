﻿using System;
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

            if (ck == 0) return RedirectToAction("Login", "Users");
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

        public ActionResult ConfirmPassword(ChangePassword model)
        {
            if (Session["logUserID"] == null) return RedirectToAction("Logout", "Users");

            string oldPass = db.Users.Find(Session["logUserID"]).password;
            if (model.oldPass.Equals(oldPass))
            {
                if (model.newPass.Equals(model.newPass2))
                {
                    db.Users.Find(Session["logUserID"]).password = model.newPass;
                    db.SaveChanges();
                    return RedirectToAction("ChangePassword", "Users");
                }
                else
                {
                    //TODO 2 new pass not same
                    return RedirectToAction("ChangePassword", "Users");
                }
            }
            else
            {
                //TODO wrong old pass
                return RedirectToAction("ChangePassword", "Users");
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

        public Profile userToProfile(User logUser)
        {
            Profile pr = new Profile();
            pr.userID = logUser.user_id;
            pr.position = db.Positions.Find(logUser.pos_id).pos_displayed;
            pr.name = logUser.full_name;
            pr.DOB = logUser.DOB;
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