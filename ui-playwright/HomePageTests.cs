using System.Text.RegularExpressions;
using UiPlaywright.Tests.Pages;

namespace UiPlaywright.Tests;

[TestFixture]
[Category("Smoke")]
public class HomePageTests : BaseUiTest
{
    private HomePage _home = null!;

    [SetUp]
    public void CreatePageObject() => _home = new HomePage(Page);

    [Test]
    public async Task Open_ReturnsSuccessfulResponseAndTitle()
    {
        var response = await _home.OpenAsync();

        Assert.That(response, Is.Not.Null, "Navigation should return an HTTP response.");
        Assert.That(response!.Ok, Is.True, $"Expected a 2xx status but got {response.Status}.");
        await Expect(Page).ToHaveTitleAsync(new Regex(@"\S"));
    }

    [Test]
    public async Task Open_ShowsMainHeadingWithText()
    {
        await _home.OpenAsync();

        await Expect(_home.MainHeading).ToBeVisibleAsync();
        await Expect(_home.MainHeading).ToHaveTextAsync(new Regex(@"\S"));
    }
}
