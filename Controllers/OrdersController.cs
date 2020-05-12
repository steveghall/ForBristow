using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SupplyStuff.Models;
using SupplyStuff.ViewModels;
using Postal;
using WebGrease;

namespace SupplyStuff.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        [Authorize (Roles = "Admin, Manager")]
        public ActionResult Index()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<ManageRequestsViewModel> orderlist = new List<ManageRequestsViewModel>();
            var orders = db.Orders.Include(d => d.OrderDetails).ToList();

            foreach (var item in orders)
            {
                orderlist.Add( new ManageRequestsViewModel()
                {
                    OrderId = item.OrderId,
                    Name = manager.FindByEmail(item.UserId).Name,
                    Orderdate = item.OrderDate,
                    Quantity = item.OrderDetails.Count,
                    Status = item.Complete
                });
            }
            return View(orderlist);
        }

        // GET: Orders/Details/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Details(int? id)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = db.Orders.Include(d => d.OrderDetails).SingleOrDefault(x => x.OrderId == id);
            ViewBag.Name = manager.FindByEmail(order.UserId).Name;
            ViewBag.Reference = manager.FindByEmail(order.UserId).Reference;

            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            List<OrderDetail> cart = (List<OrderDetail>) Session["cart"];
            string userName = User.Identity.GetUserName() ?? "";
            ViewBag.Name = manager.FindByEmail(userName).Name;
            ViewBag.Reference = manager.FindByEmail(userName).Reference;

            CreateOrderViewModel newOrder = new CreateOrderViewModel
            {
                OrderDate = DateTime.Today,
                UserId = userName,
                Instructions = ""
            };
            if (cart != null)
            {
                List<OrderDetailViewModel> cartItems = new List<OrderDetailViewModel>();
                foreach (var item in cart)
                {
                    cartItems.Add(new OrderDetailViewModel()
                    {
                        ItemId = item.ItemId,
                        Description = db.Items.Find(item.ItemId).Description,
                        Quantity = item.Quantity
                    });
                }
                newOrder.Items = cartItems;
            }
            return View(newOrder);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http;
        // s://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId, OrderDate, Instructions")] CreateOrderViewModel order)
        {
            if (ModelState.IsValid)
            {
                Order newOrder = new Order
                {
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    Complete = false,
                    Instructions = order.Instructions
                };
                db.Orders.Add(newOrder);
                db.SaveChanges();

                List<OrderDetail> cart = (List<OrderDetail>)Session["cart"];
                if (cart != null)
                {
                    foreach (var item in cart)
                    {
                        item.OrderId = newOrder.OrderId;
                        db.OrderDetails.Add(item);
                        db.SaveChanges();
                        Item thisItem = db.Items.Find(item.ItemId);
                        if (thisItem != null)
                        {
                            thisItem.Quantity -= item.Quantity;
                            db.Entry(thisItem).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    dynamic email = new Email("order");
                    email.To = db.Users.Find(User.Identity.GetUserId()).Email;
                    email.message = "Order confirmed";
                    email.orderno = newOrder.OrderId;
                    email.orderdate = newOrder.OrderDate.ToShortDateString();
                    email.items = cart;
                    email.Send();
                    Session.Remove("cart");

                }
                
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Name = manager.FindByEmail(order.UserId).Name;
            ViewBag.Reference = manager.FindByEmail(order.UserId).Reference;
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Edit([Bind(Include = "OrderId,UserId,OrderDate,Complete, Instructions")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Shop()
        {
            ShopViewModel shop = new ShopViewModel();

            var available = db.Items.ToList();
            List<ShopItemViewModel> additems = new List<ShopItemViewModel>();

            foreach (var item in available)
            {
                additems.Add(new ShopItemViewModel
                {
                    ItemId = item.ItemId,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    AddNo = 1
                });
            }

            shop.Items = additems;

            if (Session["cart"] == null)
            {
                List<OrderDetail> cart = new List<OrderDetail>();
                Session["cart"] = cart;
                shop.CartItems = null;
            }
            else
            {
                List<OrderDetail> cart = (List<OrderDetail>) Session["cart"];

                List<CartViewModel> cartItems = new List<CartViewModel>();

                foreach (var item in cart)
                {
                    cartItems.Add(new CartViewModel
                    {
                        ItemId = item.ItemId,
                        Description = db.Items.Find(item.ItemId).Description,
                        Quantity = item.Quantity,
                    });
                  shop.Items.First(c => c.ItemId == item.ItemId).Quantity -= item.Quantity;
                }
                shop.CartItems = cartItems;
            }

            return View(shop);
        }
        [HttpPost]
        public ActionResult AddItem(int itemId, int toAdd)
        {
            var selectItem = db.Items.Find(itemId);
            if (toAdd > selectItem.Quantity)
            {
                toAdd = selectItem.Quantity;
            }

            if (Session["cart"] == null)
            {
                List<OrderDetail> cart = new List<OrderDetail> {new OrderDetail {ItemId = itemId, Quantity = toAdd }};
                Session["cart"] = cart;
            }
            else
            {
                List<OrderDetail> cart = (List<OrderDetail>) Session["cart"];
                int index = IsExist(itemId);
                if (index != -1)
                {
                    if (toAdd + cart[index].Quantity > selectItem.Quantity)
                    {
                        toAdd = selectItem.Quantity - cart[index].Quantity;
                    }
                    cart[index].Quantity += toAdd;
                }
                else
                {
                    cart.Add(new OrderDetail { ItemId = itemId, Quantity = toAdd });
                }
                Session["cart"] = cart;
            }

            return RedirectToAction("Shop");
        }
        
        public ActionResult RemoveItem(int id)
        {
            if (Session["cart"] != null)
            {
                List<OrderDetail> cart = (List<OrderDetail>)Session["cart"];
                int index = IsExist(id);
                if (index != -1)
                {
                    cart.RemoveAt(index);
                }

                if (cart.Count > 0)
                {
                    Session["cart"] = cart;
                }
                else
                {
                    Session.Remove("cart");
                }
            }

            return RedirectToAction("Shop");
        }

        public ActionResult DeleteCart()
        {
            Session.Remove("cart");
            return RedirectToAction("Shop");
        }

        public ActionResult ViewCart()
        {
            List<OrderDetail> cart = (List<OrderDetail>)Session["cart"];
            if (cart != null)
            {
                List<CartViewModel> cartItems = new List<CartViewModel>();
                foreach (var item in cart)
                {
                    cartItems.Add(new CartViewModel
                    {
                        Description = db.Items.Find(item.ItemId).Description,
                        Quantity = item.Quantity
                    });
                }
                return View(cartItems);
            }
            else
            {
                return RedirectToAction("Shop");
            }
        }

        private int IsExist(int id)
        {
            List<OrderDetail> cart = (List<OrderDetail>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].ItemId.Equals(id))
                    return i;
            return -1;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            } 
            base.Dispose(disposing);
        }
    }
}
