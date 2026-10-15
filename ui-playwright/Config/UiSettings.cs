namespace UiPlaywright.Tests.Config;

/// <summary>Override the target with UI_BASE_URL to run the same suite against another environment.</summary>
public sealed record UiSettings(string BaseUrl)
{
    public static UiSettings FromEnvironment() => new(
        Environment.GetEnvironmentVariable("UI_BASE_URL") is { Length: > 0 } url
            ? url
            : "https://automationintesting.online/");
}
