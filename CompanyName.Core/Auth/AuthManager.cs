namespace CompanyName.Core.Auth;

public class AuthManager : Manager, IAuthManager
{
	public event EventHandler<string>? UserChanged;

	public string User { get; private set; } = "";

	public bool Login(string user, string password)
	{
		var success = user != "" && password != "" && user.Length == password.Length;

		if (!success)
			return success;

		User = user;
		UserChanged?.Invoke(this, User);

		return success;
	}

	public bool Logout()
	{
		if (User == "")
			return false;

		User = "";
		UserChanged?.Invoke(this, User);

		return true;
	}
}