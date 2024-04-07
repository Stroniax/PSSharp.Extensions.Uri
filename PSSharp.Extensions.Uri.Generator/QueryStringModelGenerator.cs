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
                    SpanParsable: compilation.GetTypeByMetadataName("System.ISpanParsable`1"),
                    Parsable: compilation.GetTypeByMetadataName("System.IParsable`1"),
                    String: compilation.GetSpecialType(SpecialType.System_String),
                    IFormatProvider: compilation.GetTypeByMetadataName("System.IFormatProvider"),
                    Boolean: compilation.GetSpecialType(SpecialType.System_Boolean),
                    StringBuilder: compilation.GetTypeByMetadataName("System.Text.StringBuilder"),
                    ReadOnlySpan: compilation.GetTypeByMetadataName("System.ReadOnlySpan`1"),
                    IEnumerableT: compilation.GetTypeByMetadataName(
                        "System.Collections.Generic.IEnumerable`1"
                    ),
                    QueryStringConditionAttribute: compilation.GetTypeByMetadataName(
                        "PSSharp.Extensions.Uri.QueryStringConditionAttribute"
                    ),
                    QueryStringConditionEnum: compilation.GetTypeByMetadataName(
                        "PSSharp.Extensions.Uri.QueryStringCondition"
                    ),
                    QueryStringDeserializerAttribute: compilation.GetTypeByMetadataName(
                        "PSSharp.Extensions.Uri.QueryStringDeserializerAttribute"
                    ),
                    QueryStringSerializerAttribute: compilation.GetTypeByMetadataName(
                        "PSSharp.Extensions.Uri.QueryStringSerializerAttribute"
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
                        (
                            parse
                                ? ParsableSignatures.ParseStringIFormatProvider
                                : ParsableSignatures.None
                        )
                            | (
                                tryParse
                                    ? ParsableSignatures.TryParseStringIFormatProvider
                                    : ParsableSignatures.None
                            ),
                        (
                            parseSpan
                                ? ParsableSignatures.ParseStringIFormatProvider
                                : ParsableSignatures.None
                        )
                            | (
                                tryParseSpan
                                    ? ParsableSignatures.ParseStringIFormatProvider
                                    : ParsableSignatures.None
                            ),
                        printMembers
                    ),
                    printMembersParameterName,
                    members
                        .Where(m =>
                            !m.GetAttributes()
                                .Any(a =>
                                    SymbolEqualityComparer.Default.Equals(
                                        a.AttributeClass,
                                        context.Others.QueryStringConditionAttribute
                                    )
                                    && a.ConstructorArguments.Length == 1
                                    && SymbolEqualityComparer.Default.Equals(
                                        a.ConstructorArguments[0].Type,
                                        context.Others.QueryStringConditionEnum
                                    )
                                    && a.ConstructorArguments[0].Kind == TypedConstantKind.Enum
                                    && a.ConstructorArguments[0].Value is int value
                                    && value == 2 // QueryStringCondition.Never
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
                            var serializeMethodName = m.GetAttributes()
                                .Where(a =>
                                    SymbolEqualityComparer.Default.Equals(
                                        a.AttributeClass,
                                        context.Others.QueryStringSerializerAttribute
                                    )
                                )
                                .Select(a => (string)a.ConstructorArguments[0].Value!)
                                .FirstOrDefault();
                            var serializeMethod = members
                                .Where(mm =>
                                    mm.Name == serializeMethodName
                                    && mm is IMethodSymbol { Parameters.Length: 3 or 4 } method
                                    && IsAssignableTo(type, method.Parameters[^2].Type)
                                )
                                .FirstOrDefault();

                            static bool IsAssignableTo(
                                ITypeSymbol type,
                                ITypeSymbol assignableTo
                            ) =>
                                SymbolEqualityComparer.Default.Equals(type, assignableTo)
                                || type.AllInterfaces.Any(i =>
                                    SymbolEqualityComparer.Default.Equals(i, assignableTo)
                                )
                                || (
                                    assignableTo.BaseType is not null
                                    && IsAssignableTo(type, assignableTo.BaseType)
                                );

                            var deserializeMethodName = m.GetAttributes()
                                .Where(a =>
                                    SymbolEqualityComparer.Default.Equals(
                                        a.AttributeClass,
                                        context.Others.QueryStringDeserializerAttribute
                                    )
                                )
                                .Select(a => (string)a.ConstructorArguments[0].Value!)
                                .FirstOrDefault();

                            var deserializeMethod = members
                                .Where(mm =>
                                    mm.Name == deserializeMethodName
                                    && mm is IMethodSymbol { Parameters.Length: 3 or 4 } method
                                    && IsAssignableTo(method.Parameters[^1].Type, type)
                                )
                                .FirstOrDefault();

                            var conditionAttribute = m.GetAttributes()
                                .Where(a =>
                                    SymbolEqualityComparer.Default.Equals(
                                        a.AttributeClass,
                                        context.Others.QueryStringConditionAttribute
                                    )
                                )
                                .FirstOrDefault();

                            var conditionMethodName = conditionAttribute is not null
                                ? conditionAttribute.ConstructorArguments[0].Value as string
                                : null;

                            var conditionMethod = members
                                .Where(mm =>
                                    mm.Name == conditionMethodName
                                    && mm is IMethodSymbol { Parameters.Length: 3 } method
                                    && SymbolEqualityComparer.Default.Equals(
                                        method.Parameters[1].Type,
                                        context.Others.String
                                    )
                                    && IsAssignableTo(type, method.Parameters[2].Type)
                                )
                                .Cast<IMethodSymbol>()
                                .FirstOrDefault();

                            var conditionTakesSelf = SymbolEqualityComparer.Default.Equals(
                                conditionMethod?.Parameters[0].Type,
                                context.Source
                            );

                            var conditionTakesMemberName = SymbolEqualityComparer.Default.Equals(
                                conditionMethod?.Parameters[conditionTakesSelf ? 1 : 0].Type,
                                context.Others.String
                            );

                            var conditionTakesValue = SymbolEqualityComparer.Default.Equals(
                                conditionMethod
                                    ?.Parameters[
                                        conditionTakesSelf && conditionTakesMemberName
                                            ? 2
                                            : (conditionTakesSelf || conditionTakesMemberName)
                                                ? 1
                                                : 0
                                    ]
                                    .Type,
                                type
                            );

                            var conditionAlways =
                                conditionAttribute is not null
                                && conditionAttribute.ConstructorArguments[0].Value is int value
                                && value == 1; // QueryStringCondition.Always

                            var conditionNotDefault =
                                conditionAttribute is not null
                                && conditionAttribute.ConstructorArguments[0].Value is int value1
                                && value1 == 0; // QueryStringCondition.WhenNotDefault

                            return new QueryStringModelParameter(
                                name,
                                type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                                isParsable,
                                isSpanParsable,
                                parameterName,
                                isCollection,
                                serializeMethodName is null
                                    ? new SerializerImplementation.EncodedString()
                                    : new SerializerImplementation.Method(
                                        serializeMethodName,
                                        serializeMethod?.IsStatic ?? false,
                                        true,
                                        true,
                                        true
                                    ),
                                deserializeMethodName is null
                                    ? isSpanParsable
                                        ? new DeserializerImplementation.SpanParse()
                                        : isParsable
                                            ? new DeserializerImplementation.Parse()
                                            : new DeserializerImplementation.Method(
                                                deserializeMethodName,
                                                true,
                                                true
                                            )
                                    : null,
                                conditionMethodName is null
                                    ? conditionAlways
                                        ? new ConditionImplementation.Always()
                                        : new ConditionImplementation.WhenNotDefault()
                                    : new ConditionImplementation.Method(
                                        conditionMethodName,
                                        conditionMethod?.IsStatic ?? false,
                                        conditionTakesSelf,
                                        conditionTakesMemberName,
                                        conditionTakesValue
                                    )
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
            // Condition
            if (member.Condition is ConditionImplementation.Always)
            {
                AppendIndentation(text, indentation);
                text.Append("//")
                    .Append(member.MemberName)
                    .AppendLine(" : QueryStringCondition.Always");
            }
            else if (member.Condition is ConditionImplementation.Never)
            {
                continue;
            }
            else if (member.Condition is ConditionImplementation.Method conditionMethod)
            {
                AppendIndentation(text, indentation);
                text.Append("if (");
                if (!conditionMethod.IsMethodStatic)
                {
                    text.Append("this.");
                }
                text.Append(conditionMethod.MethodName).Append("(");

                if (conditionMethod.HasSelfParameter)
                {
                    text.Append("this, ");
                }
                if (conditionMethod.HasMemberNameParameter)
                {
                    text.Append('"').Append(member.MemberName).Append("\", ");
                }
                if (conditionMethod.HasMemberValueParameter)
                {
                    text.Append("this.").Append(member.MemberName);
                }
                text.AppendLine("))");
            }
            else // if (member.QueryConditionWhenNotDefault)
            {
                AppendIndentation(text, indentation);
                text.Append("if (this.")
                    .Append(member.MemberName)
                    .Append(" is not null")
                    .AppendLine(")");
            }
            AppendIndentation(text, indentation).AppendLine("{");
            indentation += 4;

            // Value
            if (member.Serialize is SerializerImplementation.Method serializeMethod)
            {
                AppendIndentation(text, indentation);
                if (!serializeMethod.IsMethodStatic)
                {
                    text.Append("this.");
                }
                text.Append(serializeMethod.MethodName)
                    .Append("(query, \"")
                    .Append(member.MemberName)
                    .Append("\", this.")
                    .Append(member.MemberName)
                    .AppendLine(", ref hasQueryParams);");
            }
            else if (member.IsCollection)
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
