using UnityEngine;
using System.Collections;

public class GroundBase : MonoBehaviour {
    protected bool seen;

    public void OnEnable()
    {
        seen = false;
    }

    public void OnBecameVisible()
    {
        seen = true;
    }

    public void OnBecameInvisible()
    {
        if (seen)
        {
            GroundSpawner.Instance.FreeGroundInstance(this);
        }
    }
}
