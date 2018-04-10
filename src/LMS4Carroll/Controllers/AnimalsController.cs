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
using NLog;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LMS4Carroll.Controllers
{
    [Authorize(Roles = "Admin,AnimalUser,Student")]
    [ServiceFilter(typeof(LogFilter))]
    public class AnimalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        // From earlier iteration - Nlog
        //private readonly NLog.ILogger _logger;
        private IConfiguration configuration;
        public AnimalsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            this.configuration = config;
            //_logger = LogManager.GetLogger("databaseLogger");
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
            string site = "Animal";
            string query = "insert into dbo.Log([User], [Application], [Logged], [Level], [Message], [Logger], [CallSite],"+
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

        // GET: Animals
        public async Task<IActionResult> Index(string Animalstring)
        {
            //_logger.Info("Viewed an animal list - AnimalController");
            sp_Logging("1-Info", "View", "Viewed list of animals", "Success");
            ViewData["CurrentFilter"] = Animalstring;

            //Getting all animal records from the DB including 
            //related Location and Order records as LocationID and OrderID are foreign keys
            //Search Feature
            if (!String.IsNullOrEmpty(Animalstring))
            {
                var Animals = from m in _context.Animal.Include(c => c.Location).Include(c => c.Order)
                              select m;
                int forID;
                //If String parameter can be converted to inr32
                if (Int32.TryParse(Animalstring, out forID))
                {
                    Animals = Animals.Where(s => s.AnimalID.Equals(forID));
                    return View(await Animals.OrderByDescending(s => s.AnimalID).ToListAsync());
                }
                else
                {
                    Animals = Animals.Where(s => s.Designation.Contains(Animalstring)
                                       || s.Gender.Contains(Animalstring)
                                       || s.Location.Name.Contains(Animalstring)
                                       || s.Location.Room.Contains(Animalstring)
                                       || s.Location.NormalizedStr.Contains(Animalstring)
                                       || s.Order.Invoice.Contains(Animalstring)
                                       || s.Order.PO.Contains(Animalstring)
                                       || s.Order.Vendor.Name.Contains(Animalstring)
                                       || s.LOT.Contains(Animalstring)
                                       || s.CAT.Contains(Animalstring));
                    return View(await Animals.OrderByDescending(s => s.AnimalID).ToListAsync());
                }
            }
            else
            {
                var Animals = from m in _context.Animal.Include(c => c.Location).Include(c => c.Order).Take(50)
                              select m;
                return View(await Animals.OrderByDescending(s => s.AnimalID).ToListAsync());
            }
        }

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cage = await _context.Animal.SingleOrDefaultAsync(m => m.AnimalID == id);
            if (cage == null)
            {
                return NotFound();
            }

            return View(cage);
        }

        // GET: Animals/Create
        public IActionResult Create()
        {
            ViewData["LocationName"] = new SelectList(_context.Locations.Distinct(), "LocationID", "NormalizedStr");
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            return View();
        }

        // POST: Animals/Create
        // Overposting attack vulnerability [Next iteration need to bind]
        //DateTime dobinput,DateTime dorinput,string designationstring,string cat, string lot, string genderstring,int locationinput,int orderinput,string speciesstring, string namestring
        //[Bind("AnimalID,DOB,DOR,Designation,Name,LOT,CAT,Gender,LocationID,OrderID,Species")] Animal animal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DateTime dobinput, DateTime dorinput, string designationstring, string cat, string lot, string genderstring, double weightinput, int locationinput, int orderinput, string speciesstring, string namestring)
        {
            //_logger.Info("Attempted to add an animal - AnimalController");
            
            ViewData["DOB"] = dobinput;
            ViewData["DOR"] = dorinput;
            ViewData["Designation"] = designationstring;
            ViewData["Gender"] = genderstring;
			ViewData["Weight"] = weightinput;
            ViewData["Location"] = locationinput;
            ViewData["Order"] = orderinput;
            ViewData["Species"] = speciesstring;
            ViewData["Name"] = namestring;
            ViewData["CAT"] = cat;
            ViewData["LOT"] = lot;
            Animal cage = new Animal();

            if (ModelState.IsValid)
            {
                var temp = _context.Locations.First(m => m.LocationID == locationinput);
            
                cage.DOB = dobinput;
                cage.DOR = dorinput;
                cage.Designation = designationstring;
                cage.Gender = genderstring;
				cage.Weight = weightinput;
                cage.LocationID = locationinput;
                cage.OrderID = orderinput;
                cage.Species = speciesstring;
                cage.Name = namestring;
                cage.CAT = cat;
                cage.LOT = lot;
                cage.NormalizedLocation = temp.NormalizedStr;
                _context.Add(cage);
                sp_Logging("2-Change", "Create", "User created an Animal where name=" + namestring, "Success");
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["LocationName"] = new SelectList(_context.Locations.Distinct(), "LocationID", "NormalizedStr", cage.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", cage.OrderID);
            return View(cage);
        }

        // GET: Animals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cage = await _context.Animal.SingleOrDefaultAsync(m => m.AnimalID == id);
            if (cage == null)
            {
                return NotFound();
            }
            ViewData["LocationName"] = new SelectList(_context.Locations.Distinct(), "LocationID", "NormalizedStr", cage.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", cage.OrderID);
            return View(cage);
        }

        // Overposting attack vulnerability [Next iteration need to bind]
        //[Bind("CageID,DOB,Designation,Gender,LocationID,OrderID,Species")] Animal cage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DateTime dobinput, DateTime dorinput,string designationstring, string genderstring, double weightinput, string cat, string lot, int locationinput, int orderinput, string speciesstring, string namestring)
        {
            Animal cage = await _context.Animal.FirstAsync(s => s.AnimalID == id);
            var temp = _context.Locations.First(m => m.LocationID == locationinput);
            cage.DOB = dobinput;
            cage.DOR = dorinput;
            cage.Designation = designationstring;
            cage.Gender = genderstring;
			cage.Weight = weightinput;
            cage.LocationID = locationinput;
            cage.OrderID = orderinput;
            cage.Species = speciesstring;
            cage.CAT = cat;
            cage.LOT = lot;
            cage.Name = namestring;
            cage.NormalizedLocation = temp.NormalizedStr;

            if (id != cage.AnimalID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    sp_Logging("2-Change", "Edit", "User edited an Animal where ID= " + id.ToString(), "Success");
                    _context.Update(cage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExists(cage.AnimalID))
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
            ViewData["LocationName"] = new SelectList(_context.Locations.Distinct(), "LocationID", "NormalizedStr", cage.LocationID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", cage.OrderID);
            return View(cage);
        }

        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            sp_Logging("3-Remove", "Delete", "User removed an Animal where ID= " + id.ToString(), "Success");
            var cage = await _context.Animal.SingleOrDefaultAsync(m => m.AnimalID == id);
            if (cage == null)
            {
                return NotFound();
            }

            return View(cage);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cage = await _context.Animal.SingleOrDefaultAsync(m => m.AnimalID == id);
            _context.Animal.Remove(cage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AnimalExists(int id)
        {
            return _context.Animal.Any(e => e.AnimalID == id);
        }
    }
}
