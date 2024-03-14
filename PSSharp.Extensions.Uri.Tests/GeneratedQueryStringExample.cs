namespace PSSharp.Extensions.Uri;

using System.Text;

internal partial record GeneratedQueryStringExample : IQueryStringModel
{
    public string Test { get; set; } = string.Empty;

    [QueryStringParameter("foo")]
    [QueryStringCommaSeparatedCollection]
    public string[] Foo { get; set; } = [];

    [QueryStringParameter("bar")]
    public string[] Bar { get; set; } = [];

    [QueryStringCommaSeparatedCollection]
    public int[] Baz { get; set; } = [];

    public int[] Qux { get; set; } = [];

    public partial void AppendQueryString(StringBuilder query, ref bool hasQueryParams);

    protected virtual partial bool PrintMembers(StringBuilder writer);

    //public static partial GeneratedQueryStringExample Parse(
    //    string query,
    //    IFormatProvider? formatProvider
    //);

    //public static partial bool TryParse(
    //    string query,
    //    IFormatProvider? formatProvider,
    //    out GeneratedQueryStringExample e
    //);
}
