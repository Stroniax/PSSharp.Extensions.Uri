namespace PSSharp.Extensions.Uri;

using System;

public abstract record DeserializerImplementation
{
    private DeserializerImplementation() { }

    /// <summary>
    /// The type implements <c>System.IParsable&lt;T&gt;</c> which should be used.
    /// </summary>
    public sealed record Parse : DeserializerImplementation;

    /// <summary>
    /// The type implements <c>System.ISpanParsable&lt;T&gt;</c> which should be used.
    /// </summary>
    public sealed record SpanParse : DeserializerImplementation;

    /// <summary>
    /// <code><c>
    /// [accessibility] [inheritance] static bool {MethodName}(
    ///     [string memberName,]
    ///     [ReadOnlySpan&lt;char&gt; uriEncodedParameterName,]
    ///     ReadOnlySpan&lt;char&gt; uriEncodedParameterValue,
    ///     [bool hasValue,]
    ///     out T result
    /// );
    /// </c></code>
    /// </summary>
    public sealed record DeserializeFromMember(
        string MethodName,
        bool HasMemberName,
        bool HasEncodedNameParameter,
        bool HasHasValueParameter
    ) : DeserializerImplementation;

    /// <summary>
    /// <code><c>
    /// [accessibility] [inheritance] static bool {MethodName}(
    ///     [string memberName,]
    ///     ReadOnlySpan&lt;char&gt; queryString,
    ///     out T value
    /// );
    /// </c></code>
    /// </summary>
    /// <param name="MethodName"></param>
    public sealed record DeserializeFromQueryString(string MethodName, bool HasMemberName)
        : DeserializerImplementation;

    public void Switch(
        Action parse,
        Action spanParse,
        Action<DeserializeFromMember> member,
        Action<DeserializeFromQueryString> queryString
    )
    {
        switch (this)
        {
            case Parse:
                parse();
                break;
            case SpanParse:
                spanParse();
                break;
            case DeserializeFromMember m:
                member(m);
                break;
            case DeserializeFromQueryString q:
                queryString(q);
                break;
            default:
                throw new NotSupportedException($"Unmatched case: {GetType().Name}.");
        }
    }
}
