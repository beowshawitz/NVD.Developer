using NVD.Developer.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NVD.Developer.Core.Models
{
	public class ApplicationReport
	{
		public int Id { get; set; }

		[Required]
		[Display(Name = "User Identifier")]
		public string UserId { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Status")]
		public int StatusId { get; set; } = 1;

		public ApplicationReportStatus? Status { get; set; }

		[NotMapped]
		[Display(Name = "Requesting User")]		
		public string UserName { get; set; } = string.Empty;

		[NotMapped]
		[Display(Name = "User Contact Email")]
		public string UserContactEmail { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Application Name")]
		public string ApplicationName { get; set; } = string.Empty;

		[Display(Name = "Version Number")]
		public string? ApplicationVersion { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Reporting Reason")]
		public string Description { get; set; } = string.Empty;

		[Display(Name = "Date Created")]
		public DateTime DateCreated { get; set; }

		[Display(Name = "Date Updated")]
		public DateTime DateUpdated { get; set; }

		[NotMapped]
		public List<ApplicationReportComment> Comments { get; set; }

        public ApplicationReport()
		{
			Comments = new List<ApplicationReportComment>();
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
