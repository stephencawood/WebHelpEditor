using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Web.Security;

namespace WebHelpEditor.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        [Authorize]
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Status()
        {
            // TODO this should be behind security on the production box
            //if (FileEditorUser.Current == null)
            //    return RedirectToAction("Index", "Account");
            
            return View();
        }

        public ActionResult Users()
        {
            //if (FileEditorUser.Current == null || !FileEditorUser.Current.IsAdmin)
            //    return RedirectToAction("Index", "Account");

            //ViewBag.Role = "Admin";
            
            //var model = Membership.GetAllUsers();

            //return View(model);

            //using (var ctx = new UsersContext())
            //{
            //    return View(ctx.UserProfiles.ToList());
            //}

            return View(Membership.GetAllUsers());
        }

        public ActionResult EditUser(string id)
        {
            //if (FileEditorUser.Current == null || !FileEditorUser.Current.IsAdmin)
            //    return RedirectToAction("Index", "Account");
            //ViewModels.EditUserModel model = new ViewModels.EditUserModel();

            MembershipUser aspUser = Membership.GetUser(id);
            //model.UserName = aspUser.UserName;
            //model.Comment = aspUser.Comment;
            //model.AccountLocked = aspUser.IsLockedOut;

            //List<UserInfo> FileEditorUserInfo = DB.GetUserInfoByProfileID(Convert.ToInt32(aspUser.Comment));
            //model.UserNameFileEditor = FileEditorUserInfo[0].Username;
            //model.LastName = FileEditorUserInfo[0].LastName;

            //return View(model);
            return View();
        }

        public ActionResult Unlock(string userName)
        {
            MembershipUser aspUser = Membership.GetUser(userName);
            aspUser.UnlockUser();
            return RedirectToAction("EditUser", "Admin", new { id = userName });
            //return View("~/Views/Admin/EditUser/" + userName);
        }

        public ActionResult Lock(string userName)
        {
            MembershipUser aspUser = Membership.GetUser(userName);

            // TODO do it in SQL?
            //SqlMembershipProvider sqlUser = aspUser;
            //aspUser. = true;
            return View();
        }

        public ActionResult DeleteAspUser(string userName)
        {
            //MembershipUser aspUser = Membership.GetUser(userName);
            
            Membership.DeleteUser(userName);
            return View("~/Views/Admin/Users.cshtml");
        }
    }
}
