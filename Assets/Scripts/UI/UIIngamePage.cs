using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIIngamePage : UIPage {
    public Text progressText;
    public UIActivablePickup activablePrefab;
    public UIStaticPickup staticPrefab;

    public Transform activablesParent;
    public Transform staticParent;

    void Update () {
        progressText.text = "Score: " + PlayerManager.Instance.Progress.ToString("F0");
        UpdatePickups();

    }

    void UpdatePickups()
    {
        int activeCount = 0;
        int staticCount = 0;

        for (int i = 0; i < PlayerManager.Instance.player.ownedItems.Count; i++)
        {
            ItemBase item = PlayerManager.Instance.player.ownedItems[i];
            if (!item.autoUse)
            {
                bool isUsingLast = item.State == ItemBase.ItemState.Using && item.ownedCount == 1;
                
                if (!isUsingLast && item.ownedCount > 0)
                {
                    UIActivablePickup pick = GetActivableAt(activeCount);
                    pick.Setup(item);
                    pick.gameObject.SetActive(true);
                    activeCount++;
                }
            }
            else
            {
                UIStaticPickup pick = GetStaticAt(staticCount);
                pick.Setup(item);
                pick.gameObject.SetActive(true);
                staticCount++;
            }
        }

        for (int i = activeCount; i < activablesParent.childCount; i++)
        {
            activablesParent.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = staticCount; i < staticParent.childCount; i++)
        {
            staticParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    UIActivablePickup GetActivableAt( int index)
    {
        if(activablesParent.childCount > index)
        {
            return activablesParent.GetChild(index).GetComponent<UIActivablePickup>();
        }else
        {
            UIActivablePickup pick = Instantiate(activablePrefab);
            pick.transform.parent = activablesParent;
            pick.transform.localScale = Vector3.one;
            return pick;
        }
    }

    UIStaticPickup GetStaticAt(int index)
    {
        if (staticParent.childCount > index)
        {
            return staticParent.GetChild(index).GetComponent<UIStaticPickup>();
        }
        else
        {
            UIStaticPickup pick = Instantiate(staticPrefab);
            pick.transform.parent = staticParent;
            pick.transform.localScale = Vector3.one;
            return pick;
        }
    }
}
