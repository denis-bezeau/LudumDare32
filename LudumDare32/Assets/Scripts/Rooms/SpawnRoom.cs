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
	}

	public void CheckForExitPath ()
	{
		// Check for open path immediately
		List<Room> checkedRooms = new List<Room> ();
		if (!this.OpenPathToExitExists (ref checkedRooms))
		{
			Debug.LogError (this.name + " does not have a valid path to an EscapeRoom");
		}
	}
}
