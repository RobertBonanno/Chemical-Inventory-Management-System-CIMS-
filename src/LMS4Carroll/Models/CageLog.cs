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

        [ForeignKey("Animal")]
        public int AnimalID { get; set; }
        public virtual Animal Animal { get; set; }

        [Display(Name = "Food Served")]
        public Boolean Food { get; set; }

        [StringLength(150)]
        [Display(Name = "Food Comments")]
        public string FoodComments { get; set; }

        [Display(Name = "Washed")]
        public Boolean Washed { get; set; }

        [StringLength(150)]
        [Display(Name = "Wash Comments")]
        public string WashComments { get; set; }

        [Display(Name = "Cage Cleaned")]
        public Boolean Clean { get; set; }

        [Display(Name = "Socializing")]
        public Boolean Social { get; set; }

        [StringLength(150)]
        [Display(Name = "Socializing Comments")]
        public string SocialComments { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Date Created")]
        public DateTime DatetimeCreated { get; set; }
    }
}
