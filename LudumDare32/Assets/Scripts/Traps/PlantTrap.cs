using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Type of trap that affects an entire room
/// </summary>
public class PlantTrap : Trap
{
	public int Damage;
	private TendrilTriggerOffset tendrilTrigger = null;

	public void Awake()
	{
		tendrilTrigger = GetComponent<TendrilTriggerOffset>();
		OnEnterTrap(null);
	}

	public override void OnEnterTrap(PersonAI person)
	{
		Debug.Log(name + "OnEnterTrap");
		base.OnEnterTrap(person);

		if (tendrilTrigger != null)
		{
			tendrilTrigger.isAttack = true;
		}

		if (_parentRoom)
		{
			List<PersonAI> people = _parentRoom.GetPeople();
			for (int i = 0; i < people.Count; ++i)
			{
				

				PersonStats stats = people[i].GetPlayerStats();
				stats.HP -= Damage;
				people[i].SetPlayerStats(stats);

				Debug.Log("people[" + i + "] doing damage, current hp = " + stats.HP);
			}
		}

	}
}