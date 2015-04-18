using UnityEngine;
using System.Collections;

public abstract class Task
{
    public bool isActive = false;
    public System.Type TaskType;
    public bool IsComplete;
    public string taskDescription;
    public abstract string GetTaskDescription();
    public abstract void Update();
}