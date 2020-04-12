using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class VisibilityCheck : MonoBehaviour {
    public UnityEvent OnVisible;
    public UnityEvent OnInvisible;

    public void OnBecameVisible()
    {
#if UNITY_EDITOR
        if (Camera.current && Camera.current.name == "SceneCamera") return;
#endif
        OnVisible.Invoke();
    }

    public void OnBecameInvisible()
    {
#if UNITY_EDITOR
        if (Camera.current && Camera.current.name == "SceneCamera") return;
#endif
        if (gameObject.activeInHierarchy)
            OnInvisible.Invoke();
    }



}
