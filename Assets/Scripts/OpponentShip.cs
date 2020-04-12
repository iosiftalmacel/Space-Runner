using UnityEngine;
using System.Collections;

public class OpponentShip : OpponentBase {
    public override void OnDamageTaken(float time)
    {
        base.OnDamageTaken(time);
        CameraManager.Instance.Shake(0.12f, 0.01f, 0.2f, true);
    }
}
