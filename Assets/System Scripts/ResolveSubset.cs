using UnityEngine;
using System.Collections;

/// <summary>
/// Resolves dependencies only for the game object that it is attached to (and all its children).
/// </summary>
public class ResolveSubset : MonoBehaviour
{
    /// <summary>
    /// Resolve subset dependencies on awake.
    /// </summary>
    void Awake()
    {
        var dependencyResolver = new DependencyResolver();
        dependencyResolver.Resolve(this.gameObject);
    }
}
