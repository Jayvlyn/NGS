using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabBtn> tabButtons;

    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public Sprite sprite1;
    public Sprite sprite2;
    public bool twoTabs = false;
    public Image twotabSprite;
    bool tab1Selected = false;


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
            if (!twoTabs) btn.background.sprite = tabHover;
        }
    }

    public void OnTabExit(TabBtn btn)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabBtn btn)
    {
        if (twoTabs && btn != selectedTab)
        {
            twotabSprite.sprite = (tab1Selected) ? sprite1 : sprite2;
            tab1Selected = !tab1Selected;
        }

        if(selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = btn;
        selectedTab.Select();

        ResetTabs();
        if (!twoTabs)  btn.background.sprite = tabActive;

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
            if (!twoTabs) btn.background.sprite = tabIdle;
        }
    }

    public void moveTextUp(TMP_Text text)
    {
        text.transform.localPosition = new Vector3(text.transform.localPosition.x, text.transform.localPosition.y + 15, text.transform.localPosition.z);
    }
    public void moveTextDown(TMP_Text text)
    {
        text.transform.localPosition = new Vector3(text.transform.localPosition.x, text.transform.localPosition.y - 15, text.transform.localPosition.z);
    }

}
