using UiPlaywright.Tests.Config;

namespace UiPlaywright.Tests;

/// <summary>
/// Shared setup on top of Playwright's <see cref="PageTest"/>: one isolated browser context per test
/// (cheap, parallel-safe) plus a trace that is kept only when the test fails.
/// </summary>
[Parallelizable(ParallelScope.Self)]
public abstract class BaseUiTest : PageTest
{
    protected UiSettings Settings { get; } = UiSettings.FromEnvironment();

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = Settings.BaseUrl,
        ViewportSize = new ViewportSize { Width = 1440, Height = 900 },
        Locale = "en-GB"
    };

    [OneTimeSetUp]
    public void ConfigureTimeouts() => SetDefaultExpectTimeout(10_000);

    [SetUp]
    public async Task StartTracing()
    {
        Page.SetDefaultNavigationTimeout(30_000);
        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TearDown]
    public async Task StopTracing()
    {
        var failed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed;
        if (!failed)
        {
            await Context.Tracing.StopAsync(); // discard: nothing to investigate
            return;
        }

        var path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "traces", $"{SafeFileName()}.zip");
        await Context.Tracing.StopAsync(new TracingStopOptions { Path = path });
        TestContext.AddTestAttachment(path, "Playwright trace (open with: pwsh playwright.ps1 show-trace <file>)");
    }

    private static string SafeFileName()
    {
        var invalid = Path.GetInvalidFileNameChars();
        var name = new string(TestContext.CurrentContext.Test.Name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        if (name.Length > 80) name = name[..80];
        return $"{name}-{Guid.NewGuid():N}";
    }
}
