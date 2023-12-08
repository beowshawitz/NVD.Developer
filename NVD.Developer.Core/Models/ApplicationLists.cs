using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVD.Developer.Core.Models
{
	public class ApplicationLists
	{
		public int Id { get; set; }

		[Display(Name = "List Item(s)")]
		public List<ApplicationListItem> Items { get; set; }

		[Display(Name = "User Identifier")]
		public string UserId { get; set; } = string.Empty;

		public ApplicationLists() 
		{
			Items = new List<ApplicationListItem>();
		}

		public ApplicationLists(string userId)
		{
			Items = new List<ApplicationListItem>();
			UserId = userId;
		}
	}
}
