using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Timers; 

public class AuthenticationService : BaseService, IAuthenticationService
{
	private string _token;
	private string _username;
	private string _password;
	private System.Timers.Timer _refreshTimer;

	public AuthenticationService(HttpClient httpClient) : base(httpClient) { }

	public async Task<string> AuthenticateAsync(string username, string password)
	{
		_username = username;
		_password = password;

		var user = new { Username = username, Password = password };
		var response = await _httpClient.PostAsJsonAsync("api/auth/login", user);
		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsAsync<dynamic>();
		_token = content.token;
		SetAuthorizationHeader(_token);

		StartTokenRefreshTimer();

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

	public void StartTokenRefreshTimer()
	{
		_refreshTimer = new System.Timers.Timer(TimeSpan.FromMinutes(115).TotalMilliseconds);
		_refreshTimer.Elapsed += async (sender, e) => await RefreshToken();
		_refreshTimer.AutoReset = false;
		_refreshTimer.Start();
	}

	public async Task RefreshToken()
	{
		if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
		{
			return;
		}

		try
		{
			var newToken = await AuthenticateAsync(_username, _password);
			SetToken(newToken);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Token refresh failed: {ex.Message}");
		}
	}
}
