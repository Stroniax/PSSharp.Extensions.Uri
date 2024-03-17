namespace PSSharp.Extensions.Uri;

using System;

public static partial class UriExtensions
{
    /// <summary>
    /// Determines the length of the decoded value of a percent-encoded URI string. This method accpets
    /// badly formed input such as input with ' ' characters or '%' characters that are not percent-encoding
    /// (not followed by two hex characters) and will produce a length that is accurate to decode the string.
    /// </summary>
    /// <param name="encodedValue"></param>
    /// <returns></returns>
    internal static int CalculateLengthWhenDecoded(ReadOnlySpan<char> encodedValue)
    {
        // CalculateLengthWhenDecodedBenchmark.While_Seek2
        // Optimized for short, percent-encoded text (most likely to receive from System.Uri)

        var length = 0;
        var index = 0;
        var lengthIfPercent = encodedValue.Length - 2;

        while (index < encodedValue.Length)
        {
            var c = encodedValue[index];
            if (
                c == '%'
                && index < lengthIfPercent
                && char.IsAsciiHexDigit(encodedValue[index + 1])
                && char.IsAsciiHexDigit(encodedValue[index + 2])
            )
            {
                length++;
                index += 3;
                continue;
            }
            else
            {
                length++;
                index++;
            }
        }
        return length;
    }

    /// <summary>
    /// Determines the length of the decoded value of a percent-encoded URI string. This method can accept
    /// input directly from <see cref="Uri.AbsoluteUri"/> which is known to be well-formed: that is,
    /// percent-encoded properly. Misplaced percent characters will cause undefined behavior; for raw user input,
    /// call <see cref="CalculateLengthWhenDecoded(ReadOnlySpan{char})"/>.
    /// </summary>
    /// <param name="encodedValue">A well-formed percent-encoded value. Knowing this value is well-formed
    /// (no non-encoding '%' characters) allows skipping some validation when calculating length.</param>
    /// <returns></returns>
    internal static int CaclucateLengthWhenDecodedWellFormed(ReadOnlySpan<char> encodedValue)
    {
        var length = 0;
        var index = 0;

        while (index < encodedValue.Length)
        {
            var c = encodedValue[index];
            // this method is given well-formed input - so no hex validation
            if (c == '%')
            {
                length++;
                index += 3;
                continue;
            }
            else
            {
                length++;
                index++;
            }
        }
        return length;
    }
}
