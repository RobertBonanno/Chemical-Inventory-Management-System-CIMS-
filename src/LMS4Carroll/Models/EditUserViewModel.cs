using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    /// <summary>
    /// The model is used by a modal to store data inputed by the user. It can then be checked
    /// and compared against a UserListModel to see if there have been changes. Used by the edit
    /// modal to generate its form
    /// </summary>
    public class EditUserViewModel
    {
        //the ID, can be used in the futuer for multiple edits
        public string Id { get; set; }

        //FirstName is required (the method Modal.IsValid will return false if it is missing)
        [Required]
        //The label, will display as "First Name" when called
        [Display(Name = "First Name")]
        //If a value not between 50 and 2 is entered, IsValid will fail
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        //Required to be valid
        [Required]
        //W display as "Last Name" when called
        [Display(Name = "Last Name")]
        //If a value not between 50 and 2 is entered, IsValid will fail
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        //Required to be valid
        [Required]
        //Because there is no display value, the label will remail the
        //property name (Email)
        //If a value not between 80 and 8 is entered, IsValid will fail
        [StringLength(80, MinimumLength = 8)]
        //checks if it is in valid email format
        [EmailAddress]
        public string Email { get; set; }
        //list of possible application Roles the form will load
        public List<SelectListItem> ApplicationRoles { get; set; }
        //ID of currently selected role. Label displays as "Role"
        [Display(Name = "Role")]
        public string ApplicationRoleId { get; set; }
    }
}
