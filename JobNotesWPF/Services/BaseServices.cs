using System.Net.Http.Headers;
using System.Net.Http;

public abstract class BaseService
{
	protected readonly HttpClient _httpClient;

	public BaseService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public void SetAuthorizationHeader(string token)
	{
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
	}

}
