using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIStaticPickup : MonoBehaviour {

    public Image icon;
    public Text count;
    protected ItemBase data;

    public void Setup(ItemBase item)
    {
        icon.sprite = item.uiSprite;
        count.transform.parent.gameObject.SetActive(item.ownedCount > 1);
        count.text = item.ownedCount.ToString();
        data = item;
    }
}
