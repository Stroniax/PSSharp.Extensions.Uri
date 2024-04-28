namespace PSSharp.Extensions.Uri;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Constructs a query string.
/// <para>
/// <c>?q=foo</c> yields <c>KeyValuePair.Create("q", "foo")</c>
/// </para>
/// <para>
/// <c>?q=&amp;other=arg</c> yields <c>KeyValuePair.Create("q", ""), KeyValuePair.Create("other", "arg")</c>
/// </para>
/// <para>
/// <c>?q&amp;other=arg</c> yields <c>KeyValuePair.Create("q", null), KeyValuePair.Create("other", "arg")</c>
/// </para>
/// </summary>
public sealed class QueryStringBuilder : IReadOnlyCollection<KeyValuePair<string, string?>>
{
    private readonly StringBuilder _builder = new();

    /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
    public int Count() => IsEmpty() ? 0 : _builder.ToString().Count(c => c == '&') + 1;

    int IReadOnlyCollection<KeyValuePair<string, string?>>.Count => Count();

    /// <summary>
    /// Tests if the current builder instance has no elements.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => _builder.Length == 0;

    /// <summary>
    /// Removes all elements from the current instance.
    /// </summary>
    public void Clear() => _builder.Clear();

    /// <summary>
    /// Enuemrates the key-value pairs of the current instance.
    /// </summary>
    /// <returns>An enumerator which iterates through all elements in the current query string.</returns>
    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
    {
        if (IsEmpty())
        {
            yield break;
        }

        // skip '?'
        var remaining = _builder.ToString().AsMemory(1);
        while (remaining.Length > 0)
        {
            // seek to end of current &key=value
            var endOfKeyValue = remaining.Span.IndexOf('&');
            ReadOnlyMemory<char> segment;
            if (endOfKeyValue == -1)
            {
                segment = remaining;
                remaining = ReadOnlyMemory<char>.Empty;
            }
            else
            {
                segment = remaining[..endOfKeyValue];
                remaining = remaining[(endOfKeyValue + 1)..];
            }

            // split at '='
            var equalsIndex = segment.Span.IndexOf('=');
            if (equalsIndex == -1)
            {
                yield return new(Uri.EscapeDataString(segment.ToString()), null);
                continue;
            }

            var key = segment[..equalsIndex];
            var value = segment[(equalsIndex + 1)..];
            yield return new(
                Uri.UnescapeDataString(key.ToString()),
                Uri.UnescapeDataString(value.ToString())
            );
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool ContainsKey(string key)
    {
        if (IsEmpty())
        {
            return false;
        }
        var escapedKey = Uri.EscapeDataString(key);
        var remaining = _builder.ToString().AsSpan(1); // skip '?'
        while (remaining.Length > 0)
        {
            if (
                remaining.StartsWith(escapedKey)
                && (remaining.Length <= escapedKey.Length || remaining[escapedKey.Length] == '=')
            )
            {
                return true;
            }

            var endOfToken = remaining.IndexOf('&');
            if (endOfToken == -1)
            {
                break;
            }
            remaining = remaining[(endOfToken + 1)..];
        }

        return false;
    }

    public bool TryGetFirstValue(string key, out string firstValue)
    {
        if (IsEmpty())
        {
            firstValue = string.Empty;
            return false;
        }

        var escapedKey = Uri.EscapeDataString(key);
        var remaining = _builder.ToString().AsSpan(1); // skip '?'

        var index = remaining.IndexOf(escapedKey);
        while (index != -1)
        {
            if (index == 0 || remaining[index - 1] == '&')
            {
                var valueStart = index + escapedKey.Length;
                if (valueStart < remaining.Length && remaining[valueStart] == '=')
                {
                    var valueEnd = remaining.Slice(valueStart + 1).IndexOf('&');
                    if (valueEnd == -1)
                    {
                        firstValue = Uri.UnescapeDataString(
                            remaining.Slice(valueStart + 1).ToString()
                        );
                        return true;
                    }
                    firstValue = Uri.UnescapeDataString(
                        remaining.Slice(valueStart + 1, valueEnd).ToString()
                    );
                    return true;
                }
            }
            remaining = remaining[(index + 1)..];
            index = remaining.IndexOf(escapedKey);
        }
        firstValue = string.Empty;
        return false;
    }

    public bool TryGetMultipleValues(string key, out string?[] multipleValues)
    {
        var builder = new List<string?>();

        var escapedKey = Uri.EscapeDataString(key);

        foreach (var (eachKey, eachValue) in this)
        {
            if (eachKey == escapedKey)
            {
                builder.Add(eachValue is null ? null : Uri.UnescapeDataString(eachValue));
            }
        }

        multipleValues = builder.ToArray();
        return multipleValues.Length > 0;
    }

    public QueryStringBuilder AddEscaped(string parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        return AddEscaped(parameter.AsSpan());
    }

    public QueryStringBuilder AddEscaped(ReadOnlySpan<char> parameter)
    {
        AssertEscaped(parameter);
        AddDelimiter().Append(parameter);
        return this;
    }

    public QueryStringBuilder AddEscaped(string parameter, string value)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        ArgumentNullException.ThrowIfNull(value);
        return AddEscaped(parameter.AsSpan(), value.AsSpan());
    }

    public QueryStringBuilder AddEscaped(ReadOnlySpan<char> parameter, ReadOnlySpan<char> value)
    {
        AssertEscaped(parameter);
        AssertEscaped(value);
        AddDelimiter().Append(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter)
    {
        AddDelimiter();
        EscapeAndAdd(parameter);
        return this;
    }

    public QueryStringBuilder Add(string parameter, string value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=');
        EscapeAndAdd(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, int value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, long value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, float value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, double value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, decimal value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, bool value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value ? "true" : "false");
        return this;
    }

    public QueryStringBuilder Add(string parameter, uint value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, ulong value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, byte value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, sbyte value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, short value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, ushort value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, char value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value);
        return this;
    }

    public QueryStringBuilder Add(string parameter, DateTime value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value.ToString("O"));
        return this;
    }

