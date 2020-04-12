using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;
    public Transform pagesParent;

    protected Dictionary<string, UIPage> pages;
    protected UIPage activePage;
    protected UIPage prevPage;

    void Awake () {
        Instance = this;
        pages = new Dictionary<string, UIPage>();

        foreach (Transform item in pagesParent)
        {
            UIPage page = item.GetComponent<UIPage>();
            if (page != null)
                pages[item.name] = page;
        }
        GoToPage("Page_Start");
    }

    public void GoToPage(string pageName)
    {
        if (pages.ContainsKey(pageName))
        {
            if(activePage != null)
                activePage.OnHide();
            prevPage = activePage;
            activePage = pages[pageName];
            activePage.OnShow();
        }
    }
 
}
