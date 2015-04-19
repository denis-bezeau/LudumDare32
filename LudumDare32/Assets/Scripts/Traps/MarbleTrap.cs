using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Type of trap that affects an entire room
/// </summary>
public class MarbleTrap : Trap
{
	public Room _parentRoom;

	public float speedModifier;

	

	public override void OnEnterTrap(PersonAI person)
	{
		person.GetComponent<PersonMotion>().m_speedModifier = speedModifier;
	}

	public override void OnExitTrap(PersonAI person)
	{
		person.GetComponent<PersonMotion>().m_speedModifier = 1.0f;
	}
	
}