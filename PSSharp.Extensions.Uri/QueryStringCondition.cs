namespace PSSharp.Extensions.Uri;

/// <summary>
/// Options to configure when a member of a source-generated <see cref="IQueryStringModel"/> implementation
/// will be included in a query string during serialization.
/// </summary>
public enum QueryStringCondition
{
    /// <summary>
    /// The property must only be serialized when its value is not the <see langword="default"/>
    /// value for the property's type.
    /// </summary>
    WhenNotDefault,

    /// <summary>
    /// The property must always be serialized.
    /// </summary>
    Always,

    /// <summary>
    /// The property must never be serialized. Use this value to indicate that a member is not
    /// a query string parameter.
    /// </summary>
    Never,
}
