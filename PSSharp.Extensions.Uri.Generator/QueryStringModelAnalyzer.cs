namespace PSSharp.Extensions.Uri;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class QueryStringModelAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            UriDiagnostics.URI00001MemberNotPartialForSourceImplementation,
            UriDiagnostics.URI00002TypeNotPartialForSourceImplementation,
            UriDiagnostics.URI00003NoQueryStringMembers,
            UriDiagnostics.URI00004MemberTypeNotParsable,
            UriDiagnostics.URI00005InvalidQueryStringParameterName,
            UriDiagnostics.URI000006MemberNotCollection,
            UriDiagnostics.URI00007QueryStringParameterNameNull
        );

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics
        );
    }
}
