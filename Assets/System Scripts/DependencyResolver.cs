using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

/// <summary>
/// Helper class that resolves dependencies in the Unity scene.
/// </summary>
public class DependencyResolver
{
    /// <summary>
    /// Enumerate root objects in the Unity scene.
    /// 
    /// WARNING: This function can be expensive. Call it only once and cache the result if you need it.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<GameObject> GetRootObjects()
    {
        return GameObject.FindObjectsOfType<GameObject>()
            .Cast<GameObject>()
            .Where(go => go.transform.parent == null);

        /* Todo: This code would do this except it causes the following error:
         * 
         * ArgumentException: The scene is not loaded.
         * 
         * http://forum.unity3d.com/threads/bug-getrootgameobjects-is-not-working-in-awake.379317/
         * https://issuetracker.unity3d.com/issues/scene-is-not-considered-loaded-when-awake-is-called
         * 

        for (var sceneIndex = 0; sceneIndex < SceneManager.sceneCount; ++sceneIndex)
        {
            var scene = SceneManager.GetSceneAt(sceneIndex);
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                yield return rootObject;
            }
        }
        */
    }

    /// <summary>
    /// Enumerate all Game Objects in the Unity hierarchy under 'parentObjects' that require dependency resolution.
    /// </summary>
    private IEnumerable<MonoBehaviour> FindInjectables(IEnumerable<GameObject> parentObjects)
    {
        foreach (var parentObject in parentObjects)
        {
            foreach (var injectable in parentObject.GetComponentsInChildren<IInjectable>())
            {
                yield return (MonoBehaviour)injectable;
            }
        }
    }

    /// <summary>
    /// Determine if a property requires dependency resolution, checks if the property has the 'Inject' attribute.
    /// </summary>
    private bool IsPropertyInjectable(PropertyInfo property)
    {
        return property.GetCustomAttributes(true)
            .Where(attribute => attribute is InjectAttribute)
            .Count() > 0;
    }

    /// <summary>
    /// Use C# reflection to find all properties of an object that require dependency resolution and injection.
    /// </summary>
    private IEnumerable<PropertyInfo> FindInjectableProperties(object injectable)
    {
        var type = injectable.GetType();
        return type.GetProperties().Where(IsPropertyInjectable);
    }

    /// <summary>
    /// Get the ancestors from a particular Game Object. This means the parent object, the grand parent and so on up to the root of the hierarchy.
    /// </summary>
    private IEnumerable<GameObject> GetAncestors(GameObject fromGameObject)
    {
        for (var parent = fromGameObject.transform.parent; parent != null; parent = parent.parent)
        {
            yield return parent.gameObject; // Mmmmm... LINQ.
        }
    }

    /// <summary>
    /// Walk up the hierarchy (towards the root) and find an injectable dependency that matches the specified type.
    /// </summary>
    private MonoBehaviour FindDependencyInHierarchy(Type injectionType, GameObject fromGameObject)
    {
        foreach (var ancestor in GetAncestors(fromGameObject))
        {
            foreach (var component in ancestor.GetComponents<MonoBehaviour>())
            {
                if (injectionType.IsAssignableFrom(component.GetType()))
                {
                    return component;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Attempt to resolve a propety dependency by scanning up the hiearchy for a MonoBehaviour that mathces the injection type.
    /// </summary>
    private bool ResolvePropertyDependencyFromHierarchy(MonoBehaviour injectable, PropertyInfo injectableProperty)
    {
        // Find a match in the hierarchy.
        var toInject = FindDependencyInHierarchy(injectableProperty.PropertyType, injectable.gameObject);
        if (toInject != null)
        {
            try
            {
                Debug.Log("Injecting " + toInject.GetType().Name + " into " + injectable.GetType().Name + " at property " + injectableProperty.Name + " on GameObject '" + injectable.name + "'.", injectable);

                injectableProperty.SetValue(injectable, toInject, null);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, injectable); // Bad type??!
            }

            return true;
        }
        else
        {
            // Failed to find a match.
            return false;
        }
    }

    /// <summary>
    /// Resolve a property dependency and inject the resolved valued.
    /// </summary>
    private void ResolvePropertyDependency(MonoBehaviour injectable, PropertyInfo injectableProperty)
    {
        if (!ResolvePropertyDependencyFromHierarchy(injectable, injectableProperty))
        {
            Debug.LogError("Failed to find an injectable that matches " + injectableProperty.PropertyType.Name, injectable);
        }
    }

    /// <summary>
    /// Resolve dependenies for an 'injectable' object.
    /// </summary>
    private void ResolveDependencies(MonoBehaviour injectable)
    {
        var injectableProperties = FindInjectableProperties(injectable);
        foreach (var injectableProperty in injectableProperties)
        {
            ResolvePropertyDependency(injectable, injectableProperty);
        }
    }

    /// <summary>
    /// Resolve all depenencies in the entire scene.
    /// </summary>
    public void ResolveScene()
    {
        var rootObjects = GetRootObjects().ToArray(); // Bake the LINQ enumerable to an array.

        foreach (var injectable in FindInjectables(rootObjects))
        {
            ResolveDependencies(injectable);
        }
    }
}
