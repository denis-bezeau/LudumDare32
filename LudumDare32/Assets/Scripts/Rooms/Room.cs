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
		_doors;

	[SerializeField]
	protected List<GameTile>
		_tiles;
	public List<GameTile> GameTiles {
		get { return _tiles; }
		set { _tiles = value; }
	}

	protected List<Trap> _traps = new List<Trap> ();

	protected List<PersonAI> _people = new List<PersonAI> ();
	public List<PersonAI> PeopleInRoom {
		get { return _people;}
	}

	public List<Door> Doors { get { return _doors; } }
	public BoxCollider RoomCollider = null;

	public void Awake ()
	{
		// Error checking first!
		RoomCollider = GetComponent<BoxCollider> ();
		if (RoomCollider == null)
		{
			Debug.LogError ("Room doesn't have a collider!");
		}
		
		if (_doors.Count < 1)
		{
			Debug.LogWarning (this.name + " doesn't have any doors!");
		}
		else
		{
			RemoveBadDoors ();
		}

		RemoveBadTiles ();
	}

	public int RemoveBadDoors ()
	{
		List<Door> badDoors = new List<Door> ();
		foreach (Door d in _doors)
		{
			if (d == null)
			{
				Debug.LogWarning ("Missing Door! Clean up doors in " + this.name);
				
				// Can't modify the list while interating
				badDoors.Add (d);
			}
		}
		
		// Get rid of them
		foreach (Door bd in badDoors)
		{
			_doors.Remove (bd);
		}

		return badDoors.Count;
	}

	public int RemoveBadTiles ()
	{
		List<GameTile> badTiles = new List<GameTile> ();
		foreach (GameTile d in _tiles)
		{
			if (d == null)
			{
				Debug.LogWarning ("Missing tile! Clean up tiles in " + this.name);
				
				// Can't modify the list while interating
				badTiles.Add (d);
			}
		}
		
		// Get rid of them
		foreach (GameTile bd in badTiles)
		{
			_tiles.Remove (bd);
		}
		
		return badTiles.Count;
	}

	public void OnTriggerEnter (Collider col)
	{
		PersonAI person = col.gameObject.GetComponent<PersonAI> ();
		if (person != null)
		{
			_people.Add (person);

			// Notify PersonAI they're touching this room
			person.EnterRoomPhysically (this);
		}
	}

	public void OnTriggerExit (Collider col)
	{
		PersonAI person = col.gameObject.GetComponent<PersonAI> ();
		if (person != null)
		{
			_people.Remove (person);

			// Notify PersonAI they're no longer touching this room
			person.LeaveRoomPhysically (this);
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
		if (trapType == typeof(RoomTrap))
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
	public virtual void OnEnterRoom (PersonAI person)
	{

	}

	/// <summary>
	/// What to do when a person exits a room
	/// </summary>
	public virtual void OnExitRoom (PersonAI person)
	{

	}

	public void AddTrapToRoom (Trap newTrap)
	{
		_traps.Add (newTrap);
	}

	/// <summary>
	/// Adds the door to room.
	/// </summary>
	/// <returns><c>true</c>, if door to room was added, <c>false</c> otherwise.</returns>
	/// <param name="newDoor">New door.</param>
	/// <remarks>SHOULD ONLY BE USED BY EDITOR TOOLS</remarks>
	public bool AddDoorToRoom (Door newDoor)
	{
		if (_doors.Contains (newDoor))
		{
			Debug.LogWarning ("Trying to add existing door " + newDoor.name + " to room " + this.name);
			return false;
		}

		_doors.Add (newDoor);
		return true;
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
			//Debug.Log ("Route ends at " + this.name);
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
						//Debug.Log ("Route includes " + this.name);
						return true;
					}
				}
			}
		}
		
		// We haven't found a path through here to the exit
		return false;
	}

	public List<PersonAI> GetPeople ()
	{
		return _people;
	}

	public bool CheckForDoorInRoom (Door d)
	{
		return _doors.Contains (d);
	}

	public bool RemoveTileFromRoom (GameTile tile)
	{
		return _tiles.Remove (tile);
	}

}