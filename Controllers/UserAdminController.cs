
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SupplyStuff.Models;
using SupplyStuff.ViewModels;

namespace SupplyStuff.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserAdminController : Controller

    {
        public UserAdminController()
        {
        }

        public UserAdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;

        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set { _userManager = value; }
        }


        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {

            var usersWithRoles = (from user in db.Users
                select new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Reference = user.Reference,
                    RoleNames = (from userRole in user.Roles
                        join role in db.Roles on userRole.RoleId equals role.Id
                        select role.Name).FirstOrDefault()
                }).ToList().Select(p => new UserViewModel()

            {
                Id = p.Id,
                UserName = p.Username,
                Email = p.Email,
                Name = p.Name,
                Reference = p.Reference,
                Role = p.RoleNames
            });

            return View(usersWithRoles);
        }

        public ActionResult Edit(string Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser appUser = new ApplicationUser();
            appUser = UserManager.FindById(Id);
            var role = appUser.Roles.FirstOrDefault();

            EditUserViewModel user = new EditUserViewModel
            {
                Id = appUser.Id,
                Name = appUser.Name,
                Email = appUser.Email,
                Reference = appUser.Reference,
            };
            if (role != null)
            {
                user.RoleId = role.RoleId;
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", user.RoleId).OrderBy(c => c.Text);


            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            var currentUser = manager.FindById(model.Id);
            currentUser.Name = model.Name;
            currentUser.Email = model.Email;
            currentUser.Reference = model.Reference;
            await manager.UpdateAsync(currentUser);
            var ctx = store.Context;
            ctx.SaveChanges();

            var oldrole = currentUser.Roles.FirstOrDefault();
            var name = db.Roles.Find(oldrole.RoleId).Name;
            var result = UserManager.RemoveFromRole(currentUser.Id, name);

            var arole = db.Roles.Find(model.RoleId).Name;
            var result1 = UserManager.AddToRole(currentUser.Id, arole);
            return RedirectToAction("Index");
        }
            }

}