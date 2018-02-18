using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    public class ChemInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Barcode")]
        public int? ChemInventoryId { get; set; }

        [ForeignKey("Order")]
        public int? OrderID { get; set; }
        public virtual Order Order { get; set; }

        [ForeignKey("Location")]
        public int? LocationID { get; set; }
        public virtual Location Location { get; set; }

        [ForeignKey("Chemical")]
        public int? ChemID { get; set; }
        public virtual Chemical Chemical { get; set; }

        [StringLength(50)]
        [Display(Name = "Location")]
        public string NormalizedLocation { get; set; }

        [StringLength(50)]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "CAT Number")]
        public string CAT { get; set; }

        [StringLength(50)]
        [Display(Name = "Lot #")]
        public string LOT { get; set; }

        [StringLength(50)]
        [Display(Name = "Units")]
        public string Units { get; set; }

        [Display(Name = "Qty Left")]
        public float QtyLeft { get; set; }

        [DataType(DataType.Date)]
        [DefaultValue("01/01/1900")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }

        public virtual ICollection<ChemLog> ChemLogs { get; set; }

    }
}
