namespace PSSharp.Extensions.Uri;

using System;

#pragma warning disable CS9113 // Parameter is unread.

/// <summary>
/// Identifies a custom deserialization method that the <see cref="IQueryStringModel"/> source generator will
/// call to deserialize a value of a query string parameter. The method must match the following signature:
/// <code>
/// <c>internal static bool TryDeserialize(
///     string memberName,
///     ReadOnlySpan&lt;char&gt; uriEncodedParameterName,
///     ReadOnlySpan&lt;char&gt; uriEncodedParameterValue,
///     out T result
/// );</c>
/// </code>
/// </summary>
/// <param name="deserializer">The name of a method used to deserialize the member.</param>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = true,
    AllowMultiple = false
)]
public sealed class QueryStringDeserializerAttribute(string deserializer) : Attribute;
