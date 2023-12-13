using NVD.Developer.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NVD.Developer.Core.Models
{
	public class ApplicationReportComment
    {
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;

		[Required]
		public int ReportId { get; set; } = 0;

		[NotMapped] 
		public ApplicationReport? Report { get; set; }

		[NotMapped]
		[Display(Name = "Author")]		
		public string Author { get; set; } = string.Empty;

		[NotMapped]
		[Display(Name = "Author Email")]
		public string AuthorEmail { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Comment")]
		public string Comment { get; set; } = string.Empty;

		[Display(Name = "Date Created")]
		public DateTime DateCreated { get; set; }

		[Display(Name = "Date Updated")]
		public DateTime DateUpdated { get; set; }

		public ApplicationReportComment()
		{
		}	

		public string GetPrettyDateCreated()
		{
			return DateCreated.GetPrettyDate();
		}

		public string GetPrettyDateUpdated()
		{
			return DateUpdated.GetPrettyDate();
		}
	}
}
