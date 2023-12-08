using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace NVD.Developer.Core.Models
{
	public class ApplicationVersion
	{
		public int Id { get; set; }
		
		[Required]
		[Display(Name = "Version Number")]
		public string Name { get; set; } = string.Empty;

		[Required]
		public int ApplicationId { get; set; } = 0;

		[NotMapped]
		public Application? Application { get; set; }

		public ApplicationVersion() 
		{ 
		}
	}
}
