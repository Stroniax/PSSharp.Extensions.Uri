namespace PSSharp.Extensions.Uri;

using System;

public abstract record SerializerImplementation
{
    private SerializerImplementation() { }

    public sealed record UnencodedString : SerializerImplementation;

    public sealed record EncodedString : SerializerImplementation;

    /// <summary>
    /// <code><c>
    /// [accessibility] [inheritence] [static] void {MethodName}(
    ///     StringBuilder query,
    ///     [TInstance instance,]
    ///     [string memberName,]
    ///     TMember value,
    ///     ref bool hasQueryParams
    /// );
    /// </c></code>
    /// </summary>
    public sealed record Method(
        string MethodName,
        bool IsMethodStatic,
        bool HasSelfParaemter,
        bool HasMemberNameParameter,
        bool HasMemberValueParameter
    ) : SerializerImplementation;

    public void Switch(Action unencodedString, Action encodedString, Action<Method> method)
    {
        switch (this)
        {
            case UnencodedString:
                unencodedString();
                break;
            case EncodedString:
                encodedString();
                break;
            case Method m:
                method(m);
                break;
            default:
                throw new NotSupportedException($"Unmatched case: {GetType().Name}.");
        }
    }
}
