using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour {
    public ItemBase itemData;
    public Animator anim;

    protected bool collected;
    protected bool seen;

    public void OnBecameVisible()
    {
        seen = true;
        collected = false;
    }

    public void OnBecameInvisible()
    {
        if (seen)
        {
            SpawnManager.Instance.FreePickup(this);
        }
    }

    void OnEnable()
    {
        anim.Play("enabled");
    }

    public ItemBase CollectItem()
    {
        if (collected)
            return null;
        collected = true;
        anim.Play("disable");
        return itemData;
    }

    public void OnCollected()
    {
        SpawnManager.Instance.FreePickup(this);
    }
}
