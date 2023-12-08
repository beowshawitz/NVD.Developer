using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVD.Developer.Core.Models
{
	public class ApplicationRequestStatus
	{
		public int Id { get; set; }

		[Required]
		public string StatusName { get; set; } = string.Empty;
	}
}
