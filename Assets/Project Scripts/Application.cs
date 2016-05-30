using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

/// <summary>
/// Simple Unity script to boostrap our 'application'.
/// </summary>
public class Application : MonoBehaviour
{
    /// <summary>
    /// Resolve scene dependencies on awake.
    /// </summary>
    void Awake()
    {
        var dependencyResolver = new DependencyResolver();
        dependencyResolver.ResolveScene();
    }
}

