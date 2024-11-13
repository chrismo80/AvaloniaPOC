using CompanyName.Core.Auth;

namespace UnitTests.ManagerTests;

[TestClass]
public class AuthTests : ManagerTests<AuthManager>
{
	private const string USER_NAME = "user";
	private const string CORRECT_PASSWORD = "pass";
	private const string WRONG_PASSWORD = "password";

	protected override void VerifyInitialState()
	{
		Assert.AreEqual("", Sut.User);
	}

	[TestMethod]
	public void Login_NoUserNoPassword_NotLoggedIn()
	{
		var success = Sut.Login("", "");

		Assert.IsFalse(success);
		Assert.AreEqual("", Sut.User);
	}

	[TestMethod]
	public void Login_NoUserButPassword_NotLoggedIn()
	{
		var success = Sut.Login("", CORRECT_PASSWORD);

		Assert.IsFalse(success);
		Assert.AreEqual("", Sut.User);
	}

	[TestMethod]
	public void Login_UserButNoPassword_NotLoggedIn()
	{
		var success = Sut.Login(USER_NAME, "");

		Assert.IsFalse(success);
		Assert.AreEqual("", Sut.User);
	}

	[TestMethod]
	public void Login_WrongPassword_NotLoggedIn()
	{
		var success = Sut.Login(USER_NAME, WRONG_PASSWORD);

		Assert.IsFalse(success);
		Assert.AreEqual("", Sut.User);
	}

	[TestMethod]
	public void Login_CorrectPassword_LoggedIn()
	{
		var success = Sut.Login(USER_NAME, CORRECT_PASSWORD);

		Assert.IsTrue(success);
		Assert.AreEqual(USER_NAME, Sut.User);
	}

	[TestMethod]
	public void Logout_AfterLogin_LoggedOut()
	{
		var success = Sut.Login(USER_NAME, CORRECT_PASSWORD);

		Assert.IsTrue(success);
		Assert.AreEqual(USER_NAME, Sut.User);

		success = Sut.Logout();

		Assert.IsTrue(success);
		Assert.AreEqual("", Sut.User);
	}

	[TestMethod]
	public void Logout_WithoutLogin_NoException()
	{
		var success = Sut.Logout();

		Assert.IsFalse(success);
		Assert.AreEqual("", Sut.User);
	}
}