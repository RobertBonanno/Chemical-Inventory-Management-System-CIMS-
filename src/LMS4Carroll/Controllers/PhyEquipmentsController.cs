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
    public class PhyEquipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration configuration;

        public PhyEquipmentsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            this.configuration = config;
        }

        // GET: PhyEquipments
        public async Task<IActionResult> Index(string equipmentString)
        {
            ViewData["CurrentFilter"] = equipmentString;
            sp_Logging("1-Info", "View", "Successfuly viewed Physics Equipment list", "Success");

            //Search Feature
            if (!String.IsNullOrEmpty(equipmentString))
            {
                var equipments = from m in _context.PhyEquipments.Include(c => c.Location).Include(c => c.Order)
                                 select m;

                int forID;
                if (Int32.TryParse(equipmentString, out forID))
                {
                    equipments = equipments.Where(s => s.PhyEquipmentID.Equals(forID)
                                            || s.LocationID.Equals(forID)
                                            || s.OrderID.Equals(forID));
                    return View(await equipments.OrderByDescending(s => s.PhyEquipmentID).ToListAsync());
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
                    return View(await equipments.OrderByDescending(s => s.PhyEquipmentID).ToListAsync());
                }
            }

            else
            {
                var equipments = from m in _context.PhyEquipments.Include(c => c.Location).Include(c => c.Order).Take(300)
                                 select m;

                return View(await equipments.OrderByDescending(s => s.PhyEquipmentID).ToListAsync());
            }
        }

        // GET: PhyEquipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phyEquipment = await _context.PhyEquipments.SingleOrDefaultAsync(m => m.PhyEquipmentID == id);
            if (phyEquipment == null)
            {
                return NotFound();
            }

            return View(phyEquipment);
        }

        // GET: PhyEquipments/Create
        public IActionResult Create()
        {
            ViewData["LocationName"] = new SelectList(_context.Locations.Distinct(), "LocationID", "NormalizedStr");
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            return View();
        }

        // POST: PhyEquipments/Create
        // To protect from overposting attacks, enabled bind properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhyEquipmentID,SerialNumber,InstalledDate,InspectionDate,CAT,LOT,EquipmentModel,EquipmentName,LocationID,OrderID,Type,Comments")] PhyEquipment phyEquipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phyEquipment);
                await _context.SaveChangesAsync();
                sp_Logging("2-Change", "Create", "User created a physics equipment","Success");
                return RedirectToAction("Index");
            }
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "NormalizedStr", phyEquipment.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", phyEquipment.OrderID);
            return View(phyEquipment);
        }

        // GET: PhyEquipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phyEquipment = await _context.PhyEquipments.SingleOrDefaultAsync(m => m.PhyEquipmentID == id);
            if (phyEquipment == null)
            {
                return NotFound();
            }
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "NormalizedStr", phyEquipment.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", phyEquipment.OrderID);
            return View(phyEquipment);
        }

        // POST: PhyEquipments/Edit/5
        // To protect from overposting attacks, enabled bind properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PhyEquipmentID,SerialNumber,InstalledDate,InspectionDate,CAT,LOT,EquipmentModel,EquipmentName,LocationID,OrderID,Type,Comments")] PhyEquipment phyEquipment)
        {
            if (id != phyEquipment.PhyEquipmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phyEquipment);
                    sp_Logging("2-Change", "Edit", "User edited a Phylogical Equipment where ID= " + id.ToString(), "Success");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhyEquipmentExists(phyEquipment.PhyEquipmentID))
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
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "NormalizedStr", phyEquipment.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", phyEquipment.OrderID);
            return View(phyEquipment);
        }

        // GET: PhyEquipments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phyEquipment = await _context.PhyEquipments.SingleOrDefaultAsync(m => m.PhyEquipmentID == id);
            if (phyEquipment == null)
            {
                return NotFound();
            }

            return View(phyEquipment);
        }

        // POST: PhyEquipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phyEquipment = await _context.PhyEquipments.SingleOrDefaultAsync(m => m.PhyEquipmentID == id);
            _context.PhyEquipments.Remove(phyEquipment);
            sp_Logging("3-Remove", "Delete", "User deleted a Phylogical Equipment where ID=" + id.ToString(), "Success");
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: PhyEquipments/Archive/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phyEquipment = await _context.PhyEquipments.SingleOrDefaultAsync(m => m.PhyEquipmentID == id);
            if (phyEquipment == null)
            {
                return NotFound();
            }

            return View(phyEquipment);
        }

        // POST: PhyEquipments/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirm(int id)
        {
            var phyEquipment = await _context.PhyEquipments.SingleOrDefaultAsync(m => m.PhyEquipmentID == id);
            PhyArchive phyArchive = new PhyArchive();
            
            if(phyArchive != null)
            {

                phyArchive.OrderID = phyEquipment.OrderID;
                phyArchive.Type = phyEquipment.Type;
                phyArchive.SerialNumber = phyEquipment.SerialNumber;
                phyArchive.InstalledDate = phyEquipment.InstalledDate;
                phyArchive.ArchiveDate = DateTime.Today;
                phyArchive.EquipmentName = phyEquipment.EquipmentName;
                phyArchive.EquipmentModel = phyEquipment.EquipmentModel;
                phyArchive.Comments = phyEquipment.Comments;
                _context.PhyArchives.Add(phyArchive);
                await _context.SaveChangesAsync();
            }
            _context.PhyEquipments.Remove(phyEquipment);
            sp_Logging("3-Remove", "Delete", "User deleted a Physics Equipment where ID=" + id.ToString(), "Success");
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PhyEquipmentExists(int id)
        {
            return _context.PhyEquipments.Any(e => e.PhyEquipmentID == id);
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
            string site = "PhyEquipments";
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
