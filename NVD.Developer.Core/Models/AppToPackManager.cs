using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace NVD.Developer.Core.Models
{
	public class AppToPackManager
	{
		public int Id { get; set; }
		[Required]
		[Display(Name = "Application")]
		public int ApplicationId { get; set; } = 0;
		public Application? Application { get; set; }

		[Required]
		[Display(Name = "Package Manager")]
		public int PackageManagerId { get; set; } = 0;
		public PackageManager? PackageManager { get; set; }
		public AppToPackManager() { }
	}
}
