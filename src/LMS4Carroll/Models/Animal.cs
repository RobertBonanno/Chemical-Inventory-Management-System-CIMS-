using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    public class Animal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Animals ID")]
        public int AnimalID { get; set; }

        [ForeignKey("Order")]
        public int OrderID { get; set; }
        public virtual Order Order { get; set; }

        [ForeignKey("Location")]
        public int LocationID { get; set; }
        public virtual Location Location { get; set; }

        [StringLength(50)]
        [Display(Name = "Cage Designation")]
        public string Designation { get; set; }

        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "CAT Number")]
        public string CAT { get; set; }

        [StringLength(50)]
        [Display(Name = "Lot #")]
        public string LOT { get; set; }

        [DataType(DataType.Date)]
        [DefaultValue("01/01/1900")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "DOB")]
        public DateTime DOB { get; set; }

        [DataType(DataType.Date)]
        [DefaultValue("01/01/1900")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Removed")]
        public DateTime DOR { get; set; }

        [StringLength(50)]
        [Display(Name = "Species")]
        public string Species { get; set; }

        [StringLength(50)]
        [Display(Name = "Location")]
        public string NormalizedLocation { get; set; }

        public virtual ICollection<CageLog> CageLogs { get; set; }
    }
}
