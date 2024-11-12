namespace CompanyName.Core.Auth;

public interface IAuthManager
{
    event EventHandler<string> UserChanged;

    public string User { get; }

    public bool Login(string user, string password);

    public bool Logout();
}