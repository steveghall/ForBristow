using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SupplyStuff.Models;

namespace SupplyStuff.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();




        public ActionResult Index()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            // Get the current logged in User and look up the user in ASP.NET Identity
            var currentUser = manager.FindById(User.Identity.GetUserId());

            ViewBag.name = currentUser.Name;



            var userId = User.Identity.GetUserName();
            var history = db.Orders.OrderByDescending(u => u.OrderDate).Where(u => u.UserId == userId).Include(d => d.OrderDetails);
            return View(history);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult About(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = db.Orders.Include(d => d.OrderDetails).SingleOrDefault(x => x.OrderId == id);
            
            //Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ViewBag.Name = manager.FindByEmail(order.UserId).Name;
            ViewBag.Reference = manager.FindByEmail(order.UserId).Reference;
            return View(order);
            
            }
            //public ActionResult About()
            //
            //{
            //    ViewBag.Message = "Your application description page.";

            //    return View();
            //}

            //public ActionResult Contact()
            //{
            //    ViewBag.Message = "Your contact page.";

            //    return View();
            //}
        }
}