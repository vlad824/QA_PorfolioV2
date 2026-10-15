using UiPlaywright.Tests.Pages;
using UiPlaywright.Tests.TestData;

namespace UiPlaywright.Tests;

[TestFixture]
[Category("ContactForm")]
public class ContactFormTests : BaseUiTest
{
    private HomePage _home = null!;

    [SetUp]
    public async Task OpenHomePage()
    {
        _home = new HomePage(Page);
        await _home.OpenAsync();
    }

    [Test, Category("Smoke")]
    public async Task Submit_ValidMessage_ShowsConfirmationForSender()
    {
        // Arrange
        var message = ContactMessage.Valid();

        // Act
        await _home.ContactForm.SendAsync(message);

        // Assert
        await Expect(_home.ContactForm.ConfirmationFor(message.Name)).ToBeVisibleAsync();
    }

    [Test]
    public async Task Submit_EmptyForm_ShowsValidationErrors_AndNoConfirmation()
    {
        // Act
        await _home.ContactForm.SubmitAsync();

        // Assert
        await Expect(_home.ContactForm.AnyValidationError).ToBeVisibleAsync();
        await Expect(_home.ContactForm.AnyConfirmation).Not.ToBeVisibleAsync();
    }

    [TestCaseSource(nameof(InvalidMessages))]
    public async Task Submit_InvalidField_ShowsErrorForThatField(ContactMessage message, string field)
    {
        // Act
        await _home.ContactForm.SendAsync(message);

        // Assert
        await Expect(_home.ContactForm.ValidationErrorFor(field)).ToBeVisibleAsync();
        await Expect(_home.ContactForm.AnyConfirmation).Not.ToBeVisibleAsync();
    }

    private static IEnumerable<TestCaseData> InvalidMessages()
    {
        var valid = ContactMessage.Valid();
        yield return new TestCaseData(valid with { Phone = "123" }, "Phone").SetName("Phone_TooShort");
        yield return new TestCaseData(valid with { Subject = "abc" }, "Subject").SetName("Subject_TooShort");
        yield return new TestCaseData(valid with { Message = "too short" }, "Message").SetName("Message_TooShort");
    }
}
