using ApiRestSharp.Tests.Config;
using ApiRestSharp.Tests.Models;
using RestSharp;

namespace ApiRestSharp.Tests.Clients;

/// <summary>
/// Thin typed wrapper over RestSharp. Contains no assertions: it only builds requests,
/// so tests stay readable and transport details live in one place.
/// </summary>
public sealed class BookerClient : IDisposable
{
    private readonly RestClient _client;

    public BookerClient(ApiSettings settings)
    {
        _client = new RestClient(new RestClientOptions(settings.BaseUrl) { MaxTimeout = settings.TimeoutMs });
        // Without Accept: application/json the API answers GET /booking/{id} with plain text.
        _client.AddDefaultHeader("Accept", "application/json");
    }

    public Task<RestResponse<TokenResponse>> CreateTokenAsync(string username, string password)
    {
        var request = new RestRequest("/auth", Method.Post)
            .AddJsonBody(new { username, password });
        return _client.ExecuteAsync<TokenResponse>(request);
    }

    public Task<RestResponse<List<BookingIdItem>>> GetBookingIdsAsync(string? firstName = null, string? lastName = null)
    {
        var request = new RestRequest("/booking", Method.Get);
        if (firstName is not null) request.AddQueryParameter("firstname", firstName);
        if (lastName is not null) request.AddQueryParameter("lastname", lastName);
        return _client.ExecuteAsync<List<BookingIdItem>>(request);
    }

    public Task<RestResponse<Booking>> GetBookingAsync(int id) => GetBookingAsync(id.ToString());

    /// <summary>String overload exists so negative tests can send malformed ids.</summary>
    public Task<RestResponse<Booking>> GetBookingAsync(string id)
    {
        var request = new RestRequest("/booking/{id}", Method.Get)
            .AddUrlSegment("id", id);
        return _client.ExecuteAsync<Booking>(request);
    }

    /// <param name="payload">A <see cref="Booking"/> or any anonymous object (used for invalid payloads).</param>
    public Task<RestResponse<CreateBookingResponse>> CreateBookingAsync(object payload)
    {
        var request = new RestRequest("/booking", Method.Post).AddJsonBody(payload);
        return _client.ExecuteAsync<CreateBookingResponse>(request);
    }

    public Task<RestResponse<Booking>> UpdateBookingAsync(int id, Booking booking, string? token)
    {
        var request = WithAuth(new RestRequest("/booking/{id}", Method.Put), token)
            .AddUrlSegment("id", id)
            .AddJsonBody(booking);
        return _client.ExecuteAsync<Booking>(request);
    }

    public Task<RestResponse<Booking>> PartialUpdateBookingAsync(int id, object patch, string? token)
    {
        var request = WithAuth(new RestRequest("/booking/{id}", Method.Patch), token)
            .AddUrlSegment("id", id)
            .AddJsonBody(patch);
        return _client.ExecuteAsync<Booking>(request);
    }

    public Task<RestResponse> DeleteBookingAsync(int id, string? token)
    {
        var request = WithAuth(new RestRequest("/booking/{id}", Method.Delete), token)
            .AddUrlSegment("id", id);
        return _client.ExecuteAsync(request);
    }

    private static RestRequest WithAuth(RestRequest request, string? token) =>
        token is null ? request : request.AddHeader("Cookie", $"token={token}");

    public void Dispose() => _client.Dispose();
}
