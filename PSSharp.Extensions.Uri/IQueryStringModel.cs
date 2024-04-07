namespace PSSharp.Extensions.Uri;

using System;

/// <summary>
/// Defines a type that is a representation of a query string.
/// <para>
/// Declare any members of this implementation as <c>partial</c> to instruct the implementation to be
/// source-generated. <see cref="IParsable{TSelf}"/> and <see cref="ISpanParsable{TSelf}"/>
/// implementations may also be declared as <c>partial</c> to prompt source-generated implementations.
/// </para>
/// </summary>
public interface IQueryStringModel
{
    /// <summary>
    /// Adds the properties of this instance to the provided <see cref="QueryStringBuilder"/>.
    /// </summary>
    /// <param name="builder">
    /// A <see cref="QueryStringBuilder"/> to which all members of the current
    /// instance will be serialized.
    /// </param>
    void AddToQueryString(QueryStringBuilder builder);

    /// <summary>
    /// Forms a query string from the properties of this instance. This method has a default implementation
    /// which uses the <see cref="AddToQueryString(QueryStringBuilder)"/> method to build the query string,
    /// but allows for a more optimized implementation if necessary.
    /// </summary>
    /// <returns>A well-formed, escaped query string which the current instance represents.</returns>
    string ToQueryString()
    {
        var builder = new QueryStringBuilder();
        AddToQueryString(builder);
        return builder.ToString();
    }
}
