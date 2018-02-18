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
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace LMS4Carroll.Controllers
{
    [Authorize(Roles = "Admin,ChemUser,BiologyUser,Student")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration configuration;

        public CoursesController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            this.configuration = config;
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
            string site = "Course";
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

        // GET: Courses
        public async Task<IActionResult> Index(string coursestring)
        {
            ViewData["CurrentFilter"] = coursestring;
            sp_Logging("1-Info", "View", "Viewed Course List Successfuly", "Success");

            var courses = from m in _context.Course.Include(c => c.Location)
                             select m;

            if (!String.IsNullOrEmpty(coursestring))
            {
                int forID;
                if (Int32.TryParse(coursestring, out forID))
                {
                    courses = courses.Where(s => s.CourseID.Equals(forID));
                    return View(await courses.OrderByDescending(s => s.CourseID).ToListAsync());
                }
                else
                {
                    courses = courses.Where(s => s.Department.Contains(coursestring)
                                       || s.Handler.Contains(coursestring)
                                       || s.NormalizedStr.Contains(coursestring)
                                       || s.Location.Name.Contains(coursestring)
                                       || s.Location.Room.Contains(coursestring)
                                       || s.Location.NormalizedStr.Contains(coursestring)
                                       || s.Name.Contains(coursestring)
                                       || s.Number.Contains(coursestring));
                    return View(await courses.OrderByDescending(s => s.CourseID).ToListAsync());
                }
            }

            // var applicationDbContext = _context.bioicalEquipments.Include(c => c.Location).Include(c => c.Order);
            return View(await courses.OrderByDescending(s => s.CourseID).ToListAsync());
            //return View(await _context.Course.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            sp_Logging("1-Info", "View", "Viewed Course details where CourseId = " + id.ToString(), "Success");

            var course = await _context.Course.SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["LocationName"] = new SelectList(_context.Locations.Distinct(), "LocationID", "NormalizedStr");

            return View();
        }

        // POST: Courses/Create
        // Overposting attack vulnerability [Next iteration need to bind]
        //[Bind("CourseID,Department,Handler,Instructor,Name,Number")] Course course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string deptstring,string handlerstring,
            string namestring, string numberstring, int locationinput)
        {
            ViewData["Location"] = locationinput;
            ViewData["Name"] = namestring;
            ViewData["Number"] = numberstring;
            //ViewData["Instructor"] = instructorstring;
            ViewData["Handler"] = handlerstring;
            ViewData["Department"] = deptstring;
            Course course = new Course();
            if (ModelState.IsValid)
            {
                course.Department = deptstring;
                course.Handler = handlerstring;
                course.Name = namestring;
                course.Number = numberstring;
                course.LocationID = locationinput;
                course.NormalizedStr = deptstring + "-" + numberstring;               
                var temp = _context.Locations.First(m => m.LocationID == locationinput);
                course.NormalizedLocation = temp.NormalizedStr;
                _context.Add(course);
                await _context.SaveChangesAsync();
                sp_Logging("2-Create", "Create", "Created Course Successfuly where Course is " + deptstring + "-" + numberstring, "Success");
                return RedirectToAction("Index");
            }
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "NormalizedStr", course.LocationID);

            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "NormalizedStr", course.LocationID);
            return View(course);
        }

        // POST: Courses/Edit/5
        // Overposting attack vulnerability [Next iteration need to bind]
        //[Bind("CourseID,Department,Handler,Instructor,Name,Number")] Course course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string deptstring, string handlerstring,
            string namestring, string numberstring, int locationinput)
        {
            var course = await _context.Course.FirstAsync(m => m.CourseID == id);
            course.Department = deptstring;
            course.Handler = handlerstring;
            course.Name = namestring;
            course.Number = numberstring;
            course.LocationID = locationinput;
            course.NormalizedStr = deptstring + "-" + numberstring;
            var temp = _context.Locations.First(m => m.LocationID == locationinput);
            course.NormalizedLocation = temp.NormalizedStr;
            _context.Entry<Course>(course).State = EntityState.Modified;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    sp_Logging("2-Edit", "Edit", "Edited Course Successfuly where CourseId = " + id.ToString(), "Success");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseID))
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
            ViewData["LocationName"] = new SelectList(_context.Locations, "LocationID", "NormalizedStr", course.LocationID);
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.SingleOrDefaultAsync(m => m.CourseID == id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            sp_Logging("3-Remove", "Delete", "Deleted Course Successfuly where CourseId = " + id.ToString(), "Success");
            return RedirectToAction("Index");
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseID == id);
        }
    }
}
