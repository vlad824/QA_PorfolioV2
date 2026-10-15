using ApiRestSharp.Tests.Builders;
using ApiRestSharp.Tests.Clients;
using ApiRestSharp.Tests.Config;
using ApiRestSharp.Tests.Models;
using RestSharp;

namespace ApiRestSharp.Tests;

public abstract class ApiTestBase
{
    private readonly List<int> _createdBookingIds = [];

    protected ApiSettings Settings { get; private set; } = null!;
    protected BookerClient Api { get; private set; } = null!;
    protected string Token { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Settings = ApiSettings.FromEnvironment();
        Api = new BookerClient(Settings);

        var auth = await Api.CreateTokenAsync(Settings.Username, Settings.Password);
        Assert.That(auth.Data?.Token, Is.Not.Null.And.Not.Empty,
            $"Could not obtain an auth token. {Describe(auth)}");
        Token = auth.Data!.Token!;
    }

    /// <summary>Every test removes the bookings it created, so the shared demo stays clean.</summary>
    [TearDown]
    public async Task DeleteCreatedBookings()
    {
        foreach (var id in _createdBookingIds)
            await Api.DeleteBookingAsync(id, Token); // already-deleted bookings answer 405; ignored on purpose

        _createdBookingIds.Clear();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => Api.Dispose();

    protected void TrackForCleanup(int bookingId) => _createdBookingIds.Add(bookingId);

    /// <summary>Arrange helper: creates a booking, asserts it worked and registers it for cleanup.</summary>
    protected async Task<CreateBookingResponse> CreateBookingAsync(Booking? booking = null)
    {
        var response = await Api.CreateBookingAsync(booking ?? BookingBuilder.Valid().Build());

        Assert.That(response.IsSuccessful && response.Data is not null, Is.True,
            $"Arrange failed: booking was not created. {Describe(response)}");

        TrackForCleanup(response.Data!.BookingId);
        return response.Data;
    }

    protected static string Describe(RestResponse response) =>
        $"HTTP {(int)response.StatusCode}; error: {response.ErrorMessage ?? "-"}; body: {response.Content}";
}
