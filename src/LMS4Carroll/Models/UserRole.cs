using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    public class UserRole : IdentityRole
    {
        public string Description { get; set; }
    }
}
