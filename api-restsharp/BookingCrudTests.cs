using System.Net;
using ApiRestSharp.Tests.Builders;
using ApiRestSharp.Tests.Models;

namespace ApiRestSharp.Tests;

[TestFixture]
[Category("Crud")]
public class BookingCrudTests : ApiTestBase
{
    [Test, Category("Smoke")]
    public async Task GetBookingIds_ReturnsJsonArrayOfIds()
    {
        // Act
        var response = await Api.GetBookingIdsAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), Describe(response));
        Assert.That(response.Data, Is.Not.Null, $"Body is not a JSON array of ids. {Describe(response)}");
        Assert.That(response.Data, Has.All.Matches<BookingIdItem>(item => item.BookingId > 0));
    }

    [Test, Category("Smoke")]
    public async Task CreateBooking_ValidPayload_ReturnsIdAndEchoesPayload()
    {
        // Arrange
        var payload = BookingBuilder.Valid().Build();

        // Act
        var response = await Api.CreateBookingAsync(payload);
        if (response.Data is not null) TrackForCleanup(response.Data.BookingId);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), Describe(response));
        Assert.That(response.Data, Is.Not.Null, Describe(response));
        Assert.Multiple(() =>
        {
            Assert.That(response.Data!.BookingId, Is.GreaterThan(0));
            Assert.That(response.Data.Booking, Is.EqualTo(payload));
        });
    }

    [Test]
    public async Task GetBooking_ExistingId_ReturnsStoredData()
    {
        // Arrange
        var payload = BookingBuilder.Valid().Build();
        var created = await CreateBookingAsync(payload);

        // Act
        var response = await Api.GetBookingAsync(created.BookingId);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), Describe(response));
        Assert.That(response.Data, Is.EqualTo(payload));
    }

    [Test]
    public async Task GetBookingIds_FilteredByName_ContainsCreatedBooking()
    {
        // Arrange
        var payload = BookingBuilder.Valid().Build();
        var created = await CreateBookingAsync(payload);

        // Act
        var response = await Api.GetBookingIdsAsync(payload.FirstName, payload.LastName);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), Describe(response));
        Assert.That(response.Data?.Select(item => item.BookingId), Does.Contain(created.BookingId));
    }

    [Test]
    public async Task UpdateBooking_WithValidToken_ReplacesAllFields()
    {
        // Arrange
        var created = await CreateBookingAsync();
        var replacement = BookingBuilder.Valid()
            .WithTotalPrice(999)
            .WithDepositPaid(false)
            .WithAdditionalNeeds("Late check-out")
            .Build();

        // Act
        var response = await Api.UpdateBookingAsync(created.BookingId, replacement, Token);

        // Assert: the response and a follow-up GET must both reflect the change.
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), Describe(response));
        Assert.That(response.Data, Is.EqualTo(replacement));

        var persisted = await Api.GetBookingAsync(created.BookingId);
        Assert.That(persisted.Data, Is.EqualTo(replacement), "Update was not persisted.");
    }

    [Test]
    public async Task PartialUpdateBooking_ChangesOnlySentFields()
    {
        // Arrange
        var original = BookingBuilder.Valid().Build();
        var created = await CreateBookingAsync(original);

        // Act
        var response = await Api.PartialUpdateBookingAsync(
            created.BookingId, new { firstname = "Patched", totalprice = 777 }, Token);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), Describe(response));
        Assert.That(response.Data, Is.EqualTo(original with { FirstName = "Patched", TotalPrice = 777 }));
    }

    [Test]
    public async Task DeleteBooking_WithValidToken_RemovesResource()
    {
        // Arrange
        var created = await CreateBookingAsync();

        // Act
        var deleteResponse = await Api.DeleteBookingAsync(created.BookingId, Token);
        var getResponse = await Api.GetBookingAsync(created.BookingId);

        // Assert: this API answers 201 (not 200/204) to a successful DELETE.
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created), Describe(deleteResponse));
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Deleted booking must not be readable.");
    }
}
