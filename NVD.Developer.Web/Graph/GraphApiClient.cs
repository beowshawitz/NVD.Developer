using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace NVD.Developer.Web.Graph
{
	public class GraphApiClient
	{
		private readonly GraphServiceClient _graphServiceClient;

		public GraphApiClient(GraphServiceClient graphServiceClient, IConfiguration configuration)
		{
			_graphServiceClient = graphServiceClient;
		}

		public async Task<string> GetGraphApiUserId()
		{
			var user = await _graphServiceClient.Me.GetAsync(requestConfiguration =>
			{
				requestConfiguration.QueryParameters.Select =
					new string[] { "id" };
			});
			return user != null ? user.Id : string.Empty;
		}

		public async Task<User?> GetGraphApiUser()
		{
			return await _graphServiceClient.Me.GetAsync(requestConfiguration =>
			{
				requestConfiguration.QueryParameters.Select =
					new string[] { "id", "displayName", "mail", "userPrincipalName", "mobilePhone" };
			});
		}

		public async Task<User?> GetGraphApiUser(string userId)
		{
			if (string.IsNullOrEmpty(userId))
				return null;
			return await _graphServiceClient.Users[userId].GetAsync(requestConfiguration =>
			{
				requestConfiguration.QueryParameters.Select =
					new string[] { "id", "displayName", "mail", "userPrincipalName", "mobilePhone" };
			});
		}

		public async Task<IList<User>> GetUsers()
		{
			var userCol = await _graphServiceClient.Users.GetAsync(requestConfiguration =>
			{
				requestConfiguration.QueryParameters.Select =
					new string[] { "id", "displayName", "mail", "userPrincipalName", "mobilePhone" };
			});
			if (userCol != null)
			{
				List<User> users = new List<User>();
				var pageIterator = PageIterator<User, UserCollectionResponse>.CreatePageIterator(_graphServiceClient, userCol,
					// Callback executed for each item in
					// the collection
					(user) =>
					{
						users.Add(user);
						return true;
					});

					await pageIterator.IterateAsync();
				return users;
			}
			else
			{
				return new List<User>();
			}
		}


		public async Task<string> GetGraphApiProfilePhoto()
		{
			var photo = string.Empty;
			// Get user photo
			using (var photoStream = await _graphServiceClient.Me.Photo.Content.GetAsync().ConfigureAwait(false))
			{
				byte[] photoByte = ((MemoryStream)photoStream).ToArray();
				photo = Convert.ToBase64String(photoByte);
			}

			return photo;
		}
	}
}
