using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Pickup/DashItem", fileName = "DashItem")]
public class DashItem : ItemBase {
    public float speed;
    public float accelerationWindow;
    public float deccelerationWindow;

    protected float speedAtStart;

    void OnEnable()
    {
        canBeMerged = true;
        autoUse = false;
    }
    public override void OnActivated()
    {
        base.OnActivated();
        speedAtStart = PlayerManager.Instance.CurrentSpeed;
        SpawnManager.Instance.OverrideExtraWindow = SpawnManager.Instance.ExtraWindow;
    }
    public override void OnUsing()
    {
        if(Time.time - activatedTime < accelerationWindow)
        {
            PlayerManager.Instance.OverrideSpeed = Mathf.Lerp(speedAtStart, speed, (Time.time - activatedTime) / accelerationWindow);
        }
        else if (Time.time - activatedTime > activeDuration - deccelerationWindow)
        {
            PlayerManager.Instance.OverrideSpeed = Mathf.Lerp(speed, speedAtStart, (Time.time - activatedTime - (activeDuration - deccelerationWindow)) / deccelerationWindow);
        }
        else
        {
            PlayerManager.Instance.OverrideSpeed = speed;
        }
        base.OnUsing();
    }

    public override void OnConsume()
    {
        base.OnConsume();
        SpawnManager.Instance.OverrideExtraWindow = 0;
        PlayerManager.Instance.OverrideSpeed = 0;
    }

}
