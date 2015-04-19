using UnityEngine;
using System.Collections.Generic;
using System;

public class Room : MonoBehaviour
{

	/// <summary>
	/// The doors attached to this room. Should be kept in preferred escape order.
	/// </summary>
	[SerializeField]
	protected List<Door>
		_doors = new List<Door> ();

	protected List<Trap> _traps = new List<Trap> ();

	protected List<PersonAI> _people = new List<PersonAI> ();
	public List<PersonAI> PeopleInRoom {
		get { return _people;}
	}

	public List<Door> Doors { get{ return _doors; } }
	private BoxCollider _roomCollider = null;

	public void Awake ()
	{
		_roomCollider = GetComponent<BoxCollider> ();

		if (_roomCollider == null)
		{
			Debug.LogError ("Room doesn't have a collider!");
		}
		
		if (_doors.Count < 1)
		{
			Debug.LogWarning (this.name + " doesn't have any doors!");
		}
	}

	public void OnCollisionEnter (Collision col)
	{
		PersonAI person = col.gameObject.GetComponent<PersonAI> ();
		if (person != null)
		{
			Debug.Log ("Person entered room");
			_people.Add (person);
		}
	}

	public void OnCollisionExit (Collision col)
	{
		PersonAI person = col.gameObject.GetComponent<PersonAI> ();
		if (person != null)
		{
			Debug.Log ("Person left room");
			_people.Remove (person);
		}
	}

	// TODO: Maybe decide if this room has no more room for traps
	public virtual bool CanHaveTraps {
		get{ return true;}
	}

	public virtual bool CanHaveTrapType (Type trapType)
	{
		if (!CanHaveTraps)
			return false;

		// We don't want more than one room trap
		if (trapType is RoomTrap)
		{
			foreach (Trap t in _traps)
			{
				if (t is RoomTrap)
				{
					return false;
				}
			}
		}

		return true;
	}

	/// <summary>
	/// What to do when a new person enters the room
	/// </summary>
	public virtual void OnEnterRoom(PersonAI person)
	{

	}

	/// <summary>
	/// What to do when a person exits a room
	/// </summary>
	public virtual void OnExitRoom(PersonAI person)
	{

	}

	private void AddTrapToRoom (Trap newTrap)
	{
		_traps.Add (newTrap);
	}

	public List<Room> GetConnectedRooms ()
	{
		List<Room> connectedRooms = new List<Room> ();
		foreach (Door d in _doors)
		{
			connectedRooms.Add (d.GetOtherRoom (this));
		}

		return connectedRooms;
	}

	public void PrintConnectedRooms ()
	{
		string roomsPrint = "Rooms connected to " + this.name + ": ";
		List<Room> connectedRooms = GetConnectedRooms ();
		foreach (Room r in connectedRooms)
		{
			roomsPrint += "\n " + r.name;
		}
	}

	/// <summary>
	/// Check if we can get to the exit through open doors
	/// TODO: We probably don't want this here
	/// </summary>
	/// <returns><c>true</c>, if path to exit exists was opened, <c>false</c> otherwise.</returns>
	protected bool OpenPathToExitExists (ref List<Room> checkedRooms)
	{
		if (this is EscapeRoom)
		{
			Debug.Log ("Route ends at " + this.name);
			return true;
		}
		
		checkedRooms.Add (this);
		
		// Check all doors in this room
		foreach (Door d in _doors)
		{
			// Find an open one
			if (d.IsOpen)
			{
				// Don't search the next room if it's already being searched somewhere down the line
				Room nextRoom = d.GetOtherRoom (this);
				if (!checkedRooms.Contains (nextRoom))
				{
					// If we found an open path, head on back
					if (nextRoom.OpenPathToExitExists (ref checkedRooms))
					{
						Debug.Log ("Route includes " + this.name);
						return true;
					}
				}
			}
		}
		
		// We haven't found a path through here to the exit
		return false;
	}

	public List<PersonAI> GetPeople()
	{
		return _people;
	}

}