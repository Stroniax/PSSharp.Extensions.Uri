namespace PSSharp.Extensions.Uri;

using System;

public class UriWithQueryParameterTests
{
    [Theory]
    [InlineData("http://example.com", "test", "value", "http://example.com/?test=value")]
    [InlineData(
        "http://example.com?foo=bar",
        "test",
        "value",
        "http://example.com/?foo=bar&test=value"
    )]
    public void WithQueryParameter_adds_missing_parameter(
        string originalUri,
        string parameter,
        string value,
        string expected
    )
    {
        var uri = new Uri(originalUri);

        var result = uri.WithQueryParameter(parameter, value);

        Assert.Equal(expected, result.ToString());
    }

    [Theory]
    [InlineData("http://example.com?test=value", "test", "foo", "http://example.com/?test=foo")]
    public void WithQueryParameter_updates_existing_parameter(
        string originalUri,
        string parameter,
        string value,
        string expected
    )
    {
        var uri = new Uri(originalUri);

        var result = uri.WithQueryParameter(parameter, value);

        Assert.Equal(expected, result.ToString());
    }

    [Theory]
    [InlineData("http://example.com?test=value", "test", "http://example.com/")]
    public void WithQueryParameter_removes_parameter(
        string originalUri,
        string parameter,
        string expected
    )
    {
        var uri = new Uri(originalUri);

        var result = uri.WithQueryParameter(parameter, null);

        Assert.Equal(expected, result.ToString());
    }

    [Theory]
    [InlineData("foo", "bar")]
    public void WithQueryParameter_throws_on_null_uri(string key, string? value)
    {
        Uri? uri = null;

        Assert.Throws<ArgumentNullException>(
            nameof(uri),
            () => uri!.WithQueryParameter(key, value)
        );
    }

    [Theory]
    [InlineData("https://www.example.com/", "bar")]
    [InlineData("https://www.example.com/?foo=bar", "bar")]
    public void WithQueryParameter_throws_on_null_parameter(string originalUri, string? value)
    {
        var uri = new Uri(originalUri);

        Assert.Throws<ArgumentNullException>(
            "parameter",
            () => uri.WithQueryParameter(null!, value)
        );
    }

    [Theory]
    [InlineData(
        "https://www.example.com",
        "foo",
        "bar baz",
        "https://www.example.com/?foo=bar%20baz"
    )]
    [InlineData(
        "https://www.example.com",
        "foo bar",
        "baz",
        "https://www.example.com/?foo%20bar=baz"
    )]
    public void WithQueryParameter_escapes_data(
        string originalUri,
        string parameter,
        string value,
        string expectedUri
    )
    {
        var uri = new Uri(originalUri);

        var result = uri.WithQueryParameter(parameter, value);

        Assert.Equal(expectedUri, result.AbsoluteUri);
    }
}
