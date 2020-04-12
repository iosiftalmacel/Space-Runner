using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerBase : MonoBehaviour {
    public delegate void OnHit(Collider other);
    public OnHit OnPlayerHit;

    public Animator anim;
    public Transform effectsParent;
    public List<ItemBase> ownedItems;

    void Start()
    {
        ownedItems = new List<ItemBase>();
    }

    void Update()
    {
        for (int i = ownedItems.Count - 1; i >= 0; i--)
        {
            ItemBase item = ownedItems[i];

            if (item.State == ItemBase.ItemState.Using)
                item.OnUsing();
            else if(item.State == ItemBase.ItemState.Consumed && item.CheckIfFinished())
            {
                item.ClearEffects();
                ownedItems.Remove(item);
                Destroy(item);
            }
            else
                item.UpdateEffects();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            OnPlayerHit.Invoke(other);
        else if (other.CompareTag("Pickup"))
            OnItemPickup(other.GetComponent<ItemPickup>().CollectItem());
    }

    public bool OnDamageTaken()
    {
        CameraManager.Instance.Shake(0.3f, 0.01f, 0.2f, true);
        if (DeflectDamage())
        {
            anim.Play("damaged");
            return false;
        }
        else
        {
            anim.Play("destroyed");
            return true;
        }

    }

    public bool DeflectDamage()
    {
        if (IsUsingItem<DashItem>())
            return true;
        if (ActivateItem<ShieldItem>())
            return true;
      

        return false;
    }

    public bool ActivateItem<T>()
    {
        ItemBase item = GetItem<T>();

        if (item == null || item.State == ItemBase.ItemState.Using || item.State == ItemBase.ItemState.Consumed)
            return false;

        item.OnActivated();
        return true;
    }

    public bool ActivateItem(Type type)
    {
        ItemBase item = GetItem(type);

        if (item == null || item.State == ItemBase.ItemState.Using || item.State == ItemBase.ItemState.Consumed)
            return false;

        item.OnActivated();
        return true;
    }

    public ItemBase GetItem<T>()
    {       
        return GetItem(typeof(T));
    }

    public ItemBase GetItem(Type type)
    {
        foreach (var item in ownedItems)
        {
            if (item.GetType() == type)
            {
                return item;
            }
        }
        return null;
    }

    public bool IsUsingItem<T>()
    {
        ItemBase item = GetItem<T>();

        if (item != null && item.State == ItemBase.ItemState.Using)
            return true;
        else
            return false;
    }

    void OnItemPickup(ItemBase item)
    {
        if (item == null)
            return;

        ItemBase ownedItem = GetItem(item.GetType());
        if(ownedItem != null && ownedItem.canBeMerged )
        {
            ownedItem.OnCollected(this);
        }
        else
        {
            ItemBase newItem = Instantiate(item);
            newItem.OnCollected(this);
            ownedItems.Add(newItem);
        }
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 100, 150, 100), "I am a button"))
    //        ActivateItem<DashItem>();

    //}
}
