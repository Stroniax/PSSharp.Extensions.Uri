namespace PSSharp.Extensions.Uri;

using System.Collections.Immutable;
using System.Text;

public sealed record QueryStringModelType(
    string TypeName,
    string Namespace,
    string FullyQualifiedName,
    string ClassOrStruct,
    bool IsVirtual,
    bool IsOverride,
    QueryStringModelMethods GenerateMembers,
    string? PrintMembersParameterName,
    ImmutableArray<QueryStringModelParameter> Members
)
{
    private bool PrintMembers(StringBuilder sb)
    {
        sb.Append("TypeName").Append(" = ").Append(TypeName).Append(", ");
        sb.Append("Namespace").Append(" = ").Append(Namespace).Append(", ");
        sb.Append("FullyQualifiedName").Append(" = ").Append(FullyQualifiedName).Append(", ");
        sb.Append("ClassOrStruct").Append(" = ").Append(ClassOrStruct).Append(", ");
        sb.Append("IsVirtual").Append(" = ").Append(IsVirtual).Append(", ");
        sb.Append("IsOverride").Append(" = ").Append(IsOverride).Append(", ");
        sb.Append("GenerateMembers").Append(" = ").Append(GenerateMembers).Append(", ");
        sb.Append("Members").Append(" = ").Append("[ ");
        foreach (var member in Members)
        {
            sb.Append(member).Append(", ");
        }
        sb.Append("]");

        return true;
    }
}
