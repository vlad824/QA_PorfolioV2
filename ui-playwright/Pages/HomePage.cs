namespace UiPlaywright.Tests.Pages;

public sealed class HomePage(IPage page)
{
    public ContactForm ContactForm { get; } = new(page);

    public ILocator MainHeading => page.GetByRole(AriaRole.Heading, new() { Level = 1 }).First;

    /// <summary>Opens the site root (BaseURL comes from the browser context options).</summary>
    public Task<IResponse?> OpenAsync() =>
        page.GotoAsync("/", new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
}
