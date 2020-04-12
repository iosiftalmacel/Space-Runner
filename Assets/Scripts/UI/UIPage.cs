using UnityEngine;
using System.Collections;

public abstract class UIPage : MonoBehaviour {
    public virtual void OnShow () {
        gameObject.SetActive(true);
    }

    public virtual void OnHide() {
        gameObject.SetActive(false);
	}
}
