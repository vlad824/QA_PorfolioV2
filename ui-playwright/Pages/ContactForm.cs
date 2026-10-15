using System.Text.RegularExpressions;
using UiPlaywright.Tests.TestData;

namespace UiPlaywright.Tests.Pages;

/// <summary>Page-object component for the "Send us a message" form on the home page.</summary>
public sealed class ContactForm(IPage page)
{
    private ILocator NameInput => page.GetByTestId("ContactName");
    private ILocator EmailInput => page.GetByTestId("ContactEmail");
    private ILocator PhoneInput => page.GetByTestId("ContactPhone");
    private ILocator SubjectInput => page.GetByTestId("ContactSubject");
    private ILocator MessageInput => page.GetByTestId("ContactDescription");
    private ILocator SubmitButton => page.Locator("#submitContact");

    private const string ThanksText = "Thanks for getting in touch";

    public async Task FillAsync(ContactMessage message)
    {
        await NameInput.FillAsync(message.Name);
        await EmailInput.FillAsync(message.Email);
        await PhoneInput.FillAsync(message.Phone);
        await SubjectInput.FillAsync(message.Subject);
        await MessageInput.FillAsync(message.Message);
    }

    public Task SubmitAsync() => SubmitButton.ClickAsync();

    public async Task SendAsync(ContactMessage message)
    {
        await FillAsync(message);
        await SubmitAsync();
    }

    /// <summary>Confirmation addressed to the given sender.</summary>
    public ILocator ConfirmationFor(string senderName) => page.GetByText($"{ThanksText} {senderName}");

    public ILocator AnyConfirmation => page.GetByText(ThanksText);

    /// <summary>Any validation message (empty field or length violation).</summary>
    public ILocator AnyValidationError =>
        page.GetByText(new Regex("may not be blank|must be between", RegexOptions.IgnoreCase)).First;

    /// <summary>Validation message that mentions the given field, e.g. "Phone must be between ...".</summary>
    public ILocator ValidationErrorFor(string field) =>
        page.GetByText(new Regex($"{field}.*(may not be blank|must be between)", RegexOptions.IgnoreCase)).First;
}
