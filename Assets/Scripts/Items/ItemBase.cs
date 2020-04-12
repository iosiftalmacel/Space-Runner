using System;
using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    public enum ItemState
    {
        None,
        Collected,
        Activate,
        Using,
        Consumed
    }
    [Serializable]
    public class ItemEffect
    {
        public ItemState startTriggerState;
        public ItemState stopTriggerState;
        public float startDelay;
        public float stopDelay;

        public GameObject prefab;
        public Vector3 relativePosition;
        public bool parentPlayer;

        [HideInInspector]
        public GameObject effectInstance;
        [HideInInspector]
        public bool started;
        [HideInInspector]
        public bool stopped;
        [HideInInspector]
        public float startedTime;
        [HideInInspector]
        public float stoppedTime;

    }

    [HideInInspector]
    public bool autoUse;
    [HideInInspector]
    public bool canBeMerged;
    public int ownedCount { get; set; }
    public ItemState State { get { return itemState; } }


    public float activeDuration;
    public int spawnProbability;

    public ItemEffect[] effects;
    public Sprite uiSprite;

    protected float usingTime;
    protected float consumedTime;
    protected float collectedTime;
    protected float activatedTime;
    protected float lastStateTime;

    protected ItemState itemState;
    protected PlayerBase player;

    public virtual void OnCollected(PlayerBase currentPlayer)
    {
        player = currentPlayer;
        if (ownedCount <= 0)
        {
            itemState = ItemState.Collected;
            collectedTime = lastStateTime = Time.time;
        }    
        ownedCount++;
        UpdateEffects();
    }

    public virtual void OnActivated()
    {
        itemState = ItemState.Activate;
        activatedTime = lastStateTime = Time.time;
        UpdateEffects();

        if (activeDuration == 0)
            OnConsume();
        else
            OnUsing();
    }

    public virtual void OnUsing()
    {
        if (itemState != ItemState.Using)
            lastStateTime = usingTime = Time.time;

        itemState = ItemState.Using;
        UpdateEffects();

        if (Time.time - activatedTime > activeDuration)
            OnConsume();
    }

    public virtual void OnConsume()
    {
        ownedCount--;
        
        if(ownedCount <= 0)
        {
            itemState = ItemState.Consumed;
            Debug.LogError(GetType());
        }
        else
        {
            itemState = ItemState.None;
        }
        consumedTime = lastStateTime = Time.time;
        UpdateEffects();
    }

    public void UpdateEffects()
    {
        if (effects == null || effects.Length == 0)
            return;

        for (int i = 0; i < effects.Length; i++)
        {
            if (!effects[i].started)
            {
                if ((effects[i].startTriggerState == itemState && effects[i].startDelay == 0) ||
                    (effects[i].startedTime != 0 && Time.time - effects[i].startedTime > effects[i].startDelay))
                {
                    StartEffect(effects[i]);
                    effects[i].started = true;
                    effects[i].stopped = false;
                    effects[i].startedTime = 0;
                }

                if (effects[i].stopTriggerState == itemState && effects[i].startedTime == 0 && effects[i].startDelay != 0)
                {
                    effects[i].startedTime = Time.time;
                }

            }
            if (!effects[i].stopped)
            {
                if((effects[i].stopTriggerState == itemState && effects[i].stopDelay == 0) || 
                    (effects[i].stoppedTime != 0 && Time.time - effects[i].stoppedTime > effects[i].stopDelay))
                {
                    StopEffect(effects[i]);
                    effects[i].stopped = true;
                    effects[i].started = false;
                    effects[i].stoppedTime = 0;
                }

                if(effects[i].stopTriggerState == itemState && effects[i].stoppedTime == 0 && effects[i].stopDelay != 0)
                {
                    effects[i].stoppedTime = Time.time;
                }
            }
        }
    }

    internal void ClearEffects()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].effectInstance != null)
                Destroy(effects[i].effectInstance.gameObject);
        }
    }

    public bool CheckIfFinished()
    {
        if (effects == null || effects.Length <= 0)
            return true;

        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].started && !effects[i].stopped)
                return false;   
        }

        return true;
    }

    protected void StartEffect(ItemEffect effect)
    {
        Debug.LogError("startffect");

        effect.effectInstance = Instantiate(effect.prefab);
        if (effect.parentPlayer && player != null)
            effect.effectInstance.transform.parent = player.effectsParent;
        effect.effectInstance.transform.localPosition = effect.relativePosition;
    }

    protected void StopEffect(ItemEffect effect)
    {
        Debug.LogError("stopeffect");
        Destroy(effect.effectInstance);
    }
}
