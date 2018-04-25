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
    public class BioEquipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration configuration;

        public BioEquipmentsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            this.configuration = config;
        }

        // GET: BioEquipments
        public async Task<IActionResult> Index(string equipmentString)
        {
            ViewData["CurrentFilter"] = equipmentString;
            sp_Logging("1-Info", "View", "Successfuly viewed Biology Equipment list", "Success");

            //Search Feature
            if (!String.IsNullOrEmpty(equipmentString))
            {
                var equipments = from m in _context.BioEquipments.Include(c => c.Location).Include(c => c.Order)
                                 select m;

                int forID;
                if (Int32.TryParse(equipmentString, out forID))
                {
                    equipments = equipments.Where(s => s.BioEquipmentID.Equals(forID)
                                            || s.LocationID.Equals(forID)
                                            || s.OrderID.Equals(forID));
                    return View(await equipments.OrderByDescending(s => s.BioEquipmentID).ToListAsync());
                }
                else
                {
                    equipments = equipments.Where(s => s.EquipmentName.Contains(equipmentString)
                                            || s.EquipmentModel.Contains(equipmentString)
                                            || s.SerialNumber.Contains(equipmentString)
                                           // || s.Location.NormalizedStr.Contains(equipmentString)
                                            || s.LOT.Equals(equipmentString)
                                            || s.CAT.Equals(equipmentString)
                                            || s.Type.Contains(equipmentString)
                                            || s.Comments.Contains(equipmentString));
                    return View(await equipments.OrderByDescending(s => s.BioEquipmentID).ToListAsync());
                }
            }

            else
            {
                var equipments = from m in _context.BioEquipments.Include(c => c.Location).Include(c => c.Order).Take(1000)
                                 select m;

                return View(await equipments.OrderByDescending(s => s.BioEquipmentID).ToListAsync());
            }
        }

        // GET: BioEquipments/Details/5
        [Authorize(Roles = "Admin,BiologyUser")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bioEquipment = await _context.BioEquipments.SingleOrDefaultAsync(m => m.BioEquipmentID == id);
            if (bioEquipment == null)
            {
                return NotFound();
            }

            return View(bioEquipment);
        }

        // GET: BioEquipments/Create
        [Authorize(Roles = "Admin,BiologyUser")]
        public IActionResult Create()
        {
            ViewData["LocationName"] = new SelectList(_context.Locations.Distinct(), "LocationID", "NormalizedStr");
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            return View();
        }

        // POST: BioEquipments/Create
        // To protect from overposting attacks, enabled bind properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,BiologyUser")]
        public async Task<IActionResult> Create([Bind("BioEquipmentID,SerialNumber,InstalledDate,InspectionDate,CAT,LOT,EquipmentModel,EquipmentName,LocationID,OrderID,Type,Comments")] BioEquipment bioEquipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bioEquipment);
                await _context.SaveChangesAsync();
                sp_Logging("2-Change", "Create", "User created a biological equipment","Success");
                return RedirectToAction("Index");
            }
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "NormalizedStr", bioEquipment.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", bioEquipment.OrderID);
            return View(bioEquipment);
        }

        // GET: BioEquipments/Edit/5
        [Authorize(Roles = "Admin,BiologyUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bioEquipment = await _context.BioEquipments.SingleOrDefaultAsync(m => m.BioEquipmentID == id);
            if (bioEquipment == null)
            {
                return NotFound();
            }
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "NormalizedStr", bioEquipment.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", bioEquipment.OrderID);
            return View(bioEquipment);
        }

        // POST: BioEquipments/Edit/5
        // To protect from overposting attacks, enabled bind properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,BiologyUser")]
        public async Task<IActionResult> Edit(int id, [Bind("BioEquipmentID,SerialNumber,InstalledDate,InspectionDate,CAT,LOT,EquipmentModel,EquipmentName,LocationID,OrderID,Type,Comments")] BioEquipment bioEquipment)
        {
            if (id != bioEquipment.BioEquipmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bioEquipment);
                    sp_Logging("2-Change", "Edit", "User edited a Biological Equipment where ID= " + id.ToString(), "Success");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BioEquipmentExists(bioEquipment.BioEquipmentID))
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
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "NormalizedStr", bioEquipment.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", bioEquipment.OrderID);
            return View(bioEquipment);
        }

        // GET: BioEquipments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bioEquipment = await _context.BioEquipments.SingleOrDefaultAsync(m => m.BioEquipmentID == id);
            if (bioEquipment == null)
            {
                return NotFound();
            }

            return View(bioEquipment);
        }

        // POST: BioEquipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bioEquipment = await _context.BioEquipments.SingleOrDefaultAsync(m => m.BioEquipmentID == id);
            _context.BioEquipments.Remove(bioEquipment);
            sp_Logging("3-Remove", "Delete", "User deleted a Biological Equipment where ID=" + id.ToString(), "Success");
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: BioEquipments/Archive/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bioEquipment = await _context.BioEquipments.SingleOrDefaultAsync(m => m.BioEquipmentID == id);
            if (bioEquipment == null)
            {
                return NotFound();
            }

            return View(bioEquipment);
        }

        // POST: BioEquipments/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveConfirm(int id)
        {
            var bioEquipment = await _context.BioEquipments.SingleOrDefaultAsync(m => m.BioEquipmentID == id);
            BioArchive bioArchive = new BioArchive();

            if (bioArchive != null)
            {

                bioArchive.OrderID = bioEquipment.OrderID;
                bioArchive.Type = bioEquipment.Type;
                bioArchive.SerialNumber = bioEquipment.SerialNumber;
                bioArchive.InstalledDate = bioEquipment.InstalledDate;
                bioArchive.ArchiveDate = DateTime.Today;
                bioArchive.EquipmentName = bioEquipment.EquipmentName;
                bioArchive.EquipmentModel = bioEquipment.EquipmentModel;
                bioArchive.Comments = bioEquipment.Comments;
                _context.BioArchives.Add(bioArchive);
                await _context.SaveChangesAsync();
            }
            _context.BioEquipments.Remove(bioEquipment);
            sp_Logging("3-Remove", "Delete", "User deleted a Biology Equipment where ID=" + id.ToString(), "Success");
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BioEquipmentExists(int id)
        {
            return _context.BioEquipments.Any(e => e.BioEquipmentID == id);
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
            string site = "BioEquipments";
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
