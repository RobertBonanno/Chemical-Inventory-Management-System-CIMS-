using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    public class PhyEquipment
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "PhyEquipment ID")]
        public int PhyEquipmentID { get; set; }

        [ForeignKey("Order")]
        [Required]
        [Display(Name = "Order ID")]
        public int? OrderID { get; set; }
        public virtual Order Order { get; set; }

        [ForeignKey("Location")]
        [Required]
        public int? LocationID { get; set; }
        public virtual Location Location { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "CAT #")]
        public string CAT { get; set; }

        [StringLength(50)]
        [Display(Name = "LOT #")]
        public string LOT { get; set; }

        [StringLength(50)]
        [Display(Name = "Equipment Type")]
        public string Type { get; set; }

        [StringLength(50)]
        [Display(Name = "S/N")]
        public string SerialNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DefaultValue("01/01/1900")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Installed Date")]
        public DateTime InstalledDate { get; set; }

        [DataType(DataType.Date)]
        [DefaultValue("01/01/1900")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Next Inspection")]
        public DateTime InspectionDate { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Manufacturer Name")]
        public string EquipmentName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Equipment Model")]
        public string EquipmentModel { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }
    }
}
