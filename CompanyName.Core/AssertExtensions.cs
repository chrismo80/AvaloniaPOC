namespace CompanyName.Core;

public static class AssertExtensions
{
    public static void IsTrue(this object me, bool condition, string message)
    {
        if (!condition)
            throw new AssertException(message);
    }
}

public class AssertException(string message) : Exception(message) { }