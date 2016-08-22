using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// An example of a dependency that implements an interface.
/// </summary>
public class ExampleDependencyWithInterface : MonoBehaviour, IExampleDependencyInterface
{
    // Use this for initialization
    void Start ()
    {	
	}
	
	// Update is called once per frame
	void Update ()
    {	
	}

    public void SomeUsefulFunction()
    {
        Debug.Log("This should really do something useful.");
    }
}
