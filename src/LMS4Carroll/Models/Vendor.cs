using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    public class Vendor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Vendor ID")]
        public int VendorID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Manufacturer Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Address { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Comments { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
