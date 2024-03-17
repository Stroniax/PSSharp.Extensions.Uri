namespace PSSharp.Extensions.Uri;

using System;
using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class UriSpanDecodeBenchmark
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

    [Benchmark(Baseline = true)]
    public string Uri()
    {
        return System.Uri.UnescapeDataString(EncodedValue);
    }

    [Benchmark]
    public string CharArrayToString()
    {
        UriExtensions.TryUnescapeDataString(EncodedValue, [], out var length);
        var buffer = new char[length];
        UriExtensions.TryUnescapeDataString(EncodedValue, buffer, out _);
        return new string(buffer);
    }

    [Benchmark]
    public string StackAllocSpanToString()
    {
        UriExtensions.TryUnescapeDataString(EncodedValue, [], out var length);
        var buffer = (Span<char>)stackalloc char[length];
        UriExtensions.TryUnescapeDataString(EncodedValue, buffer, out _);
        return new string(buffer);
    }

    [Benchmark]
    public void StackAllocSpan()
    {
        UriExtensions.TryUnescapeDataString(EncodedValue, [], out var length);
        var buffer = (Span<char>)stackalloc char[length];
        UriExtensions.TryUnescapeDataString(EncodedValue, buffer, out _);
    }

    [Benchmark]
    public string FastUnescapeDataString()
    {
        Span<char> buffer = stackalloc char[EncodedValue.Length];
        UriExtensions.FastUnescapeDataString(EncodedValue, buffer);
        return new string(buffer[..(buffer.IndexOf('\0'))]);
    }

    [Benchmark]
    public void FastUnescapeDataSpan()
    {
        Span<char> buffer = stackalloc char[EncodedValue.Length];
        UriExtensions.FastUnescapeDataString(EncodedValue, buffer);
    }
}
