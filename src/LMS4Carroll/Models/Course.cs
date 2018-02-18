using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Course ID")]
        public int CourseID { get; set; }

        [ForeignKey("Location")]
        [Display(Name = "Lab Location")]
        public int? LocationID { get; set; }
        public virtual Location Location { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Course Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Course Number")]
        public string Number { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Course Department")]
        public string Department { get; set; }

        [StringLength(50)]
        [Display(Name = "NormalizedStr")]
        public string NormalizedStr { get; set; }

        [StringLength(50)]
        [Display(Name = "Location")]
        public string NormalizedLocation { get; set; }

        [StringLength(50)]
        [Display(Name = "Course Leader")]
        public string Handler { get; set; }

        public virtual ICollection<ChemLog> ChemLogs { get; set; }

    }
}
