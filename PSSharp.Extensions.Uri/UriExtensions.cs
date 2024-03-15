namespace PSSharp.Extensions.Uri;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

/// <summary>
/// Defines extension methods for <see cref="Uri"/> and related types.
/// </summary>
public static class UriExtensions
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

    /// <summary>
    /// Creates a URI based on the current instance with query string parameter set to the specified value.
    /// If <paramref name="value"/> matches the current value of the query string parameter, the same
    /// <see cref="Uri"/> instance will be returned.
    /// </summary>
    /// <param name="uri">The URI to base the new URI on.</param>
    /// <param name="parameter">The name of the query string parameter.</param>
    /// <param name="value">The value of the query string parameter. <see langword="null"/> to remove the
    /// parameter if it is present.</param>
    /// <returns></returns>
    public static Uri WithQueryParameter(
        [NotNull] this Uri uri,
        [NotNull] string parameter,
        string? value
    )
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(parameter);

        var escapedParameter = Uri.EscapeDataString(parameter);

        var remainingQuery = uri.Query.AsSpan();
        if (remainingQuery is ['?', ..])
        {
            remainingQuery = remainingQuery[1..];
        }

        if (!remainingQuery.Contains(escapedParameter, StringComparison.Ordinal) && value is null)
        {
            return uri;
        }

        var escapedValue = value is null ? null : Uri.EscapeDataString(value);

        var query = new StringBuilder();

        while (!remainingQuery.IsEmpty)
        {
            var endOfParameterPair = remainingQuery.IndexOfAny('&', '#');
            ReadOnlySpan<char> currentParameterPair;
            if (endOfParameterPair == -1)
            {
                currentParameterPair = remainingQuery;
                remainingQuery = [];
            }
            else
            {
                currentParameterPair = remainingQuery[..endOfParameterPair];
                remainingQuery = remainingQuery[(endOfParameterPair + 1)..];
            }

            ReadOnlySpan<char> rawCurrentParameterName;
            ReadOnlySpan<char> rawCurrentParameterValue;
            var endOfParameterName = currentParameterPair.IndexOf('=');
            if (endOfParameterName == -1)
            {
                rawCurrentParameterName = currentParameterPair;
                rawCurrentParameterValue = [];
            }
            else
            {
                rawCurrentParameterName = currentParameterPair[..endOfParameterName];
                rawCurrentParameterValue = currentParameterPair[(endOfParameterName + 1)..];
            }

            if (rawCurrentParameterName.Equals(escapedParameter, StringComparison.Ordinal))
            {
                // will be added at the end
                continue;
            }

            Append(query, rawCurrentParameterName, rawCurrentParameterValue);
        }

        if (escapedValue is not null)
        {
            Append(query, escapedParameter, escapedValue);
        }

        var builder = new UriBuilder(uri);
        builder.Query = query.ToString();
        return builder.Uri;

        static void Append(StringBuilder query, ReadOnlySpan<char> key, ReadOnlySpan<char> value)
        {
            if (query.Length == 0)
            {
                query.Append('?');
            }
            else
            {
                query.Append('&');
            }

            query.Append(key).Append('=').Append(value);
        }
    }

    /// <summary>
    /// Creates a URI based on the current instance with query string parameters set to the values
    /// specified in <paramref name="parameters"/>.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Uri WithQueryParameters(
        [NotNull] this Uri uri,
        [NotNull] IReadOnlyDictionary<string, string?> parameters
    )
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(parameters);

        foreach (var (key, value) in parameters)
        {
            uri = uri.WithQueryParameter(key, value);
        }

        return uri;
    }

    /// <summary>
    /// Creates a <see cref="Dictionary{TKey, TValue}"/> from the query string of the absolute URI.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"><paramref name="uri"/> is not an absolute URI, or the URI has multiple entries for a key.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
    public static Dictionary<string, string> GetQueryParameters(this Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        var remainingQuery = uri.QuerySpan();
        var parameters = new Dictionary<string, string>();

        if (remainingQuery is ['?', ..])
        {
            remainingQuery = remainingQuery[1..];
        }

        while (!remainingQuery.IsEmpty)
        {
            var endOfParameterPair = remainingQuery.IndexOf('&');
            ReadOnlySpan<char> currentParameterPair;
            if (endOfParameterPair == -1)
            {
                currentParameterPair = remainingQuery;
                remainingQuery = [];
            }
            else
            {
                currentParameterPair = remainingQuery[..endOfParameterPair];
                remainingQuery = remainingQuery[(endOfParameterPair + 1)..];
            }

            ReadOnlySpan<char> rawCurrentParameterName;
            ReadOnlySpan<char> rawCurrentParameterValue;
            var endOfParameterName = currentParameterPair.IndexOf('=');
            if (endOfParameterName == -1)
            {
                rawCurrentParameterName = currentParameterPair;
                rawCurrentParameterValue = [];
            }
            else
            {
                rawCurrentParameterName = currentParameterPair[..endOfParameterName];
                rawCurrentParameterValue = currentParameterPair[(endOfParameterName + 1)..];
            }

            var currentParameterName = Uri.UnescapeDataString(rawCurrentParameterName.ToString());
            var currentParameterValue = rawCurrentParameterValue.IsEmpty
                ? string.Empty
                : Uri.UnescapeDataString(rawCurrentParameterValue.ToString());

            try
            {
                parameters.Add(currentParameterName, currentParameterValue);
            }
            catch (ArgumentException inner)
            {
                throw new InvalidOperationException(
                    "The URI has multiple entries for a query parameter.",
                    inner
                );
            }
        }

        return parameters;
    }

    public static bool HasQueryParameters(this Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        var index = uri.AbsoluteUri.IndexOf('?');
        var fragment = uri.AbsoluteUri.IndexOf('#');

        return index != -1 && index != uri.AbsoluteUri.Length - 1 && index != fragment - 1;
    }

    public static TModel GetQueryParameters<TModel>(this Uri uri)
        where TModel : IQueryStringModel, ISpanParsable<TModel>
    {
        ArgumentNullException.ThrowIfNull(uri);

        return TModel.Parse(uri.QuerySpan(), null);
    }

    /// <summary>
    /// Gets the escaped query string of the URI as a span.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// The <see cref="Uri"/> class instantiates a <see cref="string"/> for each of its members as they are
    /// accessed. This method avoids that allocation by returning a span of the query string. It is a minor
    /// performance advantage that only has value if the property is not actually accessed. Nevertheless,
    /// for most internal code we operate only on a span and therefore this method is sufficient.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is null</exception>
    /// <exception cref="InvalidOperationException"><paramref name="uri"/> is not absolute</exception>
    public static ReadOnlySpan<char> QuerySpan(this Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        if (!uri.IsAbsoluteUri)
        {
            throw new InvalidOperationException("The URI must be an absolute URI.");
        }

        var startQuery = uri.AbsoluteUri.IndexOf('?');
        if (startQuery == -1)
        {
            return [];
        }
        var endQuery = uri.AbsoluteUri.IndexOf('#');
        if (endQuery == -1)
        {
            endQuery = uri.AbsoluteUri.Length;
        }

        Debug.Assert(endQuery >= startQuery);
        return uri.AbsoluteUri.AsSpan(startQuery, endQuery - startQuery);
    }
}
