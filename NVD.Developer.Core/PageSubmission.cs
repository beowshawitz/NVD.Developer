using System.ComponentModel.DataAnnotations;

namespace NVD.Developer.Core
{
	public class PageSubmission
	{
		public int Start { get; set; } = 0;

		[Range(10, 100, ErrorMessage = "Page Size invalid (10-100).")]
		public int PageSize { get; set; } = 10;

		public string? Filter { get; set; } = string.Empty;

		[Required]
		public string SortBy { get; set; } = "name";

		[Required]
		public string SortDirection { get; set; } = "asc";

		public PageSubmission()
		{

		}

		public PageSubmission(int page)
		{
			Start = (PageSize * page) - PageSize;
		}
	}
}
