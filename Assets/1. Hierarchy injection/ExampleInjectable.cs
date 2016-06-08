using UnityEngine;
using System.Collections;

/// <summary>
/// Example that has a dependency injected.
/// </summary>
public class ExampleInjectable : MonoBehaviour
{
    [Inject]
    public ExampleDependency ExampleDependencyAsProperty { get; set; }

    [Inject]
    public ExampleDependency ExampleDependencyAsField;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
