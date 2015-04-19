using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Type of trap that affects an entire room
/// </summary>
public class DoorTrap : Trap
{
	public Room _parentRoom;
	
	public override void OnEnterTrap(PersonAI person)
	{
		Debug.Log(name + "OnEnterTrap");
		base.OnEnterTrap(person);
	}
}