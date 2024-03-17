namespace PSSharp.Extensions.Uri;

using System;
using BenchmarkDotNet.Attributes;

[RankColumn]
public class CalculateLengthWhenDecodedBenchmark
{
    [Params(
        "path%20data%20string",
        "value without encoding",
        "simple",
        "Something%20prett%79%20complex%2C%20eh%3F",
        "And, something more complex but without encoding!",
        "with%bad%20encoding%"
    )]
    public string EncodedValue { get; set; } = string.Empty;

    /// <summary>
    /// IndexOf, then reassign the span to the remaining portion
    /// </summary>
    /// <returns></returns>
    [Benchmark]
    public int IndexOfPercent_Shorten()
    {
        var encodedValue = EncodedValue.AsSpan();
        int percentIndex = encodedValue.IndexOf('%');
        var length = 0;

        while (percentIndex != -1)
        {
            if (
                percentIndex + 2 >= encodedValue.Length
                || !char.IsAsciiHexDigit(encodedValue[percentIndex + 1])
                || !char.IsAsciiHexDigit(encodedValue[percentIndex + 2])
            )
            {
                length += percentIndex + 1;
                encodedValue = encodedValue[(percentIndex + 1)..];
            }
            else
            {
                length += percentIndex + 1;
                encodedValue = encodedValue[(percentIndex + 3)..];
            }

            percentIndex = encodedValue.IndexOf('%');
        }

        length += encodedValue.Length;
        return length;
    }

    /// <summary>
    /// IndexOf, tracking the 'seek from' index
    /// </summary>
    /// <returns></returns>
    [Benchmark]
    public int IndexOfPercent_Index()
    {
        var encodedValue = EncodedValue.AsSpan();
        var startIndex = 0;
        var length = 0;

        do
        {
            // instead of resizing the span, keep track of the index
            var percentIndex = encodedValue[startIndex..].IndexOf('%');
            if (percentIndex == -1)
            {
                length += encodedValue.Length - startIndex;
                break;
            }

            if (
                percentIndex + 2 >= encodedValue.Length
                || !char.IsAsciiHexDigit(encodedValue[percentIndex + 1])
                || !char.IsAsciiHexDigit(encodedValue[percentIndex + 2])
            )
            {
                length += percentIndex + 1;
                startIndex += percentIndex + 1;
            }
            else
            {
                length += percentIndex + 1;
                startIndex += percentIndex + 3;
            }
        } while (true);

        return length;
    }

    /// <summary>
    /// While loop, skips when encoded %
    /// </summary>
    /// <returns></returns>
    [Benchmark]
    public int While_Seek()
    {
        var encodedValue = EncodedValue.AsSpan();

        var length = 0;
        var index = 0;
        while (index < encodedValue.Length)
        {
            var c = encodedValue[index];
            if (c == '%')
            {
                // check if the next two characters are valid hex
                if (
                    index + 2 >= encodedValue.Length
                    || !char.IsAsciiHexDigit(encodedValue[index + 1])
                    || !char.IsAsciiHexDigit(encodedValue[index + 2])
                )
                {
                    length++;
                    index++;
                    continue;
                }
                length++;
                index += 3;
            }
            else
            {
                length++;
                index++;
            }
        }
        return length;
    }

