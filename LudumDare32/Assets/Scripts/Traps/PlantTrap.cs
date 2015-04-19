using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Type of trap that affects an entire room
/// </summary>
public class PlantTrap : Trap
{
	public Room _parentRoom;
	
	public int Damage;

	private TendrilTriggerOffset tendrilTrigger = null;

	public void Awake()
	{
		tendrilTrigger = GetComponent<TendrilTriggerOffset>();
	}

	public override void OnEnterTrap(PersonAI person)
	{
		Debug.Log(name + "OnEnterTrap");
		base.OnEnterTrap(person);

		if (tendrilTrigger != null)
		{
			tendrilTrigger.isAttack = true;
		}

		List<PersonAI> people = _parentRoom.GetPeople();
		for (int i = 0; i < people.Count; ++i)
		{
			Debug.Log("people[" + i + "] doing damage");

			PersonStats stats = people[i].GetPlayerStats();
			stats.HP -= Damage;
			people[i].SetPlayerStats(stats);
		}
	}
}