using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS4Carroll.Data;
using LMS4Carroll.Models;
using System.Data.SqlTypes;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LMS4Carroll.Controllers
{
    [Authorize(Roles = "Admin,Handler,BiologyUser,ChemUser,AnimalUser")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration configuration;

        public OrdersController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            this.configuration = config;   
        }

        // GET: Orders
        public async Task<IActionResult> Index(string orderString)
        {
            ViewData["CurrentFilter"] = orderString;
            sp_Logging("1-Info", "View", "Successfuly viewed Orders list", "Success");


            if (!String.IsNullOrEmpty(orderString))
            {
                var orders = from m in _context.Orders.Include(c => c.Vendor)
                             select m;

                /*SqlDateTime dateCompare = Convert.ToSqlDateTime(orderString);
                CultureInfo myCItrad = new CultureInfo("bg-BG");
                DateTime parsedDate = DateTime.ParseExact(
                   orderString,
                   "dd/MM/yyyy hh:mm:ss",
                   myCItrad);
                */
                //Search Feature
                DateTime dt;
                string dateTime;
                if (!orderString.Contains(":"))
                {
                    dateTime = orderString + " 00:00:00.0000000";
                }
                else
                {
                    dateTime = orderString + "00.0000000";
                }

                int forID;
                if (Int32.TryParse(orderString, out forID) && !orderString.Contains("-"))
                {
                    orders = orders.Where(p => p.OrderID.Equals(forID));
                    return View(await orders.OrderByDescending(s => s.OrderID).ToListAsync());
                }
                else if (DateTime.TryParseExact(dateTime, "dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    orders = orders.Where(p => p.Recievedate.Equals(dt)
                                || p.Orderdate.Equals(dt));

                    return View(await orders.OrderByDescending(s => s.OrderID).ToListAsync());
                }
                else
                {
                    orders = orders.Where(p => p.Status.Contains(orderString)
                                || p.OrderID.Equals(forID)
                                || p.PO.Contains(orderString)
                                || p.Invoice.Contains(orderString)
                                || p.Status.Contains(orderString)
                                || p.Vendor.Name.Contains(orderString));
                    return View(await orders.OrderByDescending(s => s.OrderID).ToListAsync());
                }
            }
            else
            {
                var orders = from m in _context.Orders.Include(c => c.Vendor).Take(50)
                             select m;
                return View(await orders.OrderByDescending(s => s.OrderID).ToListAsync());
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["VendorID"] = new SelectList(_context.Vendors, "VendorID", "Name");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enabled binding of properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,Orderdate,Recievedate,Status,VendorID,Invoice,PO")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                sp_Logging("2-Change", "Create", "User created an order", "Success");
                return RedirectToAction("Index");
            }
            ViewData["VendorID"] = new SelectList(_context.Vendors, "VendorID", "Name", order.VendorID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["VendorID"] = new SelectList(_context.Vendors, "VendorID", "Name", order.VendorID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enabled binding of properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,Orderdate,Recievedate,Status,VendorID,Invoice,PO")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    sp_Logging("2-Change", "Edit", "User edited an order where ID= " + id.ToString(), "Success");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["VendorID"] = new SelectList(_context.Vendors, "VendorID", "Name", order.VendorID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderID == id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            sp_Logging("3-Remove", "Delete", "User deleted an order where ID= " + id.ToString(), "Success");
            return RedirectToAction("Index");
        }

        private bool OrderExists(int? id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }

        //Custom Loggin Solution
        private void sp_Logging(string level, string logger, string message, string exception)
        {
            //Connection string from AppSettings.JSON
            string CS = configuration.GetConnectionString("DefaultConnection");
            //Using Identity middleware to get email address
            string user = User.Identity.Name;
            string app = "Carroll LMS";
            //Subtract 5 hours as the timestamp is in GMT timezone
            DateTime logged = DateTime.Now.AddHours(-5);
            //logged.AddHours(-5);
            string site = "Orders";
            string query = "insert into dbo.Log([User], [Application], [Logged], [Level], [Message], [Logger], [CallSite]," +
                "[Exception]) values(@User, @Application, @Logged, @Level, @Message,@Logger, @Callsite, @Exception)";
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@User", user);
                cmd.Parameters.AddWithValue("@Application", app);
                cmd.Parameters.AddWithValue("@Logged", logged);
                cmd.Parameters.AddWithValue("@Level", level);
                cmd.Parameters.AddWithValue("@Message", message);
                cmd.Parameters.AddWithValue("@Logger", logger);
                cmd.Parameters.AddWithValue("@Callsite", site);
                cmd.Parameters.AddWithValue("@Exception", exception);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}
