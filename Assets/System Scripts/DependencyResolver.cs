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
    /// Enumerate the scene and find objects of interest to the dependency resolver.
    /// 
    /// WARNING: This function can be expensive. Call it only once and cache the result if you need it.
    /// </summary>
    public void FindObjects(List<MonoBehaviour> injectables)
    {
        foreach (var gameObject in GameObject.FindObjectsOfType<GameObject>())
        {
            foreach (var component in gameObject.GetComponents<MonoBehaviour>())
            {
                var hasInjectableProperties = component
                    .GetType()
                    .GetProperties()
                    .Where(IsPropertyInjectable)
                    .Any();
                if (hasInjectableProperties)
                {
                    injectables.Add(component);
                }
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
            Debug.LogError(
                "Failed to resolve dependency for property. Property: " + injectableProperty.Name + ", MonoBehaviour: " + injectable.GetType().Name + ", GameObject: " + injectable.gameObject.name + "\r\n" +
                "Failed to find a dependency that matches " + injectableProperty.PropertyType.Name + ".", 
                injectable
            );
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
        var injectables = new List<MonoBehaviour>();
        FindObjects(injectables);

        foreach (var injectable in injectables)
        {
            ResolveDependencies(injectable);
        }
    }
}
