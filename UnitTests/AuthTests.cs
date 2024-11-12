using CompanyName.Core.Auth;

namespace UnitTests;

[TestClass]
public class AuthTests
{
	private const string USER_NAME = "user";
	private const string CORRECT_PASSWORD = "pass";
	private const string WRONG_PASSWORD = "password";

	AuthManager _sut = null!;

	[TestInitialize]
	public void Initialize()
	{
		_sut = CreateSubjectUnderTest();
	}

	[TestCleanup]
	public void Cleanup()
	{
		_sut.Dispose();
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


	private static AuthManager CreateSubjectUnderTest()
	{
		var sut = new AuthManager();

		Assert.AreEqual("", sut.User);

		return sut;
	}
}