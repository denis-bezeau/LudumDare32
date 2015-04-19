using UnityEngine;
using System.Collections;

public class PersonStats : MonoBehaviour
{
	private int hp = 30;

	public int HP
	{
		get { return hp; }
		set 
		{
			hp = value;

			if (hp <= 0)
			{
				CTEventManager.FireEvent(new KillEnemyEvent());
			}
		}
	
	}
}
