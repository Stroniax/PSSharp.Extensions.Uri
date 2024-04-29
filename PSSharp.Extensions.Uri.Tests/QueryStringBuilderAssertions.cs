namespace PSSharp.Extensions.Uri;

public static class QueryStringBuilderAssertions
{
    public static void AssertContainsValue(
        this QueryStringBuilder builder,
        string key,
        string value
    )
    {
        Assert.Contains(
            builder,
            kvp => string.Equals(key, kvp.Key) && string.Equals(value, kvp.Value)
        );
    }

    public static void AssertContainsKey(this QueryStringBuilder builder, string key)
    {
        var contains = builder.ContainsKey(key);
        Assert.True(contains, $"Expected key '{key}' to be present in the QueryStringBuilder.");
    }

    public static void AssertDoesNotContainKey(this QueryStringBuilder builder, string key)
    {
        var contains = builder.ContainsKey(key);
        Assert.False(
            contains,
            $"Expected key '{key}' to not be present in the QueryStringBuilder."
        );
    }
}
