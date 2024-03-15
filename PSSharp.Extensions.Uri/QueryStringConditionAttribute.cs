namespace PSSharp.Extensions.Uri;

using System;

#pragma warning disable IDE0060 // Remove unused parameter

/// <summary>
/// Configures when a member of a <see cref="IQueryStringModel"/> should be included in a query string
/// during serialization.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = true,
    AllowMultiple = false
)]
public sealed class QueryStringConditionAttribute : Attribute
{
    /// <summary>
    /// Instructs the <see cref="IQueryStringModel"/> source generator to conditionally
    /// serialize the member based on the value of the member or the specified constant case.
    /// </summary>
    /// <param name="condition"></param>

    public QueryStringConditionAttribute(QueryStringCondition condition) { }

    /// <summary>
    /// Instructs the <see cref="IQueryStringModel"/> source generator to use the specified predicate method to
    /// determine whether the member should be included in the query string. The method signature must be as
    /// follows, though the name may vary and is specified as the attribute's constructor argument:
    /// <code>
    /// <c>
    /// partial class MyModel : IQueryStringModel
    ///     [QueryStringCondition(nameof(ShouldSerialize))]
    ///     public int OddNumber { get; set; }
    ///
    ///     // Only allows the member to be serialized if it is an odd number.
    ///     internal static bool ShouldSerialize(
    ///         MyModel instance, // type of model
    ///         string memberName,
    ///         int member // type of member
    ///     ) => member % 2 == 1;
    ///
    ///     public partial void AppendQueryString(StringBuilder query, ref bool hasQueryParams);
    /// }
    /// </c>
    /// </code>
    /// </summary>
    /// <param name="predicate">The name of the method which determines if the member's value will be serialized.</param>
    public QueryStringConditionAttribute(string predicate) { }
}
