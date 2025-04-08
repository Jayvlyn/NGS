using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabBtn> tabButtons;

    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;

    public TabBtn selectedTab;
    public List<GameObject> objectsToSwap;

    public void TrackBtn(TabBtn btn)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabBtn>();
        }

        tabButtons.Add(btn);
    }

    public void OnTabEnter(TabBtn btn)
    {
        ResetTabs();
        if(selectedTab == null || btn != selectedTab)
        {
            btn.background.sprite = tabHover;
        }
    }

    public void OnTabExit(TabBtn btn)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabBtn btn)
    {
        if(selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = btn;
        selectedTab.Select();

        ResetTabs();
        btn.background.sprite = tabActive;

        int index = btn.transform.GetSiblingIndex();
        for(int i = 0; i < objectsToSwap.Count; i++)
        {
            if(i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach(TabBtn btn in tabButtons)
        {
            if (selectedTab != null && selectedTab == btn) continue;
            btn.background.sprite = tabIdle;
        }
    }
}
