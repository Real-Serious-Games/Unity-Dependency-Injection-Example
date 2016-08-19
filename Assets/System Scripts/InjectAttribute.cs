using System;

public enum InjectFrom
{
    Above
}

/// <summary>
/// Attribute that indentifies dependencies to be injected, aka injection points.
/// </summary>
public class InjectAttribute : Attribute
{
    /// <summary>
    /// Indentifies where the dependency can be injected from.
    /// </summary>
    private InjectFrom injectFrom;

    public InjectAttribute(InjectFrom injectFrom)
    {
        this.injectFrom = injectFrom;
    }
}

