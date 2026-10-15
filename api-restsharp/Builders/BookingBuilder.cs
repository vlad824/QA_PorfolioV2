using ApiRestSharp.Tests.Models;

namespace ApiRestSharp.Tests.Builders;

/// <summary>Test data builder: valid by default, unique names so parallel runs never collide.</summary>
public sealed class BookingBuilder
{
    private string _firstName = $"First{Guid.NewGuid():N}"[..13];
    private string _lastName = $"Last{Guid.NewGuid():N}"[..12];
    private int _totalPrice = 150;
    private bool _depositPaid = true;
    private DateOnly _checkIn = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7);
    private DateOnly _checkOut = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10);
    private string? _additionalNeeds = "Breakfast";

    public static BookingBuilder Valid() => new();

    public BookingBuilder WithFirstName(string value) { _firstName = value; return this; }
    public BookingBuilder WithLastName(string value) { _lastName = value; return this; }
    public BookingBuilder WithTotalPrice(int value) { _totalPrice = value; return this; }
    public BookingBuilder WithDepositPaid(bool value) { _depositPaid = value; return this; }
    public BookingBuilder WithAdditionalNeeds(string? value) { _additionalNeeds = value; return this; }

    public BookingBuilder WithDates(DateOnly checkIn, DateOnly checkOut)
    {
        _checkIn = checkIn;
        _checkOut = checkOut;
        return this;
    }

    public Booking Build() => new(
        _firstName, _lastName, _totalPrice, _depositPaid,
        new BookingDates(_checkIn, _checkOut), _additionalNeeds);
}
