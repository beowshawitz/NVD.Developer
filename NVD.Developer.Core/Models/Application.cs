using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace NVD.Developer.Core.Models
{
    public class Application : IEquatable<Application>
    {
        public int Id { get; set; }

		[Required]
		[StringLength(150, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 1)]
		[Display(Name = "Name")]
		public string Name { get; set; } = string.Empty;

		[Required]
		[StringLength(250, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 1)]
		[Display(Name = "Display Name")]
		[Column("DisplayName")]
		public string DisplayName { get; set; } = string.Empty;

		[MaxLength(1000, ErrorMessage = "Your description is too long, the maximum is 1000 characters.")]
		[Display(Name = "Description")]
		public string? Description { get; set; }

		[Display(Name = "Application Image")]
		[MaxLength(500000, ErrorMessage ="The {0} file size cannot exceed 500kb.")]
		public byte[]? ImageData { get; set; }

		[Display(Name = "Is License Required")]
		[Column("LicenseRequired")]
		public bool IsLicenseRequired { get; set; }

		public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }


		[Display(Name = "Available Versions")]
		public List<ApplicationVersion> Versions { get; set; }
		public List<AppToPackManager> PackageManagers { get; set; }

        public Application()
        {
			Versions = new List<ApplicationVersion>();
            PackageManagers = new List<AppToPackManager>();
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public bool Equals(Application? other)
        {
            if (other == null)
                return false;
            return GetHashCode() == other.GetHashCode();
        }

		public bool HasVersion(string version)
		{
			bool exists = false;
			int i = 0;
			while(!exists && i< Versions.Count)
			{
				if (Versions[i].Name == version)
				{
					exists = true;
				}
				i++;
			}
			return exists;
		}

		public bool HasVersion(int versionId)
		{
			bool exists = false;
			int i = 0;
			while (!exists && i < Versions.Count)
			{
				if (Versions[i].Id == versionId)
				{
					exists = true;
				}
				i++;
			}
			return exists;
		}

		public string GetVersionName(int versionId)
		{
			if(versionId > 0 && Versions != null && Versions.Exists(x=>x.Id.Equals(versionId)))
			{
				return Versions.Find(x => x.Id.Equals(versionId)).Name;
			}
			else
			{
				return string.Empty;
			}
		}

		public bool IsValid()
        {
            bool isValid = true;
            if(string.IsNullOrEmpty(Name) || Name.Length > 150) 
            {
				isValid = false;
			}
			if (string.IsNullOrEmpty(DisplayName) || DisplayName.Length > 250)
			{
				isValid = false;
			}
			if (!string.IsNullOrEmpty(Description) && Description.Length > 1000)
			{
				isValid = false;
			}
			if(Versions != null && Versions.Count > 0)
			{
				foreach (var version in Versions) 
				{
					if (string.IsNullOrEmpty(version.Name) || version.Name.Length > 150)
					{
						isValid = false;
					}
				}
			}
			return isValid;
        }
    }
}