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
	public class CageController : Controller
	{
		private readonly ApplicationDbContext _context;
		// From earlier iteration - Nlog
		//private readonly NLog.ILogger _logger;
		private IConfiguration configuration;
		public CageController(ApplicationDbContext context, IConfiguration config)
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

		// GET: Cages
		public async Task<IActionResult> Index(string Cagestring)
		{
			//_logger.Info("Viewed an animal list - CageController");
			sp_Logging("1-Info", "View", "Viewed list of animals", "Success");
			ViewData["CurrentFilter"] = Cagestring;

			//Getting all animal records from the DB including 
			//related Location and Order records as LocationID and OrderID are foreign keys
			//Search Feature
			if (!String.IsNullOrEmpty(Cagestring))
			{
				var Cages = from m in _context.Cage.Include(c => c.Location)
							select m;
				int forID;
				//If String parameter can be converted to inr32
				if (Int32.TryParse(Cagestring, out forID))
				{
					Cages = Cages.Where(s => s.CageID.Equals(forID));
					return View(await Cages.OrderByDescending(s => s.CageID).ToListAsync());
				}
				else
				{
					Cages = Cages.Where(s => s.CageDesignation.Contains(Cagestring)
									   || s.Location.Name.Contains(Cagestring)
									   || s.Location.Room.Contains(Cagestring)
									   || s.Location.NormalizedStr.Contains(Cagestring));
					return View(await Cages.OrderByDescending(s => s.CageID).ToListAsync());
				}
			}
			else
			{
				var Cages = from m in _context.Cage.Include(c => c.Location).Take(50)
							select m;
				return View(await Cages.OrderByDescending(s => s.CageID).ToListAsync());
			}
		}

		// GET: Cage/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var cage = await _context.Cage.SingleOrDefaultAsync(m => m.CageID == id);
			if (cage == null)
			{
				return NotFound();
			}

			return View(cage);
		}

		// GET: Cage/Create
		public IActionResult Create()
		{
			ViewData["LocationName"] = new SelectList(_context.Locations.Distinct(), "LocationID", "NormalizedStr");
			return View();
		}

		// POST: Cage/Create
		// Overposting attack vulnerability [Next iteration need to bind]
		//string cagedesignationstring, int locationinput, string speciesstring
		//[Bind("CageID,CageDesignation,LocationID,Species")] Cage cage
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(string cagedesignationstring, int locationinput, string speciesstring)
		{
			//_logger.Info("Attempted to add a cage - CageController")

			ViewData["CageDesignation"] = cagedesignationstring;
			ViewData["Location"] = locationinput;
			ViewData["Species"] = speciesstring;
			Cage cage = new Cage();

			if (ModelState.IsValid)
			{
				var temp = _context.Locations.First(m => m.LocationID == locationinput);

				cage.CageDesignation = cagedesignationstring;
				cage.LocationID = locationinput;
				cage.Species = speciesstring;
				cage.NormalizedLocation = temp.NormalizedStr;
				_context.Add(cage);
				sp_Logging("2-Change", "Create", "User created an Animal where name=" + cagedesignationstring, "Success");
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			ViewData["LocationName"] = new SelectList(_context.Locations.Distinct(), "LocationID", "NormalizedStr", cage.LocationID);
			return View(cage);
		}

		// GET: Cage/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var cage = await _context.Cage.SingleOrDefaultAsync(m => m.CageID == id);
			if (cage == null)
			{
				return NotFound();
			}
			ViewData["LocationName"] = new SelectList(_context.Locations.Distinct(), "LocationID", "NormalizedStr", cage.LocationID);
			return View(cage);
		}

		// Overposting attack vulnerability [Next iteration need to bind]
		//[Bind("CageID,CageDesignation,LocationID,Species")] Cage cage
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, string cagedesignationstring, int locationinput, string speciesstring)
		{
			Cage cage = await _context.Cage.FirstAsync(s => s.CageID == id);
			var temp = _context.Locations.First(m => m.LocationID == locationinput);
			cage.CageDesignation = cagedesignationstring;
			cage.LocationID = locationinput;
			cage.Species = speciesstring;
			cage.NormalizedLocation = temp.NormalizedStr;

			if (id != cage.CageID)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					sp_Logging("2-Change", "Edit", "User edited a Cage where ID= " + id.ToString(), "Success");
					_context.Update(cage);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CageExists(cage.CageID))
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
			return View(cage);
		}

		// GET: Cage/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			sp_Logging("3-Remove", "Delete", "User removed a Cage where ID= " + id.ToString(), "Success");
			var cage = await _context.Cage.SingleOrDefaultAsync(m => m.CageID == id);
			if (cage == null)
			{
				return NotFound();
			}

			return View(cage);
		}

		// POST: Cage/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var cage = await _context.Cage.SingleOrDefaultAsync(m => m.CageID == id);
			_context.Cage.Remove(cage);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private bool CageExists(int id)
		{
			return _context.Cage.Any(e => e.CageID == id);
		}
	}
}
