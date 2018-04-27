using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    public class Chemical
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Chemical ID")]
        public int? ChemID { get; set; }

        [StringLength(50)]
        [Display(Name = "CAT Number")]
        public string CAT { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "CAS Number")]
        public string CAS { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Chemical Name")]
        public string FormulaName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Formula")]
        public string Formula { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Formula Weight")]
        public string FormulaWeight { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Hazards")]
        public string Hazard { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "SDS")]
        public string SDS { get; set; }

        public virtual ICollection<ChemInventory> ChemInventories { get; set; }
    }
}
