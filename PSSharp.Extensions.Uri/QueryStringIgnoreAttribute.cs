namespace PSSharp.Extensions.Uri;

using System;

/// <summary>
/// Instructs the <see cref="IQueryStringModel"/> source generator to ignore the member when
/// de/serializing a query string from the model of the type this attribute is applied to.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = true,
    AllowMultiple = false
)]
public sealed class QueryStringIgnoreAttribute : Attribute;