    public QueryStringBuilder Add(string parameter, DateTimeOffset value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value.ToString("O"));
        return this;
    }

    public QueryStringBuilder Add(string parameter, TimeSpan value)
    {
        AddDelimiter();
        EscapeAndAdd(parameter).Append('=').Append(value.ToString("c"));
        return this;
    }

    public QueryStringBuilder AddConditional<T>(
        T arg,
        Func<T, bool> condition,
        Func<T, string> parameter,
        Func<T, string> value,
        out bool added
    )
    {
        if (condition(arg))
        {
            Add(parameter(arg), value(arg));
            added = true;
        }
        else
        {
            added = false;
        }
        return this;
    }

    public QueryStringBuilder AddConditional(bool condition, string parameter, string value)
    {
        if (condition)
        {
            Add(parameter, value);
        }
        return this;
    }

    private StringBuilder EscapeAndAdd(
        string parameter,
        [CallerArgumentExpression(nameof(parameter))] string? parameterName = null
    )
    {
        ArgumentNullException.ThrowIfNull(parameter, parameterName);

        //return _builder.Append(Uri.EscapeDataString(parameter));
        return EscapeAndAdd(parameter.AsSpan());
    }

    private StringBuilder EscapeAndAdd(ReadOnlySpan<char> parameter)
    {
        for (var i = 0; i < parameter.Length; i++)
        {
            var ch = parameter[i];
            if (
                ch
                is ':'
                    or '/'
                    or '?'
                    or '#'
                    or '['
                    or ']'
                    or '@'
                    or '!'
                    or '$'
                    or '&'
                    or '\''
                    or '('
                    or ')'
                    or '*'
                    or '+'
                    or ','
                    or ';'
                    or '='
                    or '%'
                    or ' '
            )
            {
                WriteEscaped(ch, _builder);
            }
            else
            {
                _builder.Append(ch);
            }
        }

        return _builder;

        static void WriteEscaped(char c, StringBuilder sb)
        {
            sb.Append('%');
            sb.Append((byte)c >> 4);
            sb.Append((byte)c & 0x0F);
        }
    }

    public static bool TryParse(
        ReadOnlySpan<char> value,
        [MaybeNullWhen(false)] out QueryStringBuilder builder
    )
    {
        AssertEscaped(value);
        builder = [];
        builder._builder.Append(value);
        return true;
    }

    private static void AssertEscaped(
        ReadOnlySpan<char> value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null
    )
    {
        var maxPercentIndex = value.Length - 2;
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            // https://datatracker.ietf.org/doc/html/rfc3986#section-2.2
            if (
                c
                    is ':'
                        or '/'
                        or '?'
                        or '#'
                        or '['
                        or ']'
                        or '@'
                        or '!'
                        or '$'
                        or '&'
                        or '\''
                        or '('
                        or ')'
                        or '*'
                        or '+'
                        or ','
                        or ';'
                        or '='
                        or '%'
                        or ' ' // should be %20 or + depending on context
                || (
                    c == '%'
                    && (
                        i > maxPercentIndex
                        || !Uri.IsHexDigit(value[i + 1])
                        || !Uri.IsHexDigit(value[i + 2])
                    )
                )
            )
            {
                throw new ArgumentException(
                    "The value cannot contain characters that must be percent-encoded. For a full list of characters that must be percent-encoded, see https://datatracker.ietf.org/doc/html/rfc3986#section-2.2.",
                    parameterName
                );
            }
        }
    }

    private StringBuilder AddDelimiter()
    {
        if (_builder.Length == 0)
        {
            _builder.Append('?');
        }
        else
        {
            _builder.Append('&');
        }
        return _builder;
    }

    /// <summary>
    /// Constructs a well-formed, escaped string representation of the current instance.
    /// </summary>
    /// <returns>The query string representing the current state of this builder.</returns>
    public override string ToString() => _builder.ToString();
}
