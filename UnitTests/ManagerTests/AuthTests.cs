using CompanyName.Core.Auth;

namespace UnitTests.ManagerTests;

[TestClass]
public class AuthTests : ManagerTests<AuthManager>
{
	private const string USER_NAME = "user";
	private const string CORRECT_PASSWORD = "pass";
	private const string WRONG_PASSWORD = "password";

	protected override void InitialAsserts()
	{
		Assert.AreEqual("", _sut.User);
	}

	[TestMethod]
	public void Login_WrongPassword_NotLoggedIn()
	{
		_sut.Login(USER_NAME, WRONG_PASSWORD);

		Assert.AreEqual("", _sut.User);
	}

	[TestMethod]
	public void Login_CorrectPassword_LoggedIn()
	{
		_sut.Login(USER_NAME, CORRECT_PASSWORD);

		Assert.AreEqual(USER_NAME, _sut.User);
	}

	[TestMethod]
	public void Logout_AfterLogin_LoggedOut()
	{
		_sut.Login(USER_NAME, CORRECT_PASSWORD);

		Assert.AreEqual(USER_NAME, _sut.User);

		_sut.Logout();

		Assert.AreEqual("", _sut.User);
	}
}