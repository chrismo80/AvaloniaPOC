using CompanyName.Core.Auth;

namespace UnitTests.ServiceTests;

[TestClass]
public class AuthTests : ServiceTests<AuthManager>
{
    private const string USER_NAME = "user";
    private const string CORRECT_PASSWORD = "pass";
    private const string WRONG_PASSWORD = "password";

    [TestMethod]
    public void Login_NoUserNoPassword_NotLoggedIn()
    {
        var success = Sut.Login("", "");

        success.Is(false);
        Sut.User.Is("");
    }

    [TestMethod]
    public void Login_NoUserButPassword_NotLoggedIn()
    {
        var success = Sut.Login("", CORRECT_PASSWORD);

        success.Is(false);
        Sut.User.Is("");
    }

    [TestMethod]
    public void Login_UserButNoPassword_NotLoggedIn()
    {
        var success = Sut.Login(USER_NAME, "");

        success.Is(false);
        Sut.User.Is("");
    }

    [TestMethod]
    public void Login_WrongPassword_NotLoggedIn()
    {
        var success = Sut.Login(USER_NAME, WRONG_PASSWORD);

        success.Is(false);
        Sut.User.Is("");
    }

    [TestMethod]
    public void Login_CorrectPassword_LoggedIn()
    {
        var success = Sut.Login(USER_NAME, CORRECT_PASSWORD);

        success.Is(true);
        Sut.User.Is(USER_NAME);
    }

    [TestMethod]
    public void Logout_AfterLogin_LoggedOut()
    {
        var success = Sut.Login(USER_NAME, CORRECT_PASSWORD);

        success.Is(true);
        Sut.User.Is(USER_NAME);

        success = Sut.Logout();

        success.Is(true);
        Sut.User.Is("");
    }

    [TestMethod]
    public void Logout_WithoutLogin_NoException()
    {
        var success = Sut.Logout();

        success.Is(false);
        Sut.User.Is("");
    }

    protected override void Pre()
    {
        Sut.User.Is("");
    }
}