namespace CompanyName.Core.Auth;

public class AuthManager : BaseService, IAuthManager
{
	public event EventHandler<string>? UserChanged;

	public string User { get; private set; } = "";

	public bool Login(string user, string password)
	{
		var success = ValidateCredentials(user, password);

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

	private static bool ValidateCredentials(string user, string password) =>
		user.Length == password.Length && password != "";
}