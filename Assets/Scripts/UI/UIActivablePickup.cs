using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIActivablePickup : MonoBehaviour {
    public Image icon;
    public Text count;
    protected ItemBase data;

    public void Setup (ItemBase item) {
        icon.sprite = item.uiSprite;
        int owned = item.State == ItemBase.ItemState.Using ? item.ownedCount - 1 : item.ownedCount;
        count.transform.parent.gameObject.SetActive(owned > 1);
        count.text = owned.ToString();
        data = item;
    }
	

    public void OnClick()
    {
        PlayerManager.Instance.player.ActivateItem(data.GetType());
    }
}
