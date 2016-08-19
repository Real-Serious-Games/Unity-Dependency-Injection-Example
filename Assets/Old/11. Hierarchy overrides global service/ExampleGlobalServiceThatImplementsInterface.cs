using UnityEngine;
using System.Collections;
using System;

[Service]
public class ExampleGlobalServiceThatImplementsInterface : MonoBehaviour, IExampleDependencyInterface
{
    public void SomeUsefulFunction()
    {
        // Do something useful ...
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
