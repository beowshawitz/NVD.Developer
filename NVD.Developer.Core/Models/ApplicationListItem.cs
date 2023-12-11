using Microsoft.EntityFrameworkCore.Update;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NVD.Developer.Core.Models
{
	public class ApplicationListItem
	{
		public int Id { get; set; }
		
		[Required]
		public string UserId { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Application")]
		public int ApplicationId { get; set; } = 0;

		public Application? Application { get; set; }

		public int? VersionId { get; set; } = null;
		public ApplicationVersion? Version { get; set; }

		[NotMapped]
		public bool IsSelected { get; set; } = false;

		[NotMapped]
		public string InstallType { 
			get
			{
				if (InstallAsUser)
					return "U";
				else 
					return "S";
			}
			set
			{ 
				if(value.Equals("U", StringComparison.InvariantCultureIgnoreCase))
				{
					InstallAsUser = true;
					InstallAsSystem = false;
				}
				if (value.Equals("S", StringComparison.InvariantCultureIgnoreCase))
				{
					InstallAsUser = false;
					InstallAsSystem = true;
				}
			} 
		}
		[NotMapped]
		public bool InstallAsSystem { get; set; } = false;
		[NotMapped]
		public bool InstallAsUser { get; set; } = true;

		[NotMapped]
		[Display(Name = "")]
		public bool InstallInteractive { get; set; } = false;
		[NotMapped]
		[Display(Name = "")]
		public bool InstallSilent { get; set; } = false;
		[NotMapped]
		[Display(Name = "")]
		public bool UninstallPrevious { get; set; } = false;
		[NotMapped]
		[Display(Name = "")]
		public bool AcceptAgreements { get; set; } = false;

		public ApplicationListItem() 
		{ 
		}

		public void ApplyUserSelections(ApplicationListItem? containsUserSelections, string installMode)
		{
			if (containsUserSelections != null)
			{
				this.IsSelected = containsUserSelections.IsSelected;
				this.InstallType = containsUserSelections.InstallType;
				this.InstallAsSystem = containsUserSelections.InstallAsSystem;
				this.InstallAsUser = containsUserSelections.InstallAsUser;
				this.UninstallPrevious = containsUserSelections.UninstallPrevious;
				this.AcceptAgreements = containsUserSelections.AcceptAgreements;
			}
			if(!string.IsNullOrEmpty(installMode))
			{
				if(installMode.Equals("I", StringComparison.InvariantCultureIgnoreCase))
				{
					this.InstallSilent = false;
					this.InstallInteractive = true;
				}
				if (installMode.Equals("S", StringComparison.InvariantCultureIgnoreCase))
				{
					this.InstallSilent = true;
					this.InstallInteractive = false;
				}
			}
		}

		public string GenerateScript()
		{
			StringBuilder script = new StringBuilder();
			if(Application != null) 
			{
				script.Append($"winget install --id={Application.Name} -e ");
				if (Version != null) 
				{
					script.Append($"-v \"{Version.Name}\" ");
				}
                if(InstallInteractive)
				{
					script.Append($"--interactive ");
				}
				if(InstallSilent)
				{
					script.Append($"--silent ");
				}
				if(InstallAsSystem)
				{
					script.Append($"--scope machine ");
				}
				if (InstallAsUser)
				{
					script.Append($"--scope user ");
				}
				if (AcceptAgreements)
				{
					script.Append($"--accept-package-agreements --accept-source-agreements ");
				}
				if (UninstallPrevious)
				{
					script.Append($"--uninstall-previous ");
				}
			}
			return script.ToString();
		}
	}
}
