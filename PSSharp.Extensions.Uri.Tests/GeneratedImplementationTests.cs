namespace PSSharp.Extensions.Uri;

public sealed partial record GeneratedImplementation : IQueryStringModel
{
    [QueryStringCondition(nameof(StringCondition))]
    public string StringWithCondition { get; init; } = string.Empty;
    public string? StringWithoutCondition { get; init; }

    public const string IntParameterName = "ax-o-lotol";

    [QueryStringParameter(IntParameterName, true)]
    public int Int { get; init; }

    [QueryStringCondition(QueryStringCondition.Always)]
    public int AlwaysInt { get; init; }

    [QueryStringSerializer(nameof(CustomSerializer))]
    public double CustomSerialized { get; init; }

    private void CustomSerializer(QueryStringBuilder builder)
    {
        builder.AddEscaped("custom_serialized", $"%20_{CustomSerialized}_%20");
    }

    private bool StringCondition() => !string.IsNullOrWhiteSpace(StringWithCondition);

    // The superior way to instruct a source generator to write code.
    // Use an attribute? Pedestrian.
    // Use a partial method? Now we're talking.
    public partial void AddToQueryString(QueryStringBuilder builder);
}

public class GeneratedImplementationTests
{
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
        query.AssertDoesNotContainKey(nameof(GeneratedImplementation.StringWithCondition));
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
        query.AssertContainsValue(nameof(GeneratedImplementation.StringWithCondition), value);
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
        query.AssertDoesNotContainKey(nameof(GeneratedImplementation.StringWithoutCondition));
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
        query.AssertContainsValue(nameof(GeneratedImplementation.StringWithoutCondition), value);
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
        query.AssertDoesNotContainKey(GeneratedImplementation.IntParameterName);
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
        query.AssertContainsValue(GeneratedImplementation.IntParameterName, value.ToString());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public void QueryStringParameterAttribute_sets_parameter_name_when_serialized(int value)
    {
        // Arrange
        var model = new GeneratedImplementation() { Int = value };
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        query.AssertContainsValue(GeneratedImplementation.IntParameterName, value.ToString());
        query.AssertDoesNotContainKey(nameof(GeneratedImplementation.Int));
    }

    [Fact]
    public void QueryStringCondition_Always_always_serializes()
    {
        // Arrange
        var model = new GeneratedImplementation() { AlwaysInt = default };
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        query.AssertContainsValue(
            nameof(GeneratedImplementation.AlwaysInt),
            default(int).ToString()
        );
    }

    [Theory]
    [InlineData("Test with spaces")]
    public void String_serializes_and_deserializes_with_spaces(string value)
    {
        // Arrange
        var model = new GeneratedImplementation() { StringWithoutCondition = value };
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        query.AssertContainsValue(nameof(GeneratedImplementation.StringWithoutCondition), value);
    }

    [Theory]
    [InlineData(11.0)]
    public void QueryStringSerializer_uses_custom_serializer(double value)
    {
        // Arrange
        var model = new GeneratedImplementation() { CustomSerialized = value };
        var query = new QueryStringBuilder();

        // Act
        model.AddToQueryString(query);

        // Assert
        query.AssertContainsValue("custom_serialized", $" _{value}_ ");
        // (also the prop should not be serialized)
        query.AssertDoesNotContainKey(nameof(GeneratedImplementation.CustomSerialized));
    }
}
