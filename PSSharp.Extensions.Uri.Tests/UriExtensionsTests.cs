namespace PSSharp.Extensions.Uri;

using System;

public sealed partial class UriExtensionsTests
{
    [Theory]
    [InlineData("https://some.url/path?")]
    [InlineData("https://some.url/path/?")]
    [InlineData("https://some.url/path/")]
    [InlineData("https://some.url/path/?#fragment")]
    public void HasQueryParameters_returns_false_when_no_query(string uriString)
    {
        var uri = new Uri(uriString);

        var hasQuery = uri.HasQueryParameters();

        Assert.False(hasQuery);
    }

    [Theory]
    [InlineData("https://some.url/path?foo=bar")]
    [InlineData("https://some.url/path?foo=bar&baz=qux")]
    public void HasQueryParameters_returns_true_when_query(string uriString)
    {
        var uri = new Uri(uriString);

        var hasQuery = uri.HasQueryParameters();

        Assert.True(hasQuery);
    }

    [Theory]
    [InlineData("https://some.url/path")]
    [InlineData("https://some.url/path/")]
    public void QuerySpan_returns_empty_span_when_no_query(string uriString)
    {
        var uri = new Uri(uriString);

        var query = uri.QuerySpan();

        Assert.True(query.IsEmpty);
    }

    [Theory]
    [InlineData("https://some.url/path?foo=bar", "?foo=bar")]
    [InlineData("https://some.url/path?foo=bar&baz=qux", "?foo=bar&baz=qux")]
    [InlineData("https://some.url/path?", "?")]
    [InlineData("https://some.url/path/?", "?")]
    public void QuerySpan_returns_span_when_query(string uriString, string expected)
    {
        var uri = new Uri(uriString);

        var query = uri.QuerySpan();

        Assert.Equal(expected, query.ToString());
    }

    [Theory]
    [InlineData("https://some.url/path?foo=bar%20baz", "?foo=bar%20baz")]
    public void QuerySpan_does_not_decode_query(string uriString, string queryString)
    {
        var uri = new Uri(uriString);

        var querySpan = uri.QuerySpan();

        Assert.Equal(queryString, querySpan.ToString());
    }

    [Theory]
    [InlineData("https://some.url/path?foo=bar", "aloha")]
    [InlineData("https://some.url/path?foo=bar&baz=qux", "what")]
    [InlineData("https://some.url/path?", "does")]
    [InlineData("https://some.url/path?Floo=abra", "floo")]
    [InlineData("https://some.url/path?Has", "powder")]
    [InlineData("https://some.url/path?this=poses&ado=problem", "do")]
    public void TryGetQueryParameterSpan_returns_false_when_not_present(
        string uriString,
        string parameterName
    )
    {
        var uri = new Uri(uriString);

        var success = uri.TryGetQueryParameterSpan(parameterName, out _);

        Assert.False(success, "Got 'true' but query parameter is not present");
    }

    [Theory]
    [InlineData("https://some.url/path?foo=bar", "foo")]
    [InlineData("https://some.url/path?foo=bar&baz=qux", "foo")]
    [InlineData("https://some.url/path?foo=bar&baz=qux", "baz")]
    public void TryGetQueryParameterSpan_returns_true_when_present(
        string uriString,
        string parameterName
    )
    {
        var uri = new Uri(uriString);

        var success = uri.TryGetQueryParameterSpan(parameterName, out _);

        Assert.True(success, "Got 'false' but query parameter is present");
    }

    [Theory]
    [InlineData("https://some.url/path?foo=bar", "foo", "bar")]
    [InlineData("https://some.url/path?foo=bar&baz=qux", "foo", "bar")]
    [InlineData("https://some.url/path?foo=bar&baz=qux", "baz", "qux")]
    public void TryGetQueryParameterSpan_value_expected_when_present(
        string uriString,
        string parameterName,
        string expectedValue
    )
    {
        var uri = new Uri(uriString);

        var success = uri.TryGetQueryParameterSpan(parameterName, out var value);

        Assert.Equal(expectedValue, value.ToString());
    }

    [Theory]
    [InlineData("https://some.url/path?foo", "foo")]
    [InlineData("https://some.url/path?foo&baz", "baz")]
    public void TryGetQueryParameterSpan_true_with_empty_string_when_no_value(
        string uriString,
        string parameterName
    )
    {
        var uri = new Uri(uriString);

        var success = uri.TryGetQueryParameterSpan(parameterName, out var value);

        Assert.True(success);
        Assert.True(value.IsEmpty);
    }

    [Theory]
    [InlineData("https://some.url/path?foo%20bar=baz", "foo bar")]
    public void TryGetQueryParameterSpan_finds_encoded_parameter(
        string uriString,
        string parameterName
    )
    {
        var uri = new Uri(uriString);

        var success = uri.TryGetQueryParameterSpan(parameterName, out _);

        Assert.True(success, "The query should be checked for the encoded parameter.");
    }

    [Theory]
    [InlineData("https://some.url/path?foo=bar%20baz", "foo", "bar%20baz")]
    public void TryGetQueryParameterSpan_does_not_decode_query(
        string uriString,
        string parameterName,
        string expectedValue
    )
    {
        var uri = new Uri(uriString);

        uri.TryGetQueryParameterSpan(parameterName, out var value);

        Assert.Equal(expectedValue, value.ToString());
    }
}
