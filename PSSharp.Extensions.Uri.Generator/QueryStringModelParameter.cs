namespace PSSharp.Extensions.Uri;

public sealed record QueryStringModelParameter(
    string MemberName,
    string MemberTypeName,
    bool IsParsable,
    bool IsSpanParsable,
    string QueryStringParameterName,
    bool IsCollection,
    string? SerializeMethod,
    bool IsSerializeMethodStatic,
    string? DeserializeMethod,
    bool DeserializeMethodTakesMemberName,
    string? ConditionMethod,
    bool IsConditionMethodStatic,
    bool ConditionMethodTakesSelf,
    bool ConditionMethodTakesMemberName,
    bool ConditionMethodTakesValue,
    bool QueryConditionAlways,
    bool QueryConditionNever,
    bool QueryConditionWhenNotDefault
);
