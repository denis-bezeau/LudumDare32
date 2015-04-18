using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
	List<Door> _doors = new List<Door> ();
	List<Trap> _traps = new List<Trap> ();

	// TODO: change to real people
	List<int> _people = new List<int> ();

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