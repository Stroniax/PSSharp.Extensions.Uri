namespace PSSharp.Extensions.Uri;

using System;

public abstract record ConditionImplementation
{
    private ConditionImplementation() { }

    public sealed record Always : ConditionImplementation;

    public sealed record WhenNotDefault : ConditionImplementation;

    public sealed record Never : ConditionImplementation;

    public sealed record Method(string MethodName) : ConditionImplementation;

    public void Switch(Action always, Action whenNotDefault, Action never, Action<Method> method)
    {
        switch (this)
        {
            case Always:
                always();
                break;
            case WhenNotDefault:
                whenNotDefault();
                break;
            case Never:
                never();
                break;
            case Method m:
                method(m);
                break;
            default:
                throw new NotSupportedException($"Unmatched case: {GetType().Name}.");
        }
    }
}
