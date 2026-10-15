namespace ApiRestSharp.Tests.Config;

/// <summary>
/// Test configuration. Every value can be overridden with an environment variable;
/// the defaults point to the public Restful-Booker demo (its credentials are published in the API docs).
/// </summary>
public sealed record ApiSettings(string BaseUrl, string Username, string Password, int TimeoutMs)
{
    public static ApiSettings FromEnvironment() => new(
        BaseUrl: Env("BOOKER_BASE_URL", "https://restful-booker.herokuapp.com"),
        Username: Env("BOOKER_USERNAME", "admin"),
        Password: Env("BOOKER_PASSWORD", "password123"),
        TimeoutMs: int.TryParse(Environment.GetEnvironmentVariable("BOOKER_TIMEOUT_MS"), out var ms) ? ms : 15_000);

    private static string Env(string name, string fallback) =>
        Environment.GetEnvironmentVariable(name) is { Length: > 0 } value ? value : fallback;
}
