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
}
