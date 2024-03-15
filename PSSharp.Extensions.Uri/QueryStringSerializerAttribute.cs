namespace PSSharp.Extensions.Uri;

using System;

#pragma warning disable CS9113 // Parameter is unread.

/// <summary>
/// Identifies a custom serialization method that the <see cref="IQueryStringModel"/> source generator will
/// call to serialize a value of a query string parameter. The method must match the following signature:
/// <code>
/// <c>
/// internal static string Serialize(             | internal static void Serialize(
///     TThis queryModel,                         |     TThis queryModel,
///     string memberName,                        |     string memberName,
///     TMember memberValue,                      |     TMember memberValue,
///     out bool isUriEncoded                     |     Span&lt;char&gt; uriEncodedBuffer,
/// );                                            |     ref int charsWritten,
///                                               |     out bool hasMoreData
///                                               | );
/// </c>
/// </code>
/// </summary>
/// <param name="deserializer">The name of a method used to deserialize the member.</param>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = true,
    AllowMultiple = false
)]
public sealed class QueryStringSerializerAttribute(string serializer) : Attribute;
