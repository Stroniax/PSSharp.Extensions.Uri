namespace PSSharp.Extensions.Uri;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

/// <summary>
/// Defines extension methods for <see cref="Uri"/> and related types.
/// </summary>
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

    public static bool HasQueryParameters(this Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        var index = uri.AbsoluteUri.IndexOf('?');
        var fragment = uri.AbsoluteUri.IndexOf('#');

        return index != -1 && index != uri.AbsoluteUri.Length - 1 && index != fragment - 1;
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
    /// <para>
    /// <a href="https://datatracker.ietf.org/doc/html/rfc3986#section-3.4">RFC</a>
    /// </para>
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

    public static TModel GetQueryParameters<TModel>(this Uri uri)
        where TModel : IQueryStringModel, ISpanParsable<TModel>
    {
        ArgumentNullException.ThrowIfNull(uri);

        return TModel.Parse(uri.QuerySpan(), null);
    }

    public static bool TryGetQueryParameter<T>(
        this Uri uri,
        string parameterName,
        [MaybeNullWhen(false)] out T parameterValue
    )
        where T : IParsable<T>
    {
        if (uri.TryGetQueryParameter(parameterName, out var stringValue))
        {
            return T.TryParse(stringValue, null, out parameterValue);
        }
        else
        {
            parameterValue = default;
            return false;
        }
    }

    public static bool TryGetQueryParameter(
        this Uri uri,
        string parameterName,
        [MaybeNullWhen(false)] out string parameterValue
    )
    {
        if (uri.TryGetQueryParameterSpan(parameterName, out var encodedValue))
        {
            parameterValue = Uri.UnescapeDataString(encodedValue.ToString());
            return true;
        }

        parameterValue = default;
        return false;
    }

    public static bool TryGetQueryParameterSpan(
        this Uri uri,
        string parameterName,
        out ReadOnlySpan<char> encodedValue
    )
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(parameterName);

        var remainingQuery = uri.QuerySpan();
        if (remainingQuery.IsEmpty)
        {
            encodedValue = default;
            return false;
        }

        var escapedParameter = Uri.EscapeDataString(parameterName).AsSpan();
        var parameterIndex = remainingQuery.IndexOf(escapedParameter);
        if (parameterIndex == -1)
        {
            encodedValue = default;
            return false;
        }
        // parameterIndex != 0: char 0 is always '?'
        if (
            parameterIndex == 1 && remainingQuery[parameterIndex - 1] == '?'
            || (remainingQuery[parameterIndex - 1] != '&')
        )
        {
            encodedValue = default;
            return false;
        }

        var valueIndex = parameterIndex + escapedParameter.Length;
        if (valueIndex == remainingQuery.Length)
        {
            encodedValue = default;
            return true;
        }

        if (remainingQuery[valueIndex] != '=')
        {
            encodedValue = default;
            return false;
        }

        var valueStart = valueIndex + 1;
        var valueEnd = remainingQuery[valueStart..].IndexOfAny('&', '#');
        if (valueEnd == -1)
        {
            valueEnd = remainingQuery.Length;
        }
        else
        {
            valueEnd += valueStart;
        }

        encodedValue = remainingQuery[valueStart..valueEnd];
        return true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="valueToUnescape">The encoded value to be unescaped.</param>
    /// <param name="destination">A buffer into which the string may be decoded. Either the entire decoded
    /// value will be written, or no data will be written; no partial-decoding will occur.</param>
    /// <param name="length">The length required to decode the string.</param>
    /// <returns>A value indicating whether the string could be unescaped. If
    /// <see langword="false"/>, resize the buffer and try again.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static bool TryUnescapeDataString(
        ReadOnlySpan<char> valueToUnescape,
        Span<char> destination,
        out int length
    )
    {
        length = CalculateLengthWhenDecoded(valueToUnescape);
        if (destination.Length < length)
        {
            return false;
        }

        var percentIndex = valueToUnescape.IndexOf('%');
        while (percentIndex != -1)
        {
            // copy the characters before the percent
            valueToUnescape[..percentIndex].CopyTo(destination);
            destination = destination[percentIndex..];
            valueToUnescape = valueToUnescape[percentIndex..];

            if (
                valueToUnescape.Length < 3
                || !char.IsAsciiHexDigit(valueToUnescape[1])
                || !char.IsAsciiHexDigit(valueToUnescape[2])
            )
            {
                destination[0] = valueToUnescape[0];
                destination = destination[1..];
                valueToUnescape = valueToUnescape[1..];
            }
            else
            {
                destination[0] = (char)(
                    HexChars.IndexOf(valueToUnescape[1]) << 4 | HexChars.IndexOf(valueToUnescape[2])
                );
                destination = destination[1..];
                valueToUnescape = valueToUnescape[3..];
            }

            percentIndex = valueToUnescape.IndexOf('%');
        }

        valueToUnescape.CopyTo(destination);

        return true;
    }

    private static ReadOnlySpan<char> HexChars => "0123456789ABCDEF";
}
