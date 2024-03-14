namespace PSSharp.Extensions.Uri;

using System;

/// <summary>
/// Instructs the <see cref="IQueryStringModel"/> source generator to use the specified name
/// when serializing and deserializing the member as a query string parameter.
/// </summary>
/// <param name="name">The name to de/serialize the member to or from in the query string.
/// For example, <c>[QueryStringPropertyName("q-foo")]public string Foo = "bar";</c>
/// serializes as <c>?q-foo=bar</c>. By default the query string parameter will have the
/// same name as the member.</param>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = true,
    AllowMultiple = false
)]
public sealed class QueryStringParameterAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
