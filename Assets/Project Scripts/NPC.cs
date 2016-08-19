using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    [Inject(InjectFrom.Anywhere)]
    public Player player;

    void Awake()
    {
        // Don't attempt to access 'player' here, it may not be resolved yet.
    }

    void Start()
    {
        // Can access 'player' now, it will have been 
        // resolved during the Awake phase of initialisation.
    }
}
