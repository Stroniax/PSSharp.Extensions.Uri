namespace PSSharp.Extensions.Uri;

using System;

public abstract record SerializerImplementation
{
    private SerializerImplementation() { }

    /// <summary>
    /// The value is some primitive type that will not need encoding, e.g. <see cref="int"/>.
    /// </summary>
    public sealed record UnencodedString : SerializerImplementation;

    /// <summary>
    /// The value is some type that must be encoded by calling <c>Uri.EscapeDataString(this.Property.ToString())</c>.
    /// </summary>
    public sealed record EncodedString : SerializerImplementation;

    /// <summary>
    /// <code><c>
    /// [accessibility] [inheritence] [static] void {MethodName}(
    ///     QueryStringBuilder query,
    ///     [TInstance instance,]
    ///     [string memberName,]
    ///     [TMember value]
    /// );
    /// </c></code>
    /// </summary>
    public sealed record Method(string MethodName) : SerializerImplementation;

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
