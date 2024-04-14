namespace PSSharp.Extensions.Uri;

using System.Text;

internal partial record GeneratedQueryStringExample : IQueryStringModel
{
    public string Test { get; set; } = string.Empty;

    [QueryStringParameter("foo")]
    public string[] Foo { get; set; } = [];

    [QueryStringParameter("bar")]
    public string[] Bar { get; set; } = [];

    public int[] Baz { get; set; } = [];

    [QueryStringCondition("IfQux")]
    [QueryStringSerializer("WriteQux")]
    public int[] Qux { get; set; } = [];

    private bool IfQux()
    {
        return true;
    }

    private void WriteQux(
        QueryStringBuilder ex,
        GeneratedQueryStringExample exx,
        string memberName,
        int[] qux
    )
    {
        throw new NotImplementedException();
    }

    public partial void AddToQueryString(QueryStringBuilder builder);

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
