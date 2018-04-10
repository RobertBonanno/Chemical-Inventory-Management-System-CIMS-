using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models
{
	public class Cage
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Display(Name = "Cage ID")]
		public int CageID { get; set; }

		[StringLength(50)]
		[Display(Name = "Cage Designation")]
		public string CageDesignation { get; set; }

		[ForeignKey("Location")]
		public int LocationID { get; set; }
		public virtual Location Location { get; set; }

		[StringLength(50)]
		[Display(Name = "Species")]
		public string Species { get; set; }

		[StringLength(50)]
		[Display(Name = "Location")]
		public string NormalizedLocation { get; set; }

		public virtual ICollection<CageLog> CageLogs { get; set; }
	}
}
