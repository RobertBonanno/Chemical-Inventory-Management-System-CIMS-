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
    [Authorize(Roles = "Admin,Handler,Student,BiologyUser")]
    public class BioArchivesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration configuration;

        public BioArchivesController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            this.configuration = config;
        }

        // GET: BioArchive
        public async Task<IActionResult> Index(string equipmentString)
        {
            ViewData["CurrentFilter"] = equipmentString;
            sp_Logging("1-Info", "View", "Successfuly viewed Biology Archive list", "Success");

            //Search Feature
            if (!String.IsNullOrEmpty(equipmentString))
            {
                var equipments = from m in _context.BioArchives.Include(c => c.Order)
                                 select m;

                int forID;
                if (Int32.TryParse(equipmentString, out forID))
                {
                    equipments = equipments.Where(s => s.BioArchiveID.Equals(forID)
                                            || s.OrderID.Equals(forID));
                    return View(await equipments.OrderByDescending(s => s.BioArchiveID).ToListAsync());
                }
                else
                {
                    equipments = equipments.Where(s => s.EquipmentName.Contains(equipmentString)
                                            || s.EquipmentModel.Contains(equipmentString)
                                            || s.SerialNumber.Equals(equipmentString)
                                            || s.Type.Contains(equipmentString)
                                            || s.Comments.Contains(equipmentString));
                    return View(await equipments.OrderByDescending(s => s.BioArchiveID).ToListAsync());
                }
            }

            else
            {
                var equipments = from m in _context.BioArchives.Include(c => c.Order).Take(300)
                                 select m;

                return View(await equipments.OrderByDescending(s => s.BioArchiveID).ToListAsync());
            }
        }

        // GET: BioArchives/Details/5
        [Authorize(Roles = "Admin,BiologyUser")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bioArchive = await _context.BioArchives.SingleOrDefaultAsync(m => m.BioArchiveID == id);
            if (bioArchive == null)
            {
                return NotFound();
            }

            return View(bioArchive);
        }

        // GET: BioArchives/Create
        [Authorize(Roles = "Admin,BiologyUser")]
        public IActionResult Create()
        {
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            return View();
        }

        // POST: BioArchives/Create
        // To protect from overposting attacks, enabled bind properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,BiologyUser")]
        public async Task<IActionResult> Create([Bind("BioArchiveID,SerialNumber,InstalledDate,ArchiveDate,EquipmentModel,EquipmentName,OrderID,Type,Comments")] BioArchive bioArchive)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bioArchive);
                await _context.SaveChangesAsync();
                sp_Logging("2-Change", "Create", "User created a Biology archive","Success");
                return RedirectToAction("Index");
            }
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", bioArchive.OrderID);
            return View(bioArchive);
        }

        // GET: BioArchives/Edit/5
        [Authorize(Roles = "Admin,BiologyUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bioArchive = await _context.BioArchives.SingleOrDefaultAsync(m => m.BioArchiveID == id);
            if (bioArchive == null)
            {
                return NotFound();
            }
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", bioArchive.OrderID);
            return View(bioArchive);
        }

        // POST: BioArchives/Edit/5
        // To protect from overposting attacks, enabled bind properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,BiologyUser")]
        public async Task<IActionResult> Edit(int id, [Bind("BioArchiveID,SerialNumber,InstalledDate,ArchiveDate,EquipmentModel,EquipmentName,OrderID,Type,Comments")] BioArchive bioArchive)
        {
            if (id != bioArchive.BioArchiveID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bioArchive);
                    sp_Logging("2-Change", "Edit", "User edited a Biology Archive where ID= " + id.ToString(), "Success");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BiologyArchiveExists(bioArchive.BioArchiveID))
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
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", bioArchive.OrderID);
            return View(bioArchive);
        }

        // GET: BioArchives/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bioArchive = await _context.BioArchives.SingleOrDefaultAsync(m => m.BioArchiveID == id);
            if (bioArchive == null)
            {
                return NotFound();
            }

            return View(bioArchive);
        }

        // POST: BioArchives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bioArchive = await _context.BioArchives.SingleOrDefaultAsync(m => m.BioArchiveID == id);
            _context.BioArchives.Remove(bioArchive);
            sp_Logging("3-Remove", "Delete", "User deleted a Biology Archive where ID=" + id.ToString(), "Success");
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BiologyArchiveExists(int id)
        {
            return _context.BioArchives.Any(e => e.BioArchiveID == id);
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
            string site = "BioArchives";
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
