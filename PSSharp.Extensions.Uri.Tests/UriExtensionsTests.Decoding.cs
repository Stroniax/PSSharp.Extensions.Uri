namespace PSSharp.Extensions.Uri;

public sealed partial class UriExtensionsTests
{
    [Theory]
    [InlineData("some%20value", 10)]
    [InlineData("some%25value", 10)]
    [InlineData("some%25value%20with%23tokens%61valid", 28)]
    [InlineData("The quick brown fox jumps over the lazy dog.", 44)]
    public void CalculateLengthWhenDecodedWellFormed_returns_decoded_length(
        string query,
        int expected
    )
    {
        var length = UriExtensions.CaclucateLengthWhenDecodedWellFormed(query);

        Assert.Equal(expected, length);
    }

    [Theory]
    [InlineData("some%20value", 10)]
    [InlineData("some%25value", 10)]
    [InlineData("some%25value%20with%23tokens%61valid", 28)]
    [InlineData("The quick brown fox jumps over the lazy dog.", 44)]
    public void CalculateLengthWhenDecoded_when_well_formed_returns_decoded_length(
        string query,
        int expected
    )
    {
        var length = UriExtensions.CalculateLengthWhenDecoded(query);

        Assert.Equal(expected, length);
    }

    [Theory]
    [InlineData("some%agvalue", 12)]
    [InlineData("some%gavalue", 12)]
    [InlineData("some%abvalue%", 11)]
    [InlineData("some%abvalue%1", 12)]
    [InlineData("some%abvalue%12", 11)]
    [InlineData("some%abvalue%hi", 13)]
    public void CalculateLengthWhenDecoded_not_well_formed_returns_decoded_length(
        string query,
        int expected
    )
    {
        var length = UriExtensions.CalculateLengthWhenDecoded(query);

        Assert.Equal(expected, length);
    }

    [Theory]
    [InlineData("some%agvalue", 10)]
    [InlineData("some%20value", 9)]
    [InlineData("bad%agencoding", 12)]
    public void TryUnescapeDataString_returns_false_when_destination_too_small(
        string dataString,
        int tooSmallBuffer
    )
    {
        var buffer = new char[tooSmallBuffer];
        var result = UriExtensions.TryUnescapeDataString(dataString, buffer, out _);

        Assert.False(result, "Should return 'false' because the buffer is too small.");
    }

    [Theory]
    [InlineData("some%20value", 11)]
    [InlineData("some%25value", 11)]
    [InlineData("some%25value", 68)]
    [InlineData("some%25value%20with%23tokens%61valid", 29)]
    [InlineData("some%25value%20with%23tokens%61valid", 92)]
    [InlineData("The quick brown fox jumps over the lazy dog.", 45)]
    [InlineData("Bad%agencoding", 512)]
    public void TryUnescapeDataString_returns_true_when_destination_too_large(
        string dataString,
        int largeEnoughBuffer
    )
    {
        var buffer = new char[largeEnoughBuffer];
        var result = UriExtensions.TryUnescapeDataString(dataString, buffer, out _);

        Assert.True(
            result,
            "Should return 'true' because the destination buffer is larger than the required length."
        );
    }

    [Theory]
    [InlineData("some%20value", 10)]
    [InlineData("some%25value", 10)]
    [InlineData("some%25value%20with%23tokens%61valid", 28)]
    [InlineData("The quick brown fox jumps over the lazy dog.", 44)]
    [InlineData("Bad%agencoding", 14)]
    public void TryUnescapeDataString_returns_true_when_destination_exact_length(
        string dataString,
        int expected
    )
    {
        var buffer = new char[expected];
        var result = UriExtensions.TryUnescapeDataString(dataString, buffer, out _);

        Assert.True(
            result,
            "Should return 'true' because the destination buffer is the exact length required."
        );
    }

    [Theory]
    [InlineData("some%20value", 10)]
    [InlineData("some%25value", 10)]
    [InlineData("some%25value%20with%23tokens%61valid", 28)]
    [InlineData("The quick brown fox jumps over the lazy dog.", 44)]
    [InlineData("Bad%agencoding", 14)]
    public void TryUnescapeDataString_length_returns_decoded_length(string dataString, int expected)
    {
        var buffer = new char[expected];
        UriExtensions.TryUnescapeDataString(dataString, buffer, out var length);

        Assert.Equal(expected, length);
    }

    [Theory]
    [InlineData("some%20value", "some value")]
    [InlineData("some%25value", "some%value")]
    [InlineData("some%25value%20with%23tokens%61valid", "some%value with#tokensavalid")]
    [InlineData(
        "The quick brown fox jumps over the lazy dog.",
        "The quick brown fox jumps over the lazy dog."
    )]
    [InlineData("Bad%agencoding", "Bad%agencoding")]
    public void TryUnescapeDataString_decodes_data(string dataString, string expected)
    {
        var buffer = new char[expected.Length];
        UriExtensions.TryUnescapeDataString(dataString, buffer, out _);

        Assert.Equal(expected, new string(buffer));
    }
}
