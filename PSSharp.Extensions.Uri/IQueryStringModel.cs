namespace PSSharp.Extensions.Uri;

using System.Text;

/// <summary>
/// Defines a type that is a representation of a query string.
/// <para>
/// Declare any members of this type as <c>partial</c> to instruct the implementation to be source-generated.
/// </para>
/// </summary>
public interface IQueryStringModel
{
    /// <summary>
    /// Appends the query string representation of the current instance to a <see cref="StringBuilder"/>
    /// that represents a URI.
    /// </summary>
    /// <param name="query">A builder to which members of the query string represented by the current
    /// instance will be appended.</param>
    /// <param name="hasQueryParams">A reference that determines if ? or &amp; is appended to the query
    /// string.</param>
    void AppendQueryString(StringBuilder query, ref bool hasQueryParams);
}
