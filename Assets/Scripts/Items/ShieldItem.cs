using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Pickup/ShieldItem", fileName = "ShieldItem")]
public class ShieldItem : ItemBase {
    
    void OnEnable()
    {
        canBeMerged = true;
        autoUse = true;
    }
}
