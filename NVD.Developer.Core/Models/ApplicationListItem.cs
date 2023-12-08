using Microsoft.EntityFrameworkCore.Update;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NVD.Developer.Core.Models
{
	public enum InstallAs { System, User };
	public enum InstallMode { Silent, Interactive };
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
		public InstallAs InstallType { get; set; }

		[NotMapped]
		public InstallMode InstallMode { get; set; }

		[NotMapped]
		public bool UninstallPrevious { get; set; }

		public ApplicationListItem() 
		{ 
		}

		public void ApplyInstallAsFromForm(string value)
		{
			if(!string.IsNullOrEmpty(value))
			{
				if (value.Equals("U", StringComparison.InvariantCultureIgnoreCase))
				{
					InstallType = InstallAs.User;
				}
				else if (value.Equals("S", StringComparison.InvariantCultureIgnoreCase))
				{
					InstallType = InstallAs.System;
				}
				else
				{
					InstallType = InstallAs.System;
				}
			}
			else
			{
				InstallType = InstallAs.System;
			}
			
		}

		public void ApplyInstallModeFromForm(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (value.Equals("I", StringComparison.InvariantCultureIgnoreCase))
				{
					InstallMode = InstallMode.Interactive;
				}
				else if (value.Equals("S", StringComparison.InvariantCultureIgnoreCase))
				{
					InstallMode = InstallMode.Silent;
				}
				else
				{
					InstallMode = InstallMode.Silent;
				}
			}
			else
			{
				InstallMode = InstallMode.Silent;
			}			
		}

		public void ApplyUninstallPreviousFromForm(string value)
		{
			if (!string.IsNullOrEmpty(value) && (value.Equals("checked", StringComparison.InvariantCultureIgnoreCase) || value.Equals("true", StringComparison.InvariantCultureIgnoreCase)))
			{
				UninstallPrevious = true;
			}
			else
			{
				UninstallPrevious = false;
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
                if(InstallMode == InstallMode.Interactive)
				{
					script.Append($"--interactive ");
				}
				if(InstallMode == InstallMode.Silent)
				{
					script.Append($"--silent ");
				}
				if(InstallType == InstallAs.System)
				{
					script.Append($"--scope machine ");
				}
				if (InstallType == InstallAs.User)
				{
					script.Append($"--scope user ");
				}
				if(UninstallPrevious)
				{
					script.Append($"--uninstall-previous ");
				}
			}
			return script.ToString();
		}
	}
}
