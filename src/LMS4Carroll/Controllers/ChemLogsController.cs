using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS4Carroll.Data;
using LMS4Carroll.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LMS4Carroll.Controllers
{
    [Authorize (Roles = "Admin,ChemUser,BiologyUser,Student")]
    public class ChemLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration configuration;

        public ChemLogsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            this.configuration = config;
        }

        // GET: ChemLogs
        public async Task<IActionResult> Index(string chemlogstring)
        {
            //var applicationDbContext = _context.ChemLog.Include(c => c.ChemInventory).Include(c => c.Course);
            ViewData["CurrentFilter"] = chemlogstring;
            sp_Logging("1-Info", "View", "Successfuly viewed Chemical Log list", "Success");


            //Search Feature
            if (!String.IsNullOrEmpty(chemlogstring))
            {
                var logs = from m in _context.ChemLog.Include(c => c.ChemInventory).Include(c => c.Course)
                           select m;

                int forID;
                if (Int32.TryParse(chemlogstring, out forID))
                {
                    logs = logs.Where(s => s.ChemLogId.Equals(forID));
                    return View(await logs.OrderByDescending(s => s.ChemLogId).ToListAsync());
                }
                else
                {
                    logs = logs.Where(s => s.Course.Department.Contains(chemlogstring)
                                       || s.Course.Handler.Contains(chemlogstring)
                                       || s.Course.Name.Contains(chemlogstring)
                                       || s.Course.Number.Contains(chemlogstring)
                                       || s.Course.CourseID.Equals(forID)
                                       || s.Course.Location.Name.Contains(chemlogstring)
                                       || s.Course.Location.Room.Contains(chemlogstring)
                                       || s.Course.Location.Type.Contains(chemlogstring)
                                       || s.ChemInventory.ChemInventoryId.Equals(forID)
                                       || s.ChemInventory.ChemID.Equals(forID)
                                       || s.ChemInventory.Chemical.CAS.Contains(chemlogstring)
                                       || s.ChemInventory.Chemical.Formula.Contains(chemlogstring)
                                       || s.ChemInventory.Chemical.FormulaName.Contains(chemlogstring)
                                       || s.ChemInventory.Chemical.Hazard.Contains(chemlogstring)
                                       || s.ChemInventory.Chemical.State.Contains(chemlogstring)
                                       || s.ChemInventory.Location.Name.Contains(chemlogstring)
                                       || s.ChemInventory.Location.Room.Contains(chemlogstring)
                                       || s.ChemInventory.Location.NormalizedStr.Contains(chemlogstring)
                                       || s.ChemInventory.ChemInventoryId.Equals(forID)
                                       || s.ChemInventory.Order.Vendor.Name.Contains(chemlogstring)
                                       || s.ChemInventory.Order.Invoice.Contains(chemlogstring)
                                       || s.ChemInventory.Order.PO.Contains(chemlogstring)
                                       || s.ChemInventory.CAT.Contains(chemlogstring));
                    return View(await logs.OrderByDescending(s => s.ChemLogId).ToListAsync());

                }
            }
            else
            {
                var logs = from m in _context.ChemLog.Include(c => c.ChemInventory).Include(c => c.Course).Take(50)
                           select m;
                return View(await logs.OrderByDescending(s => s.ChemLogId).ToListAsync());
            }
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: ChemLogs/Details/5
        [Authorize(Roles = "Admin,ChemUser,BiologyUser")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemLog = await _context.ChemLog.SingleOrDefaultAsync(m => m.ChemLogId == id);
            if (chemLog == null)
            {
                return NotFound();
            }

            return View(chemLog);
        }

        // GET: ChemLogs/Create
        [Authorize(Roles = "Admin,ChemUser,BiologyUser")]
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Course, "CourseID", "NormalizedStr");
            return View();
        }

        // POST: ChemLogs/Create
        // Overposting attack vulnerability [Next iteration need to bind]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ChemUser,BiologyUser")]
        public async Task<IActionResult> Create(int barcodeinput, int courseinput, float qtyusedinput)
        {
            ViewData["Barcode"] = barcodeinput;
            ViewData["Course"] = courseinput;
            ViewData["Qty"] = qtyusedinput;
            if (_context.ChemInventory.Count(M => M.ChemInventoryId == barcodeinput) >= 1) {
                ChemLog chemLog = new ChemLog();
                ChemInventory temp = _context.ChemInventory.FirstOrDefault(s => s.ChemInventoryId == barcodeinput);
                float tempValue = temp.QtyLeft;
                temp.QtyLeft = tempValue - qtyusedinput;
                _context.Entry<ChemInventory>(temp).State = EntityState.Modified;
                _context.SaveChanges();
                //chemLog.DatetimeCreated = DateTime.Now;
                if (ModelState.IsValid)
                {
                    chemLog.ChemInventoryId = barcodeinput;
                    chemLog.CourseID = courseinput;
                    chemLog.QtyUsed = qtyusedinput;
                    _context.Add(chemLog);
                    await _context.SaveChangesAsync();
                    sp_Logging("2-Change", "Create", "User created a Log entry where Barcode=" + barcodeinput, "Success");
                    return RedirectToAction("Index");
                }
                ViewData["CourseID"] = new SelectList(_context.Course, "CourseID", "NormalizedStr", chemLog.CourseID);
                return View(chemLog);
            }
            else
            {
                return View("CheckBarcode");
            }      
           
        }

        // GET: ChemLogs/Edit/5
        [Authorize(Roles = "Admin,ChemUser,BiologyUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemLog = await _context.ChemLog.SingleOrDefaultAsync(m => m.ChemLogId == id);
            if (chemLog == null)
            {
                return NotFound();
            }
            ViewData["Barcode"] = chemLog.ChemInventoryId;
            ViewData["Qty"] = chemLog.QtyUsed;
            ViewData["CourseID"] = new SelectList(_context.Course, "CourseID", "NormalizedStr", chemLog.CourseID);
            return View(chemLog);
        }

        // POST: ChemLogs/Edit/5
        // Overposting attack vulnerability [Next iteration need to bind]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ChemUser,BiologyUser")]
        public async Task<IActionResult> Edit(int id, int barcodeinput, int courseinput, float qtyusedinput)
        {
            if (_context.ChemInventory.Count(M => M.ChemInventoryId == barcodeinput) >= 1)
            {
                ChemLog chemLog = await _context.ChemLog.FirstAsync(m => m.ChemLogId == id);
                ChemInventory temp = _context.ChemInventory.FirstOrDefault(s => s.ChemInventoryId == barcodeinput);
                float tempValue = temp.QtyLeft;
                float used = chemLog.QtyUsed;
                temp.QtyLeft = tempValue + used - qtyusedinput;
                _context.Entry<ChemInventory>(temp).State = EntityState.Modified;
                _context.SaveChanges();
                chemLog.ChemInventoryId = barcodeinput;
                chemLog.CourseID = courseinput;
                chemLog.QtyUsed = qtyusedinput;

                if (id != chemLog.ChemLogId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(chemLog);
                        await _context.SaveChangesAsync();
                        sp_Logging("2-Change", "Edit", "User edited a Log entry where ID= " + id.ToString(), "Success");

                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ChemLogExists(chemLog.ChemLogId))
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
                ViewData["Barcode"] = new SelectList(_context.ChemInventory, "ChemInventoryId", "ChemInventoryId", chemLog.ChemInventoryId);
                ViewData["CourseID"] = new SelectList(_context.Course, "CourseID", "NormalizedStr", chemLog.CourseID);
                return View(chemLog);
            }
            else
            {
                return View("CheckBarcode");
            }           
        }

        // GET: ChemLogs/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemLog = await _context.ChemLog.SingleOrDefaultAsync(m => m.ChemLogId == id);
            if (chemLog == null)
            {
                return NotFound();
            }

            return View(chemLog);
        }

        // POST: ChemLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chemLog = await _context.ChemLog.SingleOrDefaultAsync(m => m.ChemLogId == id);
            var used = chemLog.QtyUsed;
            var barcode = chemLog.ChemInventoryId;
            var chemInv = _context.ChemInventory.First(m => m.ChemInventoryId == barcode);
            var tempQty = chemInv.QtyLeft;
            chemInv.QtyLeft = tempQty + used;
            _context.Update(chemInv);
            _context.ChemLog.Remove(chemLog);
            await _context.SaveChangesAsync();
            sp_Logging("3-Remove", "Edit", "User deleted a Log entry where ID= " + id.ToString(), "Success");
            return RedirectToAction("Index");
        }

        private bool ChemLogExists(int id)
        {
            return _context.ChemLog.Any(e => e.ChemLogId == id);
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
            string site = "ChemLogs";
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
