using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Order ID")]
        public int? OrderID { get; set; }

        [ForeignKey("Vendor")]
        public int VendorID { get; set; }
        public virtual Vendor Vendor { get; set; }
        /*
        [StringLength(50)]
        [Display(Name = "Item Type")]
        public string Type { get; set; }
        */
        
        //[Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Invoice #")]
        public string Invoice { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "PO #")]
        public string PO { get; set; }

        [DataType(DataType.Date)]
        [DefaultValue("01/01/1900")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime Orderdate { get; set; }

        [DataType(DataType.Date)]
        [DefaultValue("01/01/1900")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Delivery Date")]
        public DateTime Recievedate { get; set; }

        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual ICollection<FileDetail> FileDetails { get; set; }
        public virtual ICollection<ChemEquipment> ChemEquipments { get; set; }
        public virtual ICollection<BioEquipment> BioEquipments { get; set; }
        public virtual ICollection<Animal> Animals { get; set; }
        public virtual ICollection<ChemInventory> ChemInventorys { get; set; }
    }
}
