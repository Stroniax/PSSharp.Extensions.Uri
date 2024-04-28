namespace PSSharp.Extensions.Uri;

public sealed partial record GeneratedImplementation : IQueryStringModel
{
    [QueryStringCondition(nameof(StringCondition))]
    public string StringWithCondition { get; init; } = string.Empty;
    public string? StringWithoutCondition { get; init; }

    public const string IntParameterName = "ax-o-lotol";

    [QueryStringParameter(IntParameterName, true)]
    public int Int { get; init; }

    private bool StringCondition() => !string.IsNullOrWhiteSpace(StringWithCondition);

    public partial void AddToQueryString(QueryStringBuilder builder);
}

public class GeneratedImplementationTests
{
    /// <summary>
    /// These tests rely on the 'default' GeneratedImplementation instance to not serialize any properties.
    /// </summary>
    [Fact]
    public void Baseline_new_remains_empty()
    {
        // Arrange
        var model = new GeneratedImplementation();
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        Assert.Empty(query);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Condition_when_false_does_not_serialize(string foo)
    {
        // Arrange
        var model = new GeneratedImplementation { StringWithCondition = foo };
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        Assert.DoesNotContain(
            query,
            kvp =>
                string.Equals(
                    nameof(GeneratedImplementation.StringWithCondition),
                    kvp.Key,
                    StringComparison.OrdinalIgnoreCase
                )
        );
    }

    [Theory]
    [InlineData("foo")]
    public void Condition_when_true_does_serialize(string value)
    {
        // Arrange
        var model = new GeneratedImplementation { StringWithCondition = value };
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        Assert.Contains(
            query,
            kvp =>
                string.Equals(nameof(GeneratedImplementation.StringWithCondition), kvp.Key)
                && string.Equals(value, kvp.Value)
        );
    }

    [Fact]
    public void String_with_no_condition_does_not_serialize_when_null()
    {
        // Arrange
        var model = new GeneratedImplementation { StringWithoutCondition = default };
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        var contains = query.ContainsKey(nameof(GeneratedImplementation.StringWithoutCondition));
        Assert.False(contains, "Bar should not be serialized.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Test")]
    public void String_with_no_condition_serializes_when_not_null(string value)
    {
        // Arrange
        var model = new GeneratedImplementation { StringWithoutCondition = value };
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        Assert.Contains(
            query,
            kvp =>
                string.Equals(nameof(GeneratedImplementation.StringWithoutCondition), kvp.Key)
                && string.Equals(value, kvp.Value)
        );
    }

    [Fact]
    public void Int_with_no_condition_does_not_serialize_if_default()
    {
        // Arrange
        var model = new GeneratedImplementation() { Int = default };
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        var contains = query.ContainsKey(GeneratedImplementation.IntParameterName);
        Assert.False(contains, "Qux should not be serialized.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public void Int_with_no_condition_serializes_if_not_default(int value)
    {
        // Arrange
        var model = new GeneratedImplementation() { Int = value };
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        Assert.Contains(
            query,
            kvp =>
                string.Equals(GeneratedImplementation.IntParameterName, kvp.Key)
                && string.Equals(value.ToString(), kvp.Value)
        );
    }
}
