namespace PSSharp.Extensions.Uri;

/// <summary>
///
/// </summary>
/// <param name="Parse"></param>
/// <param name="TryParse"></param>
/// <param name="SpanParse"></param>
/// <param name="SpanTryParse"></param>
/// <param name="RecordPrintMembers">Generate a PrintMembers method if the type is a record type with a <c>partial bool PrintMembers(StringBuilder)</c> method.</param>
public readonly record struct QueryStringModelMethods(
    bool AppendQueryString,
    bool Parse,
    bool TryParse,
    bool SpanParse,
    bool SpanTryParse,
    bool RecordPrintMembers
);
