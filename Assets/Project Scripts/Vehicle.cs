using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour
{
    [Inject(InjectFrom.Anywhere)]
    public Pedestrian[] pedestrians;

    void Awake()
    {
        // Don't attempt to access 'pedestrians' here, it may not be resolved yet.
    }

    void Start()
    {
        // Can access 'pedestrians' now, it will have been 
        // resolved during the Awake phase of initialisation.
    }
}
