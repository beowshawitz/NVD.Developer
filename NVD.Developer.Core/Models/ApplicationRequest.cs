using NVD.Developer.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NVD.Developer.Core.Models
{
	public class ApplicationRequest
	{
		public int Id { get; set; }

		[Required]
		[Display(Name = "User Identifier")]
		public string UserId { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Status")]
		public int StatusId { get; set; } = 1;

		public ApplicationRequestStatus? Status { get; set; }

		[Required]
		[Display(Name = "Requesting User")]
		[Column("DisplayName")]
		public string UserName { get; set; } = string.Empty;
		
		[Required]
		[Display(Name = "User Contact Number")]
		[Column("ContactNumber")]
		public string UserContactNumber { get; set; } = string.Empty;

		[Required]
		[Display(Name = "User Contact Email")]
		[Column("ContactEmail")]
		public string UserContactEmail { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Application Name")]
		public string ApplicationName { get; set; } = string.Empty;

		[Display(Name = "Version Number")]
		public string? ApplicationVersion { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Reason")]
		public string RequestingReason { get; set; } = string.Empty;

		[Display(Name = "Date Created")]
		public DateTime DateCreated { get; set; }

		[Display(Name = "Date Updated")]
		public DateTime DateUpdated { get; set; }

		public ApplicationRequest()
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
