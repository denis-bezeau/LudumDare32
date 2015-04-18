using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Quest
{
    private bool IsActive = false;
    public List<int> preReqIDs = new List<int>();
    public int QuestID;
    public bool IsComplete;
    public System.Action OnComplete;

    public List<Task> TaskList = new List<Task>();
    public string QuestDescription;

    public void SetActive(bool newActive)
    {
        IsActive = newActive;
        for (int i = 0; i < TaskList.Count; ++i)
        {
            TaskList[i].isActive = newActive;
        }
    }

    public void Update()
    {
        for (int i = 0; i < TaskList.Count; ++i)
        {
            TaskList[i].Update();
        }
        UpdateTasksComplete();
    }

    public string GetQuestDescription()
    {
        string returnValue = "Quest:(" + QuestDescription + ")";
        for (int i = 0; i < TaskList.Count; ++i)
        {
            returnValue += ("\n" + TaskList[i].GetTaskDescription());
        }

        return returnValue;
    }

    public void UpdateTasksComplete()
    {
        if (IsActive == false)
        {
            return;
        }
        int taskCount = TaskList.Count;
        int currentTasksCompleteCount = 0;

        for (int i = 0; i < taskCount; ++i)
        {
            if (TaskList[i].IsComplete == true)
            {
                currentTasksCompleteCount++;
            }
        }

        if (currentTasksCompleteCount >= taskCount)
        {
            IsComplete = true;
            if (OnComplete != null)
            {
                OnComplete.Invoke();
            }
        }
    }
}