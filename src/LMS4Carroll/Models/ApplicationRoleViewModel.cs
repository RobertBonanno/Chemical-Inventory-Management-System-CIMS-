using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    public class ApplicationRoleViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        [StringLength(50, MinimumLength = 2)]
        public string RoleName { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }
}
