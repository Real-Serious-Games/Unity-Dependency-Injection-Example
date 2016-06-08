using UnityEngine;
using System.Collections;

/// <summary>
/// Example that invokes dependency resolution on just a subset of the scene.
/// </summary>
public class ResolveSubset : MonoBehaviour
{
    public GameObject ToResolve;

    void Awake()
    {
        var dependencyResolver = new DependencyResolver();

        if (ToResolve != null)
        {
            dependencyResolver.Resolve(ToResolve);
        }
    }
}
