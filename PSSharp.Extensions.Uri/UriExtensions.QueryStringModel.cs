namespace PSSharp.Extensions.Uri;

using System;
using System.Text;

public static partial class UriExtensions
{
    /// <summary>
    /// Appends the query string representation of <paramref name="model"/> to a
    /// <see cref="StringBuilder"/> that represents a URI.
    /// </summary>
    /// <typeparam name="T">A query string model.</typeparam>
    /// <param name="query">An instance that is being used to form a URI.</param>
    /// <param name="model">A query string model. If <see langword="null"/>, the <paramref name="model"/>
    /// is returned unchanged.</param>
    /// <param name="hasQueryParams">A reference used to determine if ? or &amp; must be used to prefix
    /// query parameters. <see langword="false"/> if the first query string parameter must be prefixed
    /// with <c>?</c>.</param>
    /// <returns></returns>
    public static StringBuilder AppendQueryString<T>(
        this StringBuilder query,
        in T? model,
        ref bool hasQueryParams
    )
        where T : IQueryStringModel
    {
        ArgumentNullException.ThrowIfNull(query);

        if (model is null)
        {
            return query;
        }

        model.AppendQueryString(query, ref hasQueryParams);

        return query;
    }

    public static string ToQueryString<T>(this T queryModel)
        where T : notnull, IQueryStringModel
    {
        ArgumentNullException.ThrowIfNull(queryModel);

        var query = new StringBuilder();
        var hasQueryParams = false;
        queryModel.AppendQueryString(query, ref hasQueryParams);
        return query.ToString();
    }
}
