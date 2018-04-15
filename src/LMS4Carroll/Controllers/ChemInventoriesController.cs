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
using ZXing;

namespace LMS4Carroll.Controllers
{
    [Authorize(Roles = "Admin,ChemUser,BiologyUser,Student")]
    public class ChemInventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration configuration;

        public ChemInventoriesController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            this.configuration = config;
        }

        // GET: ChemInventories
        public async Task<IActionResult> Index(string cheminventoryString)
        {
            //var applicationDbContext = _context.ChemInventory.Include(c => c.Chemical).Include(c => c.Location).Include(c => c.Order);
            ViewData["CurrentFilter"] = cheminventoryString;
            sp_Logging("1-Info", "View", "Successfuly viewed Chemical Inventory list", "Success");
            var inventory = from m in _context.ChemInventory.Include(c => c.Chemical).Include(c => c.Location).Include(c => c.Order)
                            select m;
            

            //Search Feature
            if (!String.IsNullOrEmpty(cheminventoryString))
            {
                int forID;
                if (Int32.TryParse(cheminventoryString, out forID))
                {
                    inventory = inventory.Where(s => s.ChemInventoryId.Equals(forID));
                    return View(await inventory.OrderBy(s => s.Chemical.FormulaName).ToListAsync());
                }
                else
                {
                   /* inventory = inventory.Where(s => s.Chemical.FormulaName.ToLower().Contains(cheminventoryString)
                                                  || s.Chemical.CAS.ToLower().Contains(cheminventoryString)         //This is a fickle beast
                                                  || s.Location.StorageCode.ToLower().Contains(cheminventoryString) //I'm assuming an error is 
                                                  || s.Chemical.CAT.ToLower().Contains(cheminventoryString)         //generated in these lines 
                                                  || s.LOT.ToLower().Contains(cheminventoryString)                  //but I cannot figure out 
                                                  || s.Department.ToLower().Contains(cheminventoryString)           //the origin or cause
                                                  );
                    */                 
                }
            }

            return View(await inventory.OrderBy(s => s.Chemical.FormulaName).ToListAsync());
        }

        // print out the chemInventory as a pdf...
        public void generateReport()
        {
            //todo

        }

        // GET: ChemInventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemInventory = await _context.ChemInventory.SingleOrDefaultAsync(m => m.ChemInventoryId == id);
            if (chemInventory == null)
            {
                return NotFound();
            }

            return View(chemInventory);
        }

        // GET: ChemInventories/Create
        public IActionResult Create()
        {
            ViewData["ChemID"] = new SelectList(_context.Chemical, "ChemID", "FormulaName");
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "StorageCode");
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            return View();
        }

        // POST: ChemInventories/Create
        // Overposting attack vulnerability [Next iteration need to bind]
        //[Bind("ChemInventoryId,OrderID,LocationID,ChemID,Units,QtyLeft,ExpiryDate")] ChemInventory chemInventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? formulainput, DateTime dateinput, int? storageinput, int? orderinput, string cat, string lot, float qtyinput, string unitstring, string deptstring)
        { 
            ViewData["Formula"] = formulainput;
            ViewData["ExpiryDate"] = dateinput;
            ViewData["StorageCode"] = storageinput;
            ViewData["Order"] = orderinput;
            ViewData["Qty"] = qtyinput;
            ViewData["Unit"] = unitstring;
            ViewData["Department"] = deptstring;
            ViewData["CAT"] = cat;
            ViewData["LOT"] = lot;

            ChemInventory chemInventory = null;

            if (ModelState.IsValid)
            {
                //var chemID = _context.Chemical.Where(p => p.Formula == FormulaString).Select(p => p.ChemID);
                //var Chem = _context.Chemical.Where(p => p.Formula == FormulaString);
                //chemInventory.ChemID = await chemID;
                chemInventory = new ChemInventory();
                chemInventory.ChemID = formulainput;
                chemInventory.LocationID = storageinput;
                chemInventory.ExpiryDate = dateinput;
                chemInventory.OrderID = orderinput;
                chemInventory.QtyLeft = qtyinput;
                chemInventory.Units = unitstring;
                chemInventory.Department = deptstring;
                chemInventory.CAT = cat;
                chemInventory.LOT = lot;
                var temp = _context.Locations.First(m => m.LocationID == storageinput);
                chemInventory.NormalizedLocation = temp.StorageCode;
                
                _context.Add(chemInventory);
                await _context.SaveChangesAsync();
                sp_Logging("2-Change", "Create", "User created a chemical inventory item where ChemID=" + formulainput + ", OrderID=" + formulainput, "Success");
                return RedirectToAction("Index");
            }
            ViewData["ChemID"] = new SelectList(_context.Chemical, "ChemID", "FormulaName", chemInventory.ChemID);
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "StorageCode", chemInventory.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", chemInventory.OrderID);
            return View(chemInventory);
        }

        // GET: ChemInventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemInventory = await _context.ChemInventory.SingleOrDefaultAsync(m => m.ChemInventoryId == id);
            if (chemInventory == null)
            {
                return NotFound();
            }
            ViewData["ChemID"] = new SelectList(_context.Chemical, "ChemID", "FormulaName", chemInventory.ChemID);
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "StorageCode", chemInventory.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", chemInventory.OrderID);
            return View(chemInventory);
        }

        // POST: ChemInventories/Edit/5
        // Overposting attack vulnerability [Next iteration need to bind]
        //[Bind("ChemInventoryId,OrderID,LocationID,ChemID,Units,QtyLeft,ExpiryDate")] ChemInventory chemInventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int? formulainput, DateTime dateinput, int? storageinput, int? orderinput, string cat, string lot, float qtyinput, string unitstring, string deptstring)
        {
            ChemInventory chemInventory = await _context.ChemInventory.SingleOrDefaultAsync(p => p.ChemInventoryId == id);

            if (id != chemInventory.ChemInventoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    chemInventory.ChemID = formulainput;
                    chemInventory.LocationID = storageinput;
                    chemInventory.ExpiryDate = dateinput;
                    chemInventory.OrderID = orderinput;
                    chemInventory.QtyLeft = qtyinput;
                    chemInventory.Units = unitstring;
                    chemInventory.Department = deptstring;
                    chemInventory.CAT = cat;
                    chemInventory.LOT = lot;
                    var temp = _context.Locations.First(m => m.LocationID == storageinput);
                    chemInventory.NormalizedLocation = temp.StorageCode;
                    _context.Update(chemInventory);
                    await _context.SaveChangesAsync();
                    sp_Logging("2-Change", "Edit", "User edited a Chemical inventory item where ID= " + id.ToString(), "Success");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChemInventoryExists(chemInventory.ChemInventoryId))
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
            ViewData["ChemID"] = new SelectList(_context.Chemical, "ChemID", "FormulaName", chemInventory.ChemID);
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "StorageCode", chemInventory.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", chemInventory.OrderID);
            return View(chemInventory);
        }

        // GET: ChemInventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemInventory = await _context.ChemInventory.SingleOrDefaultAsync(m => m.ChemInventoryId == id);
            if (chemInventory == null)
            {
                return NotFound();
            }

            return View(chemInventory);
        }

        // POST: ChemInventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chemInventory = await _context.ChemInventory.SingleOrDefaultAsync(m => m.ChemInventoryId == id);
            _context.ChemInventory.Remove(chemInventory);
            await _context.SaveChangesAsync();
            sp_Logging("3-Remove", "Delete", "User deleted a Chemical inventory item where ID=" + id.ToString(), "Success");
            return RedirectToAction("Index");
        }

        private bool ChemInventoryExists(int? id)
        {
            return _context.ChemInventory.Any(e => e.ChemInventoryId == id);
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
            string site = "ChemInventory";
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
