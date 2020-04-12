using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRepositionClear : MonoBehaviour {
    TrailRenderer trail;

    void Start () {
        trail = GetComponent<TrailRenderer>();
        RepositionManager.OnReposition += OnReposition;
        trail.sortingOrder = 10;
    }

    void OnDestroy()
    {
        RepositionManager.OnReposition -= OnReposition;
    }

    void OnReposition(float value)
    {
        trail.Clear();
    }
	
}
