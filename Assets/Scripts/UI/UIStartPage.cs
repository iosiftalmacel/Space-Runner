using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIStartPage : UIPage {

    public void OnStartClicked()
    {
        GameManager.Instance.StartGame();
    }

}
