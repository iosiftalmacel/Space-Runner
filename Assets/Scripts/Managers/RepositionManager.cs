using UnityEngine;
using System.Collections;

public class RepositionManager : MonoBehaviour {
    public static RepositionManager Instance;
    public delegate void OnRepositionEvent(float value);

    public static event OnRepositionEvent OnReposition;

    void Awake () {
        Instance = this;
    }

    void Update()
    {
        if(PlayerManager.Instance.player.transform.position.z > 1000)
        {
            Reposition();
        }
    }

    public void Reposition()
    {
        float value = -PlayerManager.Instance.player.transform.position.z;
        OnReposition.Invoke(value);
    }
	
}
