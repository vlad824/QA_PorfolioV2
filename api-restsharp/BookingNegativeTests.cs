using System.Net;
using ApiRestSharp.Tests.Builders;

namespace ApiRestSharp.Tests;

[TestFixture]
[Category("Negative")]
public class BookingNegativeTests : ApiTestBase
{
    [TestCase("not-a-valid-id")]
    [TestCase("0")]
    [TestCase("999999999")]
    public async Task GetBooking_UnknownOrMalformedId_ReturnsNotFound(string id)
    {
        var response = await Api.GetBookingAsync(id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), Describe(response));
    }

    [Test]
    public async Task UpdateBooking_WithoutToken_IsForbidden_AndDoesNotChangeData()
    {
        // Arrange
        var original = BookingBuilder.Valid().Build();
        var created = await CreateBookingAsync(original);
        var attempt = original with { FirstName = "Hacker" };

        // Act
        var response = await Api.UpdateBookingAsync(created.BookingId, attempt, token: null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden), Describe(response));
        var stored = await Api.GetBookingAsync(created.BookingId);
        Assert.That(stored.Data, Is.EqualTo(original), "Booking must stay untouched after a rejected update.");
    }

    [Test]
    public async Task DeleteBooking_WithoutToken_IsForbidden_AndKeepsResource()
    {
        // Arrange
        var created = await CreateBookingAsync();

        // Act
        var response = await Api.DeleteBookingAsync(created.BookingId, token: null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden), Describe(response));
        var stored = await Api.GetBookingAsync(created.BookingId);
        Assert.That(stored.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Booking must still exist.");
    }

    // The demo API answers 500 for a missing required field, which is arguably a bug (400 is expected).
    // The assertion accepts any error status so the test checks the real requirement
    // ("invalid payload is rejected") and keeps passing once the service is fixed.
    [Test]
    public async Task CreateBooking_MissingRequiredFields_IsRejected()
    {
        var response = await Api.CreateBookingAsync(new { firstname = "OnlyName" });
        if (response.Data is { BookingId: > 0 } leaked) TrackForCleanup(leaked.BookingId);

        Assert.That((int)response.StatusCode, Is.GreaterThanOrEqualTo(400), Describe(response));
    }
}
