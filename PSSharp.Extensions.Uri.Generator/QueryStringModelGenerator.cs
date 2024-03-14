namespace PSSharp.Extensions.Uri;

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class QueryStringModelGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var possibleQueryStringModelImplementation = context.SyntaxProvider.CreateSyntaxProvider(
            (node, cancellationToken) =>
            {
                return node
                        is TypeDeclarationSyntax
                        {
                            Modifiers: { Count: > 0 } modifiers,
                            BaseList.Types.Count: > 0
                        }
                    && modifiers.Any(SyntaxKind.PartialKeyword);
            },
            (context, cancellationToken) =>
            {
                var node = (TypeDeclarationSyntax)context.Node;
                var symbol = context.SemanticModel.GetDeclaredSymbol(node, cancellationToken);

                return symbol;
            }
        );

        var queryStringModel = context.CompilationProvider.Select(
            (compilation, cancellationToken) =>
                compilation.GetTypeByMetadataName("PSSharp.Extensions.Uri.IQueryStringModel")
        );

        var otherTypes = context.CompilationProvider.Select(
            (compilation, cancellationToken) =>
                (
                    QueryStringParameterAttribute: compilation.GetTypeByMetadataName(
                        "PSSharp.Extensions.Uri.QueryStringParameterAttribute"
                    ),
                    QueryStringCommaSeparatedCollectionAttribute: compilation.GetTypeByMetadataName(
                        "PSSharp.Extensions.Uri.QueryStringCommaSeparatedCollectionAttribute"
                    ),
                    QueryStringIgnoreAttribute: compilation.GetTypeByMetadataName(
                        "PSSharp.Extensions.Uri.QueryStringIgnoreAttribute"
                    ),
                    SpanParsable: compilation.GetTypeByMetadataName("System.ISpanParsable`1"),
                    Parsable: compilation.GetTypeByMetadataName("System.IParsable`1"),
                    String: compilation.GetSpecialType(SpecialType.System_String),
                    IFormatProvider: compilation.GetTypeByMetadataName("System.IFormatProvider"),
                    Boolean: compilation.GetSpecialType(SpecialType.System_Boolean),
                    StringBuilder: compilation.GetTypeByMetadataName("System.Text.StringBuilder"),
                    ReadOnlySpan: compilation.GetTypeByMetadataName("System.ReadOnlySpan`1"),
                    IEnumerableT: compilation.GetTypeByMetadataName(
                        "System.Collections.Generic.IEnumerable`1"
                    )
                )
        );

        var queryStringModelImplementation = possibleQueryStringModelImplementation
            .Where(symbol => symbol is not null)
            .Combine(queryStringModel)
            .Where(
                (context) =>
                    context.Left!.AllInterfaces.Any(i =>
                        SymbolEqualityComparer.Default.Equals(i, context.Right)
                    )
            )
            .Select((context, _) => context.Left);

        var implementationWithTypes = queryStringModelImplementation
            .Combine(queryStringModel)
            .Combine(otherTypes)
            .Select(
                (context, _) =>
                    (
                        Source: context.Left.Left!,
                        IQueryStringModel: context.Left.Right,
                        Others: context.Right
                    )
            );

        var queryStringModelContext = implementationWithTypes.Select(
            (context, cancellationToken) =>
            {
                var typeName = context.Source.Name;
                var typeNamespace = context.Source.ContainingNamespace.ToDisplayString(
                    SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(
                        SymbolDisplayGlobalNamespaceStyle.Omitted
                    )
                );
                var fullName = context.Source.ToDisplayString(
                    SymbolDisplayFormat.FullyQualifiedFormat
                );
                var typeKind = (context.Source.IsValueType, context.Source.IsRecord) switch
                {
                    (true, true) => "record struct",
                    (true, false) => "struct",
                    (false, true) => "record",
                    (false, false) => "class"
                };

                var isOverride =
                    context.Source.BaseType?.Interfaces.Any(i =>
                        SymbolEqualityComparer.Default.Equals(i, context.IQueryStringModel)
                    ) ?? false;

                var members = context.Source.GetMembers();

                var appendQueryString = members.Any(m =>
                    m
                        is IMethodSymbol
                        {
                            Name: "AppendQueryString",
                            Parameters.Length: 2,
                            IsStatic: false,
                            IsAbstract: false,
                            IsPartialDefinition: true,
                            DeclaredAccessibility: Accessibility.Public,
                        } method
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[0].Type,
                        context.Others.StringBuilder
                    )
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[1].Type,
                        context.Others.Boolean
                    )
                    && method.Parameters[1].RefKind == RefKind.Ref
                );

                var parse = members.Any(m =>
                    m
                        is IMethodSymbol
                        {
                            Name: "Parse",
                            IsStatic: true,
                            IsAbstract: false,
                            Parameters.Length: 2,
                            IsPartialDefinition: true,
                            DeclaredAccessibility: Accessibility.Public,
                        } method
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[0].Type,
                        context.Others.String
                    )
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[1].Type,
                        context.Others.IFormatProvider
                    )
                    && SymbolEqualityComparer.Default.Equals(method.ReturnType, context.Source)
                );

                var parseSpan = members.Any(m =>
                    m
                        is IMethodSymbol
                        {
                            Name: "Parse",
                            IsStatic: true,
                            IsAbstract: false,
                            Parameters.Length: 2,
                            IsPartialDefinition: true,
                            DeclaredAccessibility: Accessibility.Public,
                        } method
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[0].Type.OriginalDefinition,
                        context.Others.ReadOnlySpan
                    )
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[1].Type,
                        context.Others.IFormatProvider
                    )
                    && SymbolEqualityComparer.Default.Equals(method.ReturnType, context.Source)
                );

                var tryParse = members.Any(m =>
                    m
                        is IMethodSymbol
                        {
                            Name: "TryParse",
                            IsStatic: true,
                            IsAbstract: false,
                            IsPartialDefinition: true,
                            Parameters.Length: 3,
                            DeclaredAccessibility: Accessibility.Public,
                        } method
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[0].Type,
                        context.Others.String
                    )
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[1].Type,
                        context.Others.IFormatProvider
                    )
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[2].Type,
                        context.Source
                    )
                    && method.Parameters[2].RefKind == RefKind.Out
                    && SymbolEqualityComparer.Default.Equals(
                        method.ReturnType,
                        context.Others.Boolean
                    )
                );

                var tryParseSpan = members.Any(m =>
                    m
                        is IMethodSymbol
                        {
                            Name: "TryParse",
                            IsStatic: true,
                            IsAbstract: false,
                            IsPartialDefinition: true,
                            Parameters.Length: 3,
                            DeclaredAccessibility: Accessibility.Public,
                        } method
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[0].Type.OriginalDefinition,
                        context.Others.ReadOnlySpan
                    )
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[1].Type,
                        context.Others.IFormatProvider
                    )
                    && SymbolEqualityComparer.Default.Equals(
                        method.Parameters[2].Type,
                        context.Source
                    )
                    && method.Parameters[2].RefKind == RefKind.Out
                    && SymbolEqualityComparer.Default.Equals(
                        method.ReturnType,
                        context.Others.Boolean
                    )
                );

                var (printMembers, printMembersParameterName) = context.Source.IsRecord
                    ? members
                        .Where(m =>
                            m
                                is IMethodSymbol
                                {
                                    Name: "PrintMembers",
                                    IsPartialDefinition: true,
                                    Parameters.Length: 1,
                                    DeclaredAccessibility: Accessibility.Protected,
                                } methodSymbol
                            && SymbolEqualityComparer.Default.Equals(
                                methodSymbol.Parameters[0].Type,
                                context.Others.StringBuilder
                            )
                            && SymbolEqualityComparer.Default.Equals(
                                methodSymbol.ReturnType,
                                context.Others.Boolean
                            )
                        )
                        .Select(m => (true, ((IMethodSymbol)m).Parameters[0].Name))
                        .FirstOrDefault()
                    : default;

                var source = new QueryStringModelType(
                    typeName,
                    typeNamespace,
                    fullName,
                    typeKind,
                    !context.Source.IsValueType && !context.Source.IsSealed,
                    isOverride,
                    new QueryStringModelMethods(
                        appendQueryString,
                        parse,
                        tryParse,
                        parseSpan,
                        tryParseSpan,
                        printMembers
                    ),
                    printMembersParameterName,
                    members
                        .Where(m =>
                            !m.GetAttributes()
                                .Any(a =>
                                    SymbolEqualityComparer.Default.Equals(
                                        a.AttributeClass,
                                        context.Others.QueryStringIgnoreAttribute
                                    )
                                )
                            && m
                                is IPropertySymbol { IsImplicitlyDeclared: false }
                                    or IFieldSymbol { IsImplicitlyDeclared: false }
                        )
                        .Select(m =>
                        {
                            var name = m.Name;
                            var type = m switch
                            {
                                IFieldSymbol field => field.Type,
                                IPropertySymbol property => property.Type,
                                _ => throw new System.NotSupportedException()
                            };
                            var isParsable = type.Interfaces.Any(i =>
                                SymbolEqualityComparer.Default.Equals(
                                    i.OriginalDefinition,
                                    context.Others.Parsable
                                )
                            );
                            var isSpanParsable = type.Interfaces.Any(i =>
                                SymbolEqualityComparer.Default.Equals(
                                    i.OriginalDefinition,
                                    context.Others.SpanParsable
                                )
                            );
                            var parameterName = m.GetAttributes()
                                .Where(a =>
                                    SymbolEqualityComparer.Default.Equals(
                                        a.AttributeClass,
                                        context.Others.QueryStringParameterAttribute
                                    )
                                )
                                .Select(a => (string)a.ConstructorArguments[0].Value!)
                                .DefaultIfEmpty(name)
                                .Single();
                            var isCollection =
                                !SymbolEqualityComparer.Default.Equals(type, context.Others.String)
                                && type.AllInterfaces.Any(i =>
                                    SymbolEqualityComparer.Default.Equals(
                                        i.OriginalDefinition,
                                        context.Others.IEnumerableT
                                    )
                                    && i.TypeArguments[0]
                                        .Interfaces.Any(i =>
                                            SymbolEqualityComparer.Default.Equals(
                                                i.OriginalDefinition,
                                                context.Others.SpanParsable
                                            )
                                            || SymbolEqualityComparer.Default.Equals(
                                                i.OriginalDefinition,
                                                context.Others.Parsable
                                            )
                                        )
                                );
                            var isCommaSeparatedCollection = m.GetAttributes()
                                .Any(a =>
                                    SymbolEqualityComparer.Default.Equals(
                                        a.AttributeClass,
                                        context.Others.QueryStringCommaSeparatedCollectionAttribute
                                    )
                                );
                            return new QueryStringModelParameter(
                                name,
                                type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                                isParsable,
                                isSpanParsable,
                                parameterName,
                                isCollection,
                                isCommaSeparatedCollection
                            );
                        })
                        .ToImmutableArray()
                );

                //throw new System.Exception(source.ToString());

                return source;
            }
        );

        context.RegisterSourceOutput(
            queryStringModelContext,
            GenerateQueryStringModelImplementation
        );
    }

    private void GenerateQueryStringModelImplementation(
        SourceProductionContext context,
        QueryStringModelType? source
    )
    {
        if (source is null)
        {
            return;
        }

        var hintName = $"{source.Namespace}.{source.TypeName}.g.cs";

        var text = new StringBuilder();
        var indentation = 0;

        // begin namespace
        text.AppendLine("// <auto-generated/>");
        text.AppendLine("#nullable enable");
        text.Append("namespace ").Append(source.Namespace).AppendLine();
        text.AppendLine("{");
        indentation += 4;

        // begin type
        AppendIndentation(text, indentation)
            .Append("partial ")
            .Append(source.ClassOrStruct)
            .Append(" ")
            .AppendLine(source.TypeName);
        AppendIndentation(text, indentation).AppendLine("{");
        indentation += 4;

        if (source.GenerateMembers.AppendQueryString)
        {
            indentation = ImplementAppendQueryString(source, text, indentation);
        }

        if (source.GenerateMembers.RecordPrintMembers)
        {
            indentation = ImplementPrintMembers(source, text, indentation);
        }

        // end type
        indentation -= 4;
        AppendIndentation(text, indentation).AppendLine("}");

        // end namespace
        indentation -= 4;
        AppendIndentation(text, indentation).AppendLine("}");

        context.AddSource(hintName, text.ToString());
    }

    private static StringBuilder AppendIndentation(StringBuilder sb, int indentation) =>
        sb.Append(' ', indentation);

    private int ImplementPrintMembers(
        QueryStringModelType source,
        StringBuilder text,
        int indentation
    )
    {
        AppendIndentation(text, indentation)
            .AppendLine(
                "[global::System.CodeDom.Compiler.GeneratedCode(\"PSSharp.Extensions.Uri\", \"1.0\")]"
            );

        var accessibility = (source.IsVirtual, source.IsOverride) switch
        {
            (true, true) => "protected override ",
            (false, true) => "protected override ",
            (false, false) => "private ",
            (true, false) => "protected virtual ",
        };

        AppendIndentation(text, indentation)
            .Append(accessibility)
            .Append("partial bool PrintMembers(global::System.Text.StringBuilder ")
            .Append(source.PrintMembersParameterName)
            .AppendLine(")");
        AppendIndentation(text, indentation).AppendLine("{");
        indentation += 4;

        AppendIndentation(text, indentation).AppendLine("var wrote = false;");

        foreach (var member in source.Members)
        {
            AppendIndentation(text, indentation)
                .Append("if (this.")
                .Append(member.MemberName)
                .AppendLine(" is not null)");
            AppendIndentation(text, indentation).AppendLine("{");
            indentation += 4;

            AppendIndentation(text, indentation)
                .Append("if (wrote) { ")
                .Append(source.PrintMembersParameterName)
                .AppendLine(".Append(\", \"); }");

            AppendIndentation(text, indentation).AppendLine("wrote = true;");
            AppendIndentation(text, indentation)
                .Append(source.PrintMembersParameterName)
                .Append(".Append(\"")
                .Append(member.MemberName)
                .Append(" = \").Append(this.")
                .Append(member.MemberName)
                .AppendLine(");");

            indentation -= 4;
            AppendIndentation(text, indentation).AppendLine("}");
        }

        AppendIndentation(text, indentation).AppendLine("return wrote;");

        indentation -= 4;
        AppendIndentation(text, indentation).AppendLine("}");

        return indentation;
    }

    private static int ImplementAppendQueryString(
        QueryStringModelType source,
        StringBuilder text,
        int indentation
    )
    {
        AppendIndentation(text, indentation)
            .AppendLine(
                "[global::System.CodeDom.Compiler.GeneratedCode(\"PSSharp.Extensions.Uri\", \"1.0\")]"
            );
        AppendIndentation(text, indentation)
            .AppendLine(
                "public partial void AppendQueryString(global::System.Text.StringBuilder query, ref bool hasQueryParams)"
            );
        AppendIndentation(text, indentation).AppendLine("{");
        indentation += 4;

        AppendIndentation(text, indentation)
            .AppendLine(
                "if (query is null) { throw new global::System.ArgumentNullException(nameof(query)); }"
            );

        foreach (var member in source.Members)
        {
            AppendIndentation(text, indentation);
            text.Append("if (this.").Append(member.MemberName).Append(" is not null");
            if (member.IsCollection)
            {
                text.Append(" && global::System.Linq.Enumerable.Any(this.")
                    .Append(member.MemberName)
                    .Append(")");
            }
            text.AppendLine(")");
            AppendIndentation(text, indentation).AppendLine("{");
            indentation += 4;

            if (member.IsCollection && !member.IsCommaSeparatedCollection)
            {
                AppendIndentation(text, indentation)
                    .Append("foreach (var item in this.")
                    .Append(member.MemberName)
                    .AppendLine(")");
                AppendIndentation(text, indentation).AppendLine("{");
                indentation += 4;

                AppendIndentation(text, indentation)
                    .AppendLine("query.Append(hasQueryParams ? '&' : '?');");
                AppendIndentation(text, indentation).AppendLine("hasQueryParams = true;");
                AppendIndentation(text, indentation)
                    .Append("query.Append(\"")
                    .Append(member.QueryStringParameterName)
                    .AppendLine(
                        "\").Append('=').Append(global::System.Uri.EscapeDataString(item.ToString()));"
                    );

                indentation -= 4;
                AppendIndentation(text, indentation).AppendLine("}");
            }
            else if (member.IsCollection && member.IsCommaSeparatedCollection)
            {
                AppendIndentation(text, indentation)
                    .AppendLine("query.Append(hasQueryParams ? '&' : '?');");
                AppendIndentation(text, indentation).AppendLine("hasQueryParams = true;");
                AppendIndentation(text, indentation)
                    .Append("var joined = string.Join(',', this.")
                    .Append(member.MemberName)
                    .AppendLine(");");
                AppendIndentation(text, indentation)
                    .Append("query.Append(\"")
                    .Append(member.QueryStringParameterName)
                    .AppendLine(
                        "\").Append('=').Append(global::System.Uri.EscapeDataString(joined));"
                    );
            }
            else
            {
                AppendIndentation(text, indentation)
                    .AppendLine("query.Append(hasQueryParams ? '&' : '?');");
                AppendIndentation(text, indentation).AppendLine("hasQueryParams = true;");
                AppendIndentation(text, indentation)
                    .Append("query.Append(\"")
                    .Append(member.QueryStringParameterName)
                    .Append("\").Append(global::System.Uri.EscapeDataString(this.")
                    .Append(member.MemberName)
                    .AppendLine(".ToString()));");
            }

            indentation -= 4;
            AppendIndentation(text, indentation).AppendLine("}");
        }

        indentation -= 4;
        AppendIndentation(text, indentation).AppendLine("}");
        return indentation;
    }
}
