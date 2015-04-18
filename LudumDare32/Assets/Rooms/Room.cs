using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{


	private List<Door> _doors = new List<Door> ();
	private List<Trap> _traps = new List<Trap> ();

	// TODO: change to real people
	List<int> _people = new List<int> ();

	// TODO: Maybe decide if this room has no more room for traps
	public virtual bool CanHaveTraps {
		get{ return true;}
	}

	/// <summary>
	/// What to do when a new person enters the room
	/// </summary>
	public void OnEnterRoom ()
	{

	}

	/// <summary>
	/// What to do when a person exits a room
	/// </summary>
	public void OnExitRoom ()
	{

	}

}