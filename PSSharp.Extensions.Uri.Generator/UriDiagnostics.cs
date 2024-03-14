namespace PSSharp.Extensions.Uri;

using Microsoft.CodeAnalysis;

public static class UriDiagnostics
{
    public static readonly DiagnosticDescriptor URI00001MemberNotPartialForSourceImplementation =
        new(
            "URI00001",
            "Declare partial member IQueryStringModel.AppendQueryString",
            "Declare partial member IQueryStringModel.AppendQueryString for {0}. IQueryStringModel is not generating code because the member does not have a partial declaration.",
            "PSSharp.Usage",
            DiagnosticSeverity.Warning,
            true
        );

    public static readonly DiagnosticDescriptor URI00002TypeNotPartialForSourceImplementation =
        new(
            "URI00002",
            "Make type definition partial",
            "Make the type definition for {0} partial. IQueryStringModel is not generating code because the target type cannot be extended by a partial implementation.",
            "PSSharp.Usage",
            DiagnosticSeverity.Error,
            true,
            customTags: ["PSSharp.Extensions.Uri.QueryString"]
        );

    public static readonly DiagnosticDescriptor URI00003NoQueryStringMembers =
        new(
            "URI00003",
            "No query string members",
            "The type {0} has no members that can be de/serialized for a query string",
            "PSSharp.Usage",
            DiagnosticSeverity.Warning,
            true,
            customTags: ["PSSharp.Extensions.Uri.QueryString"]
        );

    public static readonly DiagnosticDescriptor URI00004MemberTypeNotParsable =
        new(
            "URI00004",
            "Query string member is not parsable",
            "The member {0} is not a parsable type. De/serialization code will not be generated for this member. Declare members as types that implement IParsable<T> or user [QueryStringIgnore] to avoid generating code for this member.",
            "PSSharp.Usage",
            DiagnosticSeverity.Error,
            true,
            customTags: ["PSSharp.Extensions.Uri.QueryString"]
        );

    public static readonly DiagnosticDescriptor URI00005InvalidQueryStringParameterName =
        new(
            "URI00005",
            "Invalid query string parameter name",
            "The name '{0}' is not a valid URI-encoded string. Use a URI-encoded name for the property value.",
            "PSSharp.Usage",
            DiagnosticSeverity.Error,
            true,
            customTags: ["PSSharp.Extensions.Uri.QueryString"]
        );

    public static readonly DiagnosticDescriptor URI000006MemberNotCollection =
        new(
            "URI00006",
            "Member is not a collection",
            "The member {0} is not a collection type. [QueryStringCommaSeparatedCollection] only affects code generation for collection types that implement IEnumerable<T>.",
            "PSSharp.Usage",
            DiagnosticSeverity.Error,
            true,
            customTags: ["PSSharp.Extensions.Uri.QueryString"]
        );

    public static readonly DiagnosticDescriptor URI00007QueryStringParameterNameNull =
        new(
            "URI00007",
            "Query string parameter name cannot be null",
            "The name of the query string parameter cannot be null. Provide a non-null value to [QueryStringParameter(string name))].",
            "PSSharp.Usage",
            DiagnosticSeverity.Error,
            true,
            customTags: ["PSSharp.Extensions.Uri.QueryString"]
        );
}
