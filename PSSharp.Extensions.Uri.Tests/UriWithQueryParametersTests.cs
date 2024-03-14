namespace PSSharp.Extensions.Uri;

using System;

public sealed class UriWithQueryParametersTests
{
    public static TheoryData<Uri, Dictionary<string, string?>, Uri> AddQueryData()
    {
        return new TheoryData<Uri, Dictionary<string, string?>, Uri>()
        {
            {
                new Uri("https://example.com"),
                new Dictionary<string, string?> { { "a", "b" } },
                new Uri("https://example.com/?a=b")
            },
            {
                new Uri("https://example.com"),
                new Dictionary<string, string?> { { "a", "b" }, { "c", "d" } },
                new Uri("https://example.com/?a=b&c=d")
            },
            {
                new Uri("https://example.com"),
                new Dictionary<string, string?> { { "a", "b" }, { "c", null } },
                new Uri("https://example.com/?a=b")
            },
            {
                new Uri("https://example.com"),
                new Dictionary<string, string?>
                {
                    { "a", "b" },
                    { "c", "d" },
                    { "e", "f" }
                },
                new Uri("https://example.com/?a=b&c=d&e=f")
            },
        };
    }

    [Theory]
    [MemberData(nameof(AddQueryData))]
    public void WithQueryParameters_adds_query_param_when_not_present(
        Uri uri,
        Dictionary<string, string?> query,
        Uri expected
    )
    {
        var result = uri.WithQueryParameters(query);

        Assert.Equal(expected, result);
    }

    public static TheoryData<Uri, Dictionary<string, string?>, Uri> OverwriteQueryData()
    {
        return new TheoryData<Uri, Dictionary<string, string?>, Uri>()
        {
            {
                new Uri("https://example.com/?a=b"),
                new Dictionary<string, string?>() { { "a", "b" } },
                new Uri("https://example.com/?a=b")
            },
            {
                new Uri("https://example.com/?a=b"),
                new Dictionary<string, string?>() { { "a", "c" } },
                new Uri("https://example.com/?a=c")
            },
            {
                new Uri("https://example.com/?a=b&c=d&e=f"),
                new Dictionary<string, string?>() { { "c", "g" } },
                new Uri("https://example.com/?a=b&e=f&c=g")
            },
            {
                new Uri("https://example.com/?a=b&c=d&e=f"),
                new Dictionary<string, string?>()
                {
                    { "a", "1" },
                    { "c", "3" },
                    { "e", "5" }
                },
                new Uri("https://example.com/?a=1&c=3&e=5")
            },
        };
    }

    [Theory]
    [MemberData(nameof(OverwriteQueryData))]
    public void WithQueryParameters_updates_existing_query_parameters(
        Uri originalUri,
        Dictionary<string, string?> parameters,
        Uri expected
    )
    {
        var actual = originalUri.WithQueryParameters(parameters);

        Assert.Equal(actual, expected);
    }
}
