using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVD.Developer.Core.Models
{
    public class PackageManager
    {
        public int Id { get; set; }

		[Required]
		[StringLength(150, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 1)]
		[Display(Name = "Name")]
		public string Name { get; set; } = string.Empty;

        public PackageManager()
        {

        }
    }
}
