namespace PSSharp.Extensions.Uri;

public sealed class QueryStringBuilderTests
{
    [Theory]
    [InlineData("key", "value", "?key=value")]
    [InlineData("key", "value with spaces", "?key=value%20with%20spaces")]
    [InlineData("key", 1234.5678, "?key=1234.5678")]
    public void Add_key_value_equals_expected(string key, dynamic value, string expected)
    {
        // arrange
        var builder = new QueryStringBuilder();

        // act
        builder.Add(key, value);
        var actual = builder.ToString();

        // assert
        Assert.Equal(expected, actual);
    }

    public static TheoryData<Action<QueryStringBuilder>[]> Adds()
    {
        var data = new TheoryData<Action<QueryStringBuilder>[]>();
        data.Add([builder => builder.Add("key", "value")]);
        data.Add([builder => builder.Add("1, 2"), builder => builder.Add("3", 4)]);
        data.Add(
            [
                builder => builder.Add("key", "value"),
                builder => builder.Add("key2", 11),
                builder => builder.Add("key3")
            ]
        );
        data.Add([]);
        data.Add([builder => builder.Add("key", "value"), builder => builder.Add("key", "value")]);

        return data;
    }

    [Theory]
    [MemberData(nameof(Adds))]
    public void Count_matches_num_of_add_calls(Action<QueryStringBuilder>[] adds)
    {
        var builder = new QueryStringBuilder();

        foreach (var add in adds)
        {
            add(builder);
        }

        Assert.Equal(adds.Length, builder.Count());
    }

    [Theory]
    [InlineData("not escaped", "escaped")]
    [InlineData("badly%2Zescaped", "escaped")]
    public void AddEscaped_throws_when_parameter_not_escaped(string parameter, string value)
    {
        var builder = new QueryStringBuilder();
        Assert.Throws<ArgumentException>("parameter", () => builder.AddEscaped(parameter, value));
    }

    [Theory]
    [InlineData("escaped", "not escaped")]
    [InlineData("escaped", "badly%2Zescaped")]
    public void AddEscaped_throws_when_value_not_escaped(string parameter, string value)
    {
        var builder = new QueryStringBuilder();
        Assert.Throws<ArgumentException>("value", () => builder.AddEscaped(parameter, value));
    }

    [Theory]
    [InlineData("not escaped", "escaped")]
    public void Add_escapes_parameter(string parameter, string value)
    {
        var builder = new QueryStringBuilder { { parameter, value } };

        var escaped = System.Uri.EscapeDataString(parameter);

        Assert.Contains(escaped, builder.ToString());
    }

    [Theory]
    [InlineData("escaped", "not escaped")]
    public void Add_escapes_value(string parameter, string value)
    {
        var builder = new QueryStringBuilder { { parameter, value } };

        var escaped = System.Uri.EscapeDataString(value);

        Assert.Contains(escaped, builder.ToString());
    }

    [Theory]
    [InlineData("escaped", "escaped")]
    public void Add_does_not_escape_already_escaped(string parameter, string value)
    {
        var builder = new QueryStringBuilder { { parameter, value } };

        Assert.Contains(parameter, builder.ToString());
        Assert.Contains(value, builder.ToString());
    }

    public static TheoryData<QueryStringBuilder> GetBuildersWithKey()
    {
        return new TheoryData<QueryStringBuilder>()
        {
            new QueryStringBuilder { { "key", "value" } },
            new QueryStringBuilder { { "key", "value" }, { "key2", 1234 } },
            new QueryStringBuilder { { "key", "value" }, { "key", "value" } },
            new QueryStringBuilder { { "key", 11 }, { "foo", true } },
            new QueryStringBuilder { { "key" } },
        };
    }

    public static TheoryData<QueryStringBuilder> GetBuildersWithoutKey()
    {
        return new TheoryData<QueryStringBuilder>()
        {
            new QueryStringBuilder(),
            new QueryStringBuilder { { "key1", "value" } },
            new QueryStringBuilder { { "akey", "value" }, { "key2", 1234 } },
            new QueryStringBuilder
            {
                { "akeya", "value" },
                { "&key", "value" },
                { "?key", "value" }
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetBuildersWithKey))]
    public void ContainsKey_returns_true_when_key_exists(QueryStringBuilder builder)
    {
        var actual = builder.ContainsKey("key");

        Assert.True(actual, "Builder does contain the expected key.");
    }

    [Theory]
    [MemberData(nameof(GetBuildersWithoutKey))]
    public void ContainsKey_returns_false_when_key_does_not_exist(QueryStringBuilder builder)
    {
        var actual = builder.ContainsKey("key");

        Assert.False(actual, "Builder does not contain the key.");
    }

    [Theory]
    [MemberData(nameof(GetBuildersWithKey))]
    public void TryGetFirstValue_returns_true_when_key_exists(QueryStringBuilder builder)
    {
        var actual = builder.TryGetFirstValue("key", out _);

        Assert.True(actual, "Builder does contain the expected key.");
    }

    [Theory]
    [MemberData(nameof(GetBuildersWithoutKey))]
    public void TryGetFirstValue_returns_false_when_key_does_not_exist(QueryStringBuilder builder)
    {
        var actual = builder.TryGetFirstValue("key", out _);

        Assert.False(actual, "Builder does not contain the key.");
    }

    [Theory]
    [MemberData(nameof(GetBuildersWithKey))]
    public void TryGetMultipleValues_returns_true_when_key_exists(QueryStringBuilder builder)
    {
        var actual = builder.TryGetMultipleValues("key", out _);

        Assert.True(actual, "Builder does contain the expected key.");
    }

    [Theory]
    [MemberData(nameof(GetBuildersWithoutKey))]
    public void TryGetMultipleValues_returns_false_when_key_does_not_exist(
        QueryStringBuilder builder
    )
    {
        var actual = builder.TryGetMultipleValues("key", out _);

        Assert.False(actual, "Builder does not contain the key.");
    }
}
