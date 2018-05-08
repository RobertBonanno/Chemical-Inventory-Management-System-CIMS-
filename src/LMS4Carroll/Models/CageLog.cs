using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    public class CageLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Log ID")]
        public int CageLogId { get; set; }

		[Required]
		[ForeignKey("Cage")]
        public int CageID { get; set; }
        public virtual Cage Cage { get; set; }

        [Required]
        [Display(Name = "Food Served")]
        public Boolean Food { get; set; }

        [Required]
        [Display(Name = "Washed")]
        public Boolean Washed { get; set; }

        [Required]
        [Display(Name = "Cage Cleaned")]
        public Boolean Clean { get; set; }

        [Required]
        [Display(Name = "Socializing")]
        public Boolean Social { get; set; }

        [StringLength(150)]
        [Display(Name = "Noteworthy Comments")]
        public string NoteworthyComments { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Date Created")]
        public DateTime DatetimeCreated { get; set; }
    }
}
