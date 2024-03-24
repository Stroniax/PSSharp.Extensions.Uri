namespace PSSharp.Extensions.Uri;

using System;

public abstract record DeserializerImplementation
{
    private DeserializerImplementation() { }

    public sealed record Parse : DeserializerImplementation;

    public sealed record SpanParse : DeserializerImplementation;

    /// <summary>
    /// <code><c>
    /// [accessibility] [inheritance] static bool {MethodName}(
    ///     [string memberName,]
    ///     [ReadOnlySpan&lt;char&gt; uriEncodedParameterName,]
    ///     ReadOnlySpan&lt;char&gt; uriEncodedParameterValue,
    ///     out T result
    /// );
    /// </c></code>
    /// </summary>
    public sealed record Method(string MethodName, bool HasMemberName, bool HasEncodedNameParameter)
        : DeserializerImplementation;

    public void Switch(Action parse, Action spanParse, Action<Method> method)
    {
        switch (this)
        {
            case Parse:
                parse();
                break;
            case SpanParse:
                spanParse();
                break;
            case Method m:
                method(m);
                break;
            default:
                throw new NotSupportedException($"Unmatched case: {GetType().Name}.");
        }
    }
}
