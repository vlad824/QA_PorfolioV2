using System.Text.Json.Serialization;

namespace ApiRestSharp.Tests.Models;

// Property names are mapped explicitly: the API uses all-lowercase names that do not match
// camelCase serialization. Records give value equality for payload-vs-response asserts.

public sealed record BookingDates(
    [property: JsonPropertyName("checkin")] DateOnly CheckIn,
    [property: JsonPropertyName("checkout")] DateOnly CheckOut);

public sealed record Booking(
    [property: JsonPropertyName("firstname")] string FirstName,
    [property: JsonPropertyName("lastname")] string LastName,
    [property: JsonPropertyName("totalprice")] int TotalPrice,
    [property: JsonPropertyName("depositpaid")] bool DepositPaid,
    [property: JsonPropertyName("bookingdates")] BookingDates BookingDates,
    [property: JsonPropertyName("additionalneeds"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? AdditionalNeeds = null);

public sealed record CreateBookingResponse(
    [property: JsonPropertyName("bookingid")] int BookingId,
    [property: JsonPropertyName("booking")] Booking Booking);

public sealed record BookingIdItem(
    [property: JsonPropertyName("bookingid")] int BookingId);

public sealed record TokenResponse(
    [property: JsonPropertyName("token")] string? Token,
    [property: JsonPropertyName("reason")] string? Reason);
