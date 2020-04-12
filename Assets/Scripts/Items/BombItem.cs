using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Pickup/BombItem", fileName = "BombItem")]
public class BombItem : ItemBase {
    void OnEnable()
    {
        canBeMerged = true;
        autoUse = false;
    }
    public override void OnActivated()
    {
        SpawnManager.Instance.DestroyAllOpponentsWithRandomDelay(0.1f, 0.25f);
        CameraManager.Instance.Shake(0.45f, 0.01f, 0.28f, true);
        base.OnActivated();
    }

    public override void OnUsing()
    {
        base.OnUsing();
        SpawnManager.Instance.ClearNotDyingOpponent();
    }

}
