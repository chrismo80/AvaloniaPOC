using CompanyName.Core.Auth;
using Is;

namespace CompanyName.Core.Tests.ServiceTests;

[TestClass]
public class AuthTests : ServiceTests<AuthManager>
{
	private const string USER_NAME = "user";
	private const string CORRECT_PASSWORD = "pass";
	private const string WRONG_PASSWORD = "password";

	[TestMethod]
	public void Login_NoUserNoPassword_NotLoggedIn()
	{
		Sut.Login("", "").Is(false);
		Sut.User.Is("");
	}

	[TestMethod]
	public void Login_NoUserButPassword_NotLoggedIn()
	{
		Sut.Login("", CORRECT_PASSWORD).Is(false);
		Sut.User.Is("");
	}

	[TestMethod]
	public void Login_UserButNoPassword_NotLoggedIn()
	{
		Sut.Login(USER_NAME, "").Is(false);
		Sut.User.Is("");
	}

	[TestMethod]
	public void Login_WrongPassword_NotLoggedIn()
	{
		Sut.Login(USER_NAME, WRONG_PASSWORD).Is(false);
		Sut.User.Is("");
	}

	[TestMethod]
	public void Login_CorrectPassword_LoggedIn()
	{
		Sut.Login(USER_NAME, CORRECT_PASSWORD).Is(true);
		Sut.User.Is(USER_NAME);
	}

	[TestMethod]
	public void Logout_AfterLogin_LoggedOut()
	{
		Sut.Login(USER_NAME, CORRECT_PASSWORD).Is(true);
		Sut.User.Is(USER_NAME);

		Sut.Logout().Is(true);
		Sut.User.Is("");
	}

	[TestMethod]
	public void Logout_WithoutLogin_NoException()
	{
		Sut.Logout().Is(false);
		Sut.User.Is("");
	}

	protected override void Pre()
	{
		Sut.User.Is("");
	}
}