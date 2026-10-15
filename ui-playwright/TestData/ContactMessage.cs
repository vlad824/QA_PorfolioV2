namespace UiPlaywright.Tests.TestData;

public sealed record ContactMessage(string Name, string Email, string Phone, string Subject, string Message)
{
    /// <summary>A message that passes the form validation; the name is unique per call.</summary>
    public static ContactMessage Valid() => new(
        Name: $"QA Auto {Guid.NewGuid():N}"[..16],
        Email: "qa.auto@example.com",
        Phone: "380501234567",
        Subject: "Automated portfolio check",
        Message: "This is a harmless automated message sent by a portfolio test run.");
}
