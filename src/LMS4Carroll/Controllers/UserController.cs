using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LMS4Carroll.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using LMS4Carroll.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace LMS4Carroll.Controllers
{
    /// <summary>
    /// Controller that allows the use of the User releated pages (not including "account" functionality
    /// this is for the administrator to edit a user's information, inlcuding email, name, and role
    /// as the "Admin" controller, it has power over the functionality of the Index, Edit, and Delete 
    /// actions within the folder.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private IConfiguration configuration;

        /// <summary>
        /// Loads the context for the current user's session
        /// </summary>
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration config)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = config;
        }

        /// <summary>
        /// Method that gathers the available users for editing
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                sp_Logging("1-Info", "View", "Successfuly viewed Users list", "Success");
                //creates list of empty models from the context and then loads them with the 
                //necessary information. Roles are loaded later, only in the edit method
                List<UserListViewModel> model = new List<UserListViewModel>();
                model = userManager.Users.Select(u => new UserListViewModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    //RoleName = await u.returnRoleName(u.Id),
                    Email = u.Email
                }).ToList();
                //the view returns with the appropriate model, accessing the Index action
                return View(model);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Pulls up the edit Modal based on an ID. This ID is stored in the database
        /// as well
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            //the EditUserViewModel is specific to this method, and is compared to the 
            //UserListViewModel
            EditUserViewModel model = new EditUserViewModel();
            //the role list is now loaded, since it can be accesed in the edit method
            model.ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).ToList();

            //error check that model has been passed appropriate id
            if (!String.IsNullOrEmpty(id))
            {
                //that this user exists in database (async)
                ApplicationUser user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    //fills the empty model with the information of the user
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.Email = user.Email;
                    model.ApplicationRoleId = roleManager.Roles.Single(r => r.Name == userManager.GetRolesAsync(user).Result.Single()).Id;
                }
            }
            //the view is now ready to have adjustments made to it
            return PartialView("EditUser", model);
        }

        /// <summary>
        /// This method is called when the save button is pressed on the model. It is passed the ID
        /// and the information that the user shoulder be changed to.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, EditUserViewModel model)
        {
            //this is checked by the required fields
            if (ModelState.IsValid)
            {
                //locates the correct user
                ApplicationUser user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    //code that sets the values of the user to the model's
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    //gets the existing role
                    string existingRole = userManager.GetRolesAsync(user).Result.Single();
                    string existingRoleId = roleManager.Roles.Single(r => r.Name == existingRole).Id;
                    IdentityResult result = await userManager.UpdateAsync(user);
                    sp_Logging("2-Change", "Edit", "User edited a user item where email=" + model.Email, "Success");
                    if (result.Succeeded)
                    {
                        //checks if role has changed
                        if (existingRoleId != model.ApplicationRoleId)
                        {
                            //removes existing role
                            IdentityResult roleResult = await userManager.RemoveFromRoleAsync(user, existingRole);
                            if (roleResult.Succeeded)
                            {
                                ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId);
                                if (applicationRole != null)
                                {
                                    //sets rew role
                                    IdentityResult newRoleResult = await userManager.AddToRoleAsync(user, applicationRole.Name);

                                    if (newRoleResult.Succeeded)
                                    {
                                        return RedirectToAction("Index");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //return PartialView("EditUser", model);
            return RedirectToAction("Index");
            //the async won't be effective for model-based error handling.
            //consider following the format Chemicals use to edit details in a new window

            /// <summary>
            /// The view will now return to the index page
            /// </summary>
        }

        //code beyond this was not modified
        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser applicationUser = await userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    name = applicationUser.FirstName + " " + applicationUser.LastName;
                }
            }
            return RedirectToAction("Index");
        }
 
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id, FormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser applicationUser = await userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    IdentityResult result = await userManager.DeleteAsync(applicationUser);
                    sp_Logging("2-Change", "Edit", "User deleted a user item where email=" + applicationUser.Email, "Success");
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
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
    }
}