using System;

public enum InjectFrom
{
    /// <summary>
    /// Inject from the hierarchy above the current game object.
    /// </summary>
    Above,

    /// <summary>
    /// Inject from anywhere in the scene.
    /// </summary>
    Anywhere
}

/// <summary>
/// Attribute that indentifies dependencies to be injected, aka injection points.
/// </summary>
public class InjectAttribute : Attribute
{
    /// <summary>
    /// Indentifies where the dependency can be injected from.
    /// </summary>
    public InjectFrom InjectFrom { get; private set; }

    public InjectAttribute(InjectFrom injectFrom)
    {
        this.InjectFrom = injectFrom;
    }
}

