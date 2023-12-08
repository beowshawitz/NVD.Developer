using System.Text.Json.Serialization;

namespace NVD.Developer.Apps.Shared
{
	public class AppDeployment : IEquatable<AppDeployment>
	{
        [JsonPropertyName("_id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("img")]
        public string? Image { get; set; }

		public bool IsValid { 
			get
			{
				return !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Name);
			}
		}

		public AppDeployment()
		{
			
		}

		public bool Equals(AppDeployment? other)
		{
			return IsValid && other != null && other.IsValid &&
				Id.Equals(other.Id, StringComparison.InvariantCultureIgnoreCase) && Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}