namespace PSSharp.Extensions.Uri;

/// <summary>
/// Methods to generate for an implementation of <c>IQueryStringModel</c>.
/// </summary>
/// <param name="Parsable"></param>
/// <param name="SpanParsable"></param>
/// <param name="RecordPrintMembers">Generate a PrintMembers method if the type is a record type with a <c>partial bool PrintMembers(StringBuilder)</c> method.</param>
public readonly partial record struct QueryStringModelMethods(
    bool AppendQueryString,
    ParsableSignatures Parsable,
    ParsableSignatures SpanParsable,
    bool RecordPrintMembers
);