    [Benchmark]
    public int While_Seek2()
    {
        var encodedValue = EncodedValue.AsSpan();

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
    /// While loop, skips when encoded %, starts with index shortcut
    /// </summary>
    /// <returns></returns>
    [Benchmark]
    public int While_Seek_Index()
    {
        var encodedValue = EncodedValue.AsSpan();

        if (encodedValue.IndexOf('%') == -1)
        {
            return encodedValue.Length;
        }

        var length = 0;
        var index = 0;
        while (index < encodedValue.Length)
        {
            var c = encodedValue[index];
            if (c == '%')
            {
                // check if the next two characters are valid hex
                if (
                    index + 2 >= encodedValue.Length
                    || !char.IsAsciiHexDigit(encodedValue[index + 1])
                    || !char.IsAsciiHexDigit(encodedValue[index + 2])
                )
                {
                    length++;
                    index++;
                    continue;
                }
                length++;
                index += 3;
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
    /// While loop, skips when encoded %, starts with index shortcut
    /// </summary>
    /// <returns></returns>
    [Benchmark]
    public int While_Seek_SmartIndex()
    {
        var encodedValue = EncodedValue.AsSpan();

        var index = encodedValue.IndexOf('%');

        if (index == -1)
        {
            return encodedValue.Length;
        }

        var length = index;
        while (index < encodedValue.Length)
        {
            var c = encodedValue[index];
            if (
                c == '%'
                && index < encodedValue.Length - 2
                && char.IsAsciiHexDigit(encodedValue[index + 1])
                && char.IsAsciiHexDigit(encodedValue[index + 2])
            )
            {
                length++;
                index += 3;
            }
            else
            {
                length++;
                index++;
            }
        }
        return length;
    }

    [Benchmark]
    public int While_Seek_SmartIndex2()
    {
        var encodedValue = EncodedValue.AsSpan();

        var index = encodedValue.IndexOf('%');

        if (index == -1)
        {
            return encodedValue.Length;
        }

        var lengthIfPercent = encodedValue.Length - 1;

        var length = index;
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
    /// For loop, uses count to determine distance past %
    /// </summary>
    /// <returns></returns>
    [Benchmark]
    public int For_CountPast()
    {
        var encodedValue = EncodedValue.AsSpan();

        var length = 0;
        var skipCount = 0;

        for (int index = 0; index < encodedValue.Length; index++)
        {
            var c = encodedValue[index];

            if (skipCount > 0)
            {
                if (char.IsAsciiHexDigit(c))
                {
                    if (++skipCount == 3)
                    {
                        length += 1;
                        skipCount = 0;
                    }
                }
                else
                {
                    length += skipCount + 1;
                    skipCount = 0;
                }
            }
            else if (c == '%')
            {
                skipCount = 1;
            }
            else
            {
                length++;
            }
        }

        return length + skipCount;
    }

    /// <summary>
    /// For loop, uses count to determine distance past %
    /// </summary>
    /// <returns></returns>
    [Benchmark]
    public int For_Seek()
    {
        var encodedValue = EncodedValue.AsSpan();

        var length = 0;

        for (int index = 0; index < encodedValue.Length; index++)
        {
            var c = encodedValue[index];

            if (
                c == '%'
                && encodedValue.Length > index + 2
                && char.IsAsciiHexDigit(encodedValue[index + 1])
                && char.IsAsciiHexDigit(encodedValue[index + 2])
            )
            {
                length++;
                index += 2;
            }
            else
            {
                length++;
            }
        }

        return length;
    }

    [Benchmark]
    public int For_Seek_Index()
    {
        var encodedValue = EncodedValue.AsSpan();

        if (encodedValue.IndexOf('%') == -1)
        {
            return encodedValue.Length;
        }

        var length = 0;

        for (int index = 0; index < encodedValue.Length; index++)
        {
            var c = encodedValue[index];

            if (
                c == '%'
                && encodedValue.Length > index + 2
                && char.IsAsciiHexDigit(encodedValue[index + 1])
                && char.IsAsciiHexDigit(encodedValue[index + 2])
            )
            {
                length++;
                index += 2;
            }
            else
            {
                length++;
            }
        }

        return length;
    }

    [Benchmark]
    public int For_Seek_SmartIndex()
    {
        var encodedValue = EncodedValue.AsSpan();

        var index = encodedValue.IndexOf('%');
        if (index == -1)
        {
            return encodedValue.Length;
        }

        var length = index;

        for (; index < encodedValue.Length; index++)
        {
            var c = encodedValue[index];

            if (
                c == '%'
                && encodedValue.Length > index + 2
                && char.IsAsciiHexDigit(encodedValue[index + 1])
                && char.IsAsciiHexDigit(encodedValue[index + 2])
            )
            {
                length++;
                index += 2;
            }
            else
            {
                length++;
            }
        }

        return length;
    }
}
