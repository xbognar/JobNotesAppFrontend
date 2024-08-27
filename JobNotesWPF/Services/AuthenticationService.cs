using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class AuthenticationService : BaseService, IAuthenticationService
{
	private string _token;

	public AuthenticationService(HttpClient httpClient) : base(httpClient) { }

	public async Task<string> AuthenticateAsync(string username, string password)
	{
		var user = new { Username = username, Password = password };
		var response = await _httpClient.PostAsJsonAsync("api/auth/login", user);
		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsAsync<dynamic>();
		_token = content.Token;
		SetAuthorizationHeader(_token);
		return _token;
	}

	public void SetToken(string token)
	{
		_token = token;
		SetAuthorizationHeader(_token);
	}

	public string GetToken()
	{
		return _token;
	}
}
