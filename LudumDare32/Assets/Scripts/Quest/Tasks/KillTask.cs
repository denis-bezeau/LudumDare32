using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KillTask : Task
{

	public int enemyType;
	public int count = 0;
	public int goalCount = 10;
	private bool SendTaskCompleteEvent;

	public KillTask()
	{
		CTEventManager.AddListener<KillEnemyEvent>(OnKillEnemyEvent);
	}

	public void OnKillEnemyEvent(KillEnemyEvent eventData)
	{
		if (isActive == false)
		{
			return;
		}
		if (enemyType == eventData.enemyType)
		{
			count += eventData.count;
			if (count >= goalCount)
			{
				if (IsComplete == false)
				{
					IsComplete = true;
					SendTaskCompleteEvent = true;
				}
			}
		}
	}

	public override string GetTaskDescription()
	{
		return taskDescription + count + "/" + goalCount;
	}

	public override void Update()
	{
		if (SendTaskCompleteEvent == true)
		{
			SendTaskCompleteEvent = false;
		}
	}
}