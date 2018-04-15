using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LMS4Carroll.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using LMS4Carroll.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LMS4Carroll.Controllers
{
    [Authorize(Roles = "Admin,Handler,Student,PhysicsUser")]
    public class PhyArchivesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration configuration;

        public PhyArchivesController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            this.configuration = config;
        }

        // GET: PhyArchive
        public async Task<IActionResult> Index(string equipmentString)
        {
            ViewData["CurrentFilter"] = equipmentString;
            sp_Logging("1-Info", "View", "Successfuly viewed Physics Archive list", "Success");

            //Search Feature
            if (!String.IsNullOrEmpty(equipmentString))
            {
                var equipments = from m in _context.PhyArchives.Include(c => c.Order)
                                 select m;

                int forID;
                if (Int32.TryParse(equipmentString, out forID))
                {
                    equipments = equipments.Where(s => s.PhyArchiveID.Equals(forID)
                                            || s.OrderID.Equals(forID));
                    return View(await equipments.OrderByDescending(s => s.PhyArchiveID).ToListAsync());
                }
                else
                {
                    equipments = equipments.Where(s => s.EquipmentName.Contains(equipmentString)
                                            || s.EquipmentModel.Contains(equipmentString)
                                            || s.SerialNumber.Equals(equipmentString)
                                            || s.Type.Contains(equipmentString)
                                            || s.Comments.Contains(equipmentString));
                    return View(await equipments.OrderByDescending(s => s.PhyArchiveID).ToListAsync());
                }
            }

            else
            {
                var equipments = from m in _context.PhyArchives.Include(c => c.Order).Take(300)
                                 select m;

                return View(await equipments.OrderByDescending(s => s.PhyArchiveID).ToListAsync());
            }
        }

        // GET: PhyArchives/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phyArchive = await _context.PhyArchives.SingleOrDefaultAsync(m => m.PhyArchiveID == id);
            if (phyArchive == null)
            {
                return NotFound();
            }

            return View(phyArchive);
        }

        // GET: PhyArchives/Create
        public IActionResult Create()
        {
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            return View();
        }

        // POST: PhyArchives/Create
        // To protect from overposting attacks, enabled bind properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhyArchiveID,SerialNumber,InstalledDate,ArchiveDate,EquipmentModel,EquipmentName,OrderID,Type,Comments")] PhyArchive phyArchive)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phyArchive);
                await _context.SaveChangesAsync();
                sp_Logging("2-Change", "Create", "User created a physics archive","Success");
                return RedirectToAction("Index");
            }
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", phyArchive.OrderID);
            return View(phyArchive);
        }

        // GET: PhyArchives/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phyArchive = await _context.PhyArchives.SingleOrDefaultAsync(m => m.PhyArchiveID == id);
            if (phyArchive == null)
            {
                return NotFound();
            }
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", phyArchive.OrderID);
            return View(phyArchive);
        }

        // POST: PhyArchives/Edit/5
        // To protect from overposting attacks, enabled bind properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PhyArchiveID,SerialNumber,InstalledDate,ArchiveDate,EquipmentModel,EquipmentName,OrderID,Type,Comments")] PhyArchive phyArchive)
        {
            if (id != phyArchive.PhyArchiveID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phyArchive);
                    sp_Logging("2-Change", "Edit", "User edited a Physics Archive where ID= " + id.ToString(), "Success");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhyArchiveExists(phyArchive.PhyArchiveID))
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
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", phyArchive.OrderID);
            return View(phyArchive);
        }

        // GET: PhyArchives/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phyArchive = await _context.PhyArchives.SingleOrDefaultAsync(m => m.PhyArchiveID == id);
            if (phyArchive == null)
            {
                return NotFound();
            }

            return View(phyArchive);
        }

        // POST: PhyArchives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phyArchive = await _context.PhyArchives.SingleOrDefaultAsync(m => m.PhyArchiveID == id);
            _context.PhyArchives.Remove(phyArchive);
            sp_Logging("3-Remove", "Delete", "User deleted a Physics Archive where ID=" + id.ToString(), "Success");
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PhyArchiveExists(int id)
        {
            return _context.PhyArchives.Any(e => e.PhyArchiveID == id);
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
            string site = "PhyArchives";
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
