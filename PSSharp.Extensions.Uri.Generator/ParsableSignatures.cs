namespace PSSharp.Extensions.Uri;

using System;

[Flags]
public enum ParsableSignatures
{
    None,

    /// <summary>
    /// Explicit interface implementaiton
    /// </summary>
    Explicit = 1,

    ParseStringIFormatProvider = 2,

    ParseString = 4,

    TryParseExplicit = 8,

    TryParseStringIFormatProvider = 16,

    TryParseString = 32,
}
