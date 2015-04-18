using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class QuestManager
{
    public List<Quest> InactiveQuestList = new List<Quest>();
    public List<Quest> ActiveQuestList = new List<Quest>();
    public List<Quest> CompleteQuestList = new List<Quest>();

    bool ActiveQuestListNeedsUpdate = false;

    public void AddQuest(Quest newQuest)
    {
       // Debug.Log("QuestManager.AddQuest(" + newQuest.QuestID + ")");
        if (InactiveQuestList.Contains(newQuest) == false)
        {
            InactiveQuestList.Add(newQuest);
            ActiveQuestListNeedsUpdate = true;
        }
    }

    public void Update()
    {
        if (ActiveQuestList.Count == 0)
        {
            ActiveQuestListNeedsUpdate = true;
        }
        for (int i = 0; i < ActiveQuestList.Count; ++i)
        {
            ActiveQuestList[i].Update();
            if (ActiveQuestList[i].IsComplete)
            {
                ActiveQuestListNeedsUpdate = true;
            }
        }
        if (ActiveQuestListNeedsUpdate == true)
        {
            updateActiveQuestList();
            ActiveQuestListNeedsUpdate = false;
        }
    }

    private void updateActiveQuestList()
    {
       // Debug.Log("updateActiveQuestList!!!!!!!!!!!!!!!!!!!");

        List<Quest> MoveToCompletedQuests = new List<Quest>();
        List<Quest> MoveToActiveQuests = new List<Quest>();
        for (int i = 0; i < ActiveQuestList.Count; i++)
        {
            if (ActiveQuestList[i].IsComplete == true)
            {
                MoveToCompletedQuests.Add(ActiveQuestList[i]);
            }
        }

        for (int i = 0; i < InactiveQuestList.Count; i++)
        {
            int prereqsComplete = 0;
            int prereqsCount = InactiveQuestList[i].preReqIDs.Count;
            for (int j = 0; j < prereqsCount; j++)
            {
                for (int k = 0; k < CompleteQuestList.Count; k++)
                {
                    if (CompleteQuestList[k].QuestID == InactiveQuestList[i].preReqIDs[j])
                    {
                        prereqsComplete++;
                    }
                }
            }

            if (prereqsComplete >= prereqsCount)
            {
                MoveToActiveQuests.Add(InactiveQuestList[i]);
            }
        }

        for (int i = 0; i < MoveToCompletedQuests.Count; i++)
        {
            CompleteQuestList.Add(MoveToCompletedQuests[i]);
            ActiveQuestList.Remove(MoveToCompletedQuests[i]);
            MoveToCompletedQuests[i].SetActive( true);
        }
        MoveToCompletedQuests.Clear();

        for (int i = 0; i < MoveToActiveQuests.Count; i++)
        {
            MoveToActiveQuests[i].SetActive( true);
            ActiveQuestList.Add(MoveToActiveQuests[i]);
            InactiveQuestList.Remove(MoveToActiveQuests[i]);
        }
        MoveToActiveQuests.Clear();
    }
}
