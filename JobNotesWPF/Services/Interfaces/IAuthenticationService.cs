using System.Threading.Tasks;

public interface IAuthenticationService
{
	
	Task<string> AuthenticateAsync(string username, string password);
	
	void SetToken(string token);
	
	string GetToken();

}
