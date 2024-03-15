namespace PSSharp.Extensions.Uri;

using System;

#pragma warning disable CS9113 // Parameter is unread.

/// <summary>
/// Instructs the <see cref="IQueryStringModel"/> source generator to use the specified name
/// when serializing and deserializing the member as a query string parameter.
/// By default the query string parameter will have the same name as the member.
/// <code>
/// <c>
/// // serializes as ?q-foo=bar
/// [QueryStringPropertyName("q-foo")]
/// public string Foo { get; set; } = "bar";
/// </c>
/// </code>
/// </summary>
/// <param name="name">
/// The name to de/serialize the member to or from in the query string.
/// </param>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = true,
    AllowMultiple = false
)]
public sealed class QueryStringParameterAttribute(string name) : Attribute;
