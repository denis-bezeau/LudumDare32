using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A special room that allows spawning of people
/// </summary>
public class SpawnRoom : Room
{

	public override bool CanHaveTraps {
		get{ return false;}
	}

	new public void Awake ()
	{
		base.Awake ();

		Debug.Log ("Checking for available exit...");
		// Check for open path immediately
		List<Room> checkedRooms = new List<Room> ();
		this.OpenPathToExitExists (ref checkedRooms);
	}
}
