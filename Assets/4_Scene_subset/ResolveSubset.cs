using UnityEngine;
using System.Collections;

/// <summary>
/// Example that invokes dependency resolution on just a subset of the scene.
/// </summary>
public class ResolveSubset : MonoBehaviour
{
    void Awake()
    {
        var dependencyResolver = new DependencyResolver();
        dependencyResolver.Resolve(this.gameObject);
    }
}
