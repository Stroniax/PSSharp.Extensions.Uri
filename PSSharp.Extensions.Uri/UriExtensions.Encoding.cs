namespace PSSharp.Extensions.Uri;

using System;

public static partial class UriExtensions
{
    private static void PercentEncode(char c, Span<char> buffer)
    {
        buffer[0] = '%';
        buffer[1] = HexChars[c >> 4];
        buffer[2] = HexChars[c & 0xF];
    }

    internal static int CalculateLengthWhenEncoded(ReadOnlySpan<char> value)
    {
        var i = value.IndexOfAny(RequiresEncoding);

        if (i == -1)
        {
            return value.Length;
        }

        var length = i;
        for (; i < value.Length; i++)
        {
            if (RequiresEncoding.Contains(value[i]))
            {
                length += 3;
            }
            else
            {
                length += 1;
            }
        }

        return length;
    }

    private static ReadOnlySpan<char> RequiresEncoding => ":/?#[]@!$&'()*+,;=";
}
