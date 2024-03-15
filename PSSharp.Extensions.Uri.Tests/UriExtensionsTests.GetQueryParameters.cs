namespace PSSharp.Extensions.Uri;

using System;

public sealed partial class UriExtensionsTests
{
    [Theory]
    [InlineData("https://some.url/path?")]
    [InlineData("https://some.url/path/?")]
    [InlineData("https://some.url/path/")]
    [InlineData("https://some.url/path")]
    public void GetQueryParameters_returns_empty_dictionary_when_no_query(string uriString)
    {
        var uri = new Uri(uriString);

        var query = uri.GetQueryParameters();

        Assert.Empty(query);
    }

    public static TheoryData<Uri> NonAbsoluteUri()
    {
        return
        [
            new("/path?foo=bar", UriKind.Relative),
            new("path?foo=bar", UriKind.RelativeOrAbsolute),
            new("path", UriKind.Relative),
            new("/path", UriKind.Relative),
            new("?foo=bar", UriKind.Relative),
        ];
    }

    [Theory]
    [MemberData(nameof(NonAbsoluteUri))]
    public void GetQueryParameters_throws_InvalidOperationException_when_not_absolute(Uri uri)
    {
        Assert.Throws<InvalidOperationException>(() => uri.GetQueryParameters());
    }

    [Fact]
    public void GetQueryParameters_throws_ArgumentNullException_when_null()
    {
        Uri? uri = null;

        Assert.Throws<ArgumentNullException>(() => uri!.GetQueryParameters());
    }

    public static TheoryData<Uri, Dictionary<string, string>> UriWithExpectedQuery()
    {
        return new TheoryData<Uri, Dictionary<string, string>>()
        {
            {
                new Uri("https://some.url/path?foo=bar"),
                new Dictionary<string, string> { { "foo", "bar" } }
            },
            {
                new Uri("https://some.url/path?foo=bar&baz=qux"),
                new Dictionary<string, string> { { "foo", "bar" }, { "baz", "qux" } }
            },
        };
    }

    [Theory]
    [MemberData(nameof(UriWithExpectedQuery))]
    public void GetQueryParameters_returns_expected_query(
        Uri uri,
        Dictionary<string, string> expectedQuery
    )
    {
        var query = uri.GetQueryParameters();

        Assert.Collection(
            query,
            expectedQuery
                .Select(expectedKeyValue => new Action<KeyValuePair<string, string>>(
                    actualKeyValue => Assert.Equal(expectedKeyValue, actualKeyValue)
                ))
                .ToArray()
        );
    }

    [Theory]
    [InlineData("https://some.uri/?queryWithNoValue")]
    [InlineData("https://some.uri/?queryWithNoValue=")]
    [InlineData("https://some.uri/?other=hasvalue&queryWithNoValue&final=1")]
    [InlineData("https://some.uri/?other=hasvalue&queryWithNoValue=&final=1")]
    public void GetQueryParamters_returns_empty_string_for_query_with_no_value(string uriString)
    {
        var uri = new Uri(uriString);

        var query = uri.GetQueryParameters();

        Assert.True(query.TryGetValue("queryWithNoValue", out var value));
        Assert.Same(string.Empty, value);
    }

    [Theory]
    [InlineData("https://some.uri/?queryWithNoValue&queryWithNoValue")]
    public void GetQueryParameters_throws_when_multiple_entries(string uriString)
    {
        var uri = new Uri(uriString);

        Assert.Throws<InvalidOperationException>(() => uri.GetQueryParameters());
    }

    [Theory]
    [InlineData("https://some.uri/?var=some%20value", "var", "some value")]
    [InlineData("https://some.uri/?a=b&c&var=some%20value", "var", "some value")]
    public void GetQueryParameters_decodes_values(
        string uriString,
        string key,
        string expectedValue
    )
    {
        var uri = new Uri(uriString);

        var query = uri.GetQueryParameters();

        Assert.True(query.TryGetValue(key, out var value));
        Assert.Equal(expectedValue, value);
    }
}
