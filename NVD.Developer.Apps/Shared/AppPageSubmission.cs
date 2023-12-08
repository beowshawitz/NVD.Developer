using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVD.Developer.Apps.Shared
{
	public class AppPageSubmission
	{
		public int Start { get; set; } = 0;

		[Range(10, 100, ErrorMessage = "Page Size invalid (10-100).")]
		public int PageSize { get; set; } = 10;

		public string? Filter { get; set; } = string.Empty;

		[Required]
		public string SortBy { get; set; } = "name";

		[Required]
		public string SortDirection { get; set; } = "asc";
	}
}
