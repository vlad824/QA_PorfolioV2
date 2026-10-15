using System.Net;

namespace ApiRestSharp.Tests;

[TestFixture]
[Category("Auth")]
public class AuthApiTests : ApiTestBase
{
    [Test, Category("Smoke")]
    public async Task CreateToken_ValidCredentials_ReturnsToken()
    {
        var response = await Api.CreateTokenAsync(Settings.Username, Settings.Password);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), Describe(response));
        Assert.That(response.Data?.Token, Is.Not.Null.And.Not.Empty);
    }

    // Known API quirk: bad credentials are answered with HTTP 200 and a "reason" field instead of 401.
    // The test pins the observable contract (no token is issued) rather than the questionable status code.
    [TestCase("admin", "wrong-password")]
    [TestCase("wrong-user", "password123")]
    [TestCase("", "")]
    public async Task CreateToken_InvalidCredentials_DoesNotIssueToken(string username, string password)
    {
        var response = await Api.CreateTokenAsync(username, password);

        Assert.That(response.Data?.Token, Is.Null.Or.Empty, $"No token expected. {Describe(response)}");
        Assert.That(response.Data?.Reason, Is.Not.Null.And.Not.Empty, "A rejection reason is expected.");
    }
}
