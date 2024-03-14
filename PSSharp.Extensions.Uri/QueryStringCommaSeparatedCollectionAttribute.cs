namespace PSSharp.Extensions.Uri;

using System;

/// <summary>
/// Instructs the <see cref="IQueryStringModel"/> source generator to serialize and deserialize
/// the collection member as a comma separated list instead of using the multiple entry format
/// for query string parameters.
/// <para>
/// For example, when present <c>string[] Numbers = { 1, 2, 3 };</c> will serialize
/// as <c>?Numbers=1,2,3</c>. When not present, the default serialization is
/// <c>?Numbers=1&amp;Numbers=2&amp;Numbers=3</c>.
/// </para>
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = true,
    AllowMultiple = false
)]
public sealed class QueryStringCommaSeparatedCollectionAttribute : Attribute;
