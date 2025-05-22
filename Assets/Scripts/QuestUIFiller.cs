using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIFiller : MonoBehaviour
{
    [SerializeField] GameObject questPrefab;
    [SerializeField] GameObject questUI;
    [SerializeField] List<GameObject> currentQuestUIElements;
    [SerializeField] List<Quest> questToAdd;
    [SerializeField] List<Quest> questToRemove;
    [SerializeField] float randomRotationRange = 10f;
    [SerializeField] List<Color> stickyNoteColors;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.isActiveAndEnabled)
        {
            //when inventory is set active then add the UI elements 
            if (questToAdd.Count > 0 && this.isActiveAndEnabled)
            {
                foreach (Quest quest in questToAdd)
                {
                    addQuestToUI(quest);

                }
                questToAdd.Clear(); //clear after the list is added.
            }
            if (questToRemove.Count > 0 && this.isActiveAndEnabled)
            {
                foreach (Quest quest in questToRemove)
                {
                    removeQuestFromUI(quest);
                }
                questToRemove.Clear();
            }
        }
    }

    void addQuestToUI(Quest quest)
    { 
        GameObject newPrefab = Instantiate<GameObject>(questPrefab, this.transform);
        float randomRotation = Random.Range(-randomRotationRange, randomRotationRange);
        Image stickyNote = newPrefab.GetComponentsInChildren<Image>()[0];
        stickyNote.gameObject.transform.rotation = Quaternion.Euler(0, 0, randomRotation);
        stickyNote.color = stickyNoteColors[Random.Range(0, stickyNoteColors.Count)];
       // stickyNote.GetComponentsInChildren<TMP_Text>()[2].text = quest.name;
        stickyNote.GetComponentsInChildren<TMP_Text>()[1].text = quest.description;
        stickyNote.GetComponentsInChildren<TMP_Text>()[0].text = quest.reward.ToString();
    }
    void removeQuestFromUI(Quest quest)
    { 
    
    }
}
