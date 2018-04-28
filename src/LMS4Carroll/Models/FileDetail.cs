using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LMS4Carroll.Services;

namespace LMS4Carroll.Models
{
    public enum FileType
    {
        jpg = 1, pdf, doc, docx
    }

    public class FileDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "File ID")]
        [Required]
        public int FileDetailID { get; set; }

        [Display(Name = "File Name")]
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string FileName { get; set; }

        [Display(Name = "File Type")]
        [StringLength(100, MinimumLength = 2)]
        public string FileType { get; set; }

        [Required]
        [Display(Name = "File")]
        public byte[] File { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Content Type")]
        public string ContentType { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Date Created")]
        public DateTime DatetimeCreated { get; set; }

        [ForeignKey("Order")]
        [Display(Name = "Order ID")]
        [Required]
        public int? OrderID { get; set; }
        public virtual Order Order { get; set; }
    }
}
