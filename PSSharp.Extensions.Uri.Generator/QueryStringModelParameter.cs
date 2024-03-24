namespace PSSharp.Extensions.Uri;

public sealed record QueryStringModelParameter(
    string MemberName,
    string MemberTypeName,
    bool IsParsable,
    bool IsSpanParsable,
    string QueryStringParameterName,
    bool IsCollection,
    SerializerImplementation Serialize,
    DeserializerImplementation Deserialize,
    ConditionImplementation Condition
);
