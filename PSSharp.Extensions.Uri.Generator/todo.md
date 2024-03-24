Might be worth it to add

```C#
/// <summary>
/// Specifies a method to aggregate multiple query string parameters of the same name into a single value.
/// <code><c>
/// [QueryStringDeserializerAggregator("CreateAggregator", "Build")]
/// public int[] PropVal { get; init; }
/// private static List<int> CreateAggregator(int first) => new List<int>() { first };
/// private static int[] Build(List<int> aggregate) => aggregate.ToArray();
/// </c></code>
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class QueryStringDeserializerAggregatorAttribute(
	string createAggregator,
	string build
) : Attribute;
```

```C#
[QueryStringDeserializerFactory("Create")]
public class NoUsableInit {
  [QueryStringParameter("prop")]]
  public int Prop { get; }

  [QueryStringParameter("foo")]
  public int Foo { get; }

  [QueryStringParameter("bar")]
  public int Bar { get; init; }

  public NoUsableCtor(int prop, int foo) {
	Prop = prop;
	Foo = foo;
  }

  private static NoUsableCtor Create(int prop, int foo, int bar) => new(prop, foo) { Bar = bar };
}
```