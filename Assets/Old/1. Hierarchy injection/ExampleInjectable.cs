using UnityEngine;
using System.Collections;

/// <summary>
/// Example that has dependencies injected.
/// </summary>
public class ExampleInjectable : MonoBehaviour
{
    [Inject(InjectFrom.Above)]
    public ExampleDependency ExampleDependencyAsProperty { get; set; }

    [Inject(InjectFrom.Above)]
    public ExampleDependency ExampleDependencyAsField;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
