namespace PSSharp.Extensions.Uri;

using System;

#pragma warning disable CS9113 // Parameter is unread.

/// <summary>
/// Identifies a custom serialization method that the <see cref="IQueryStringModel"/> source generator will
/// call to serialize a value of a query string parameter. The method must match the following signature,
/// though the name will vary and is identified by <paramref name="serializer"/>:
/// <code>
/// <c>
/// private void Serialize(
///     QueryStringBuilder builder,
///     string memberName,          // optional
///     TMember memberValue
/// );
/// </c>
/// </code>
/// </summary>
/// <param name="serializer">The name of a method used to serialize the member.</param>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = true,
    AllowMultiple = false
)]
public sealed class QueryStringSerializerAttribute(string serializer) : Attribute;
