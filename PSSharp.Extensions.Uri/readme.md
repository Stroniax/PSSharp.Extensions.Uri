# PSSharp.Extensions.Uri

## Examples

### Query String

```C#
var uri = new Uri("http://example.com?name=John&age=30&birthDate=1985-01-01");
var query = uri.ParseQueryString<MyEndpointQuery>();
```

## RFCs

- [RFC 3986](https://tools.ietf.org/html/rfc3986) - Uniform Resource Identifier (URI): Generic Syntax
- [RFC 6570](https://tools.ietf.org/html/rfc6570) - URI Template

## The Story

This library began with my desire to strongly type a query string model and get automatically implemented
de/serialization.

```C#

// My code
public partial class MyEndpointQuery
{
	public string? Name { get; set; }
	public int? Age { get; set; }
	public DateTime? BirthDate { get; set; }
}

// What I want for free
public partial class MyEndpointQuery
{
	public string? Name { get; set; }
	public int? Age { get; set; }
	public DateTime? BirthDate { get; set; }

	public string ToQueryString()
	{
		// Name=John&Age=30&BirthDate=1985-01-01
	}

	public static MyEndpointQuery FromQueryString(string queryString)
	{
		// ...
	}
}
```

