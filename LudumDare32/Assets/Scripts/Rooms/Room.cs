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

	private List<GameTile>
		_tiles;
	public List<GameTile> GameTiles {
		get { return _tiles; }
		set { _tiles = value; }
	}

	[SerializeField]
	protected List<InteractiveObject>
		_interactiveObjects = new List<InteractiveObject>();

	//public List<InteractiveObject> InteractiveObjects { get { return _interactiveObjects; } }

	protected List<Trap> _traps = new List<Trap>();

	// OLD
	protected List<PersonAI> _people = new List<PersonAI>();
	public List<PersonAI> PeopleInRoom {
		get { return _people;}
	}

	protected List<MentalStateControl> _persons = new List<MentalStateControl>();
	public List<MentalStateControl> PersonsInRoom {
		get { return _persons;}
	}

	public List<Door> Doors { get { return _doors; } }
	public BoxCollider RoomCollider = null;

	public bool HasObjects {
		get{ return (_interactiveObjects.Count > 0);}
	}

	#region Unity/Monobehaviour Functions
	protected void Awake ()
	{
		// Error checking first!
		RoomCollider = GetComponent<BoxCollider>();
		if (RoomCollider == null)
		{
			Debug.LogError("Room doesn't have a collider!");
		}
		
		if (_doors.Count < 1)
		{
			Debug.LogWarning(this.name + " doesn't have any doors!");
		}
		else
		{
			RemoveBadDoors();
		}

		RemoveBadTiles();
	}

	private void OnDrawGizmos ()
	{
		Gizmos.color = Color.yellow;

		if (UnityEditor.Selection.activeObject == this.gameObject)
		{
			Gizmos.color = Color.blue;
		}
		else
		{
			Gizmos.color = Color.green;
		}
	
		foreach (Door d in _doors)
		{
			Gizmos.DrawLine(GetRoomCenter(), d.transform.position);
		}

		Gizmos.DrawCube(GetRoomCenter(), new Vector3(0.5f, 0.5f, 0.5f));
	}

	private void OnDrawGizmosSelected ()
	{
		foreach (Room r in GetConnectedRooms())
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(GetRoomCenter(), r.GetRoomCenter());
		}
	}
	#endregion

	#region Public Functions
	public Vector3 GetRoomCenter ()
	{
		if (RoomCollider != null)
		{
			return this.transform.position + RoomCollider.center;
		}

		Debug.LogError("Room doesn't have RoomCollider assigned");

		return Vector3.zero;
	}

	public List<PersonAI> GetPeople ()
	{
		return _people;
	}

	/// <summary>
	/// Retrieves a list of objects in this room that aren't in use.
	/// </summary>
	/// <returns>The available interactive objects.</returns>
	public List<InteractiveObject> GetAvailableInteractiveObjects ()
	{
		List<InteractiveObject> theObjects = new List<InteractiveObject>();

		if (_interactiveObjects.Count > 0)
		{
			foreach (InteractiveObject io in _interactiveObjects)
			{
				if (!io.IsBeingUsed)
				{
					theObjects.Add(io);
				}
			}
		}

		return theObjects;
	}

	public int RemoveBadDoors ()
	{
		List<Door> badDoors = new List<Door>();
		foreach (Door d in _doors)
		{
			if (d == null)
			{
				Debug.LogWarning("Missing Door! Clean up doors in " + this.name);
				
				// Can't modify the list while interating
				badDoors.Add(d);
			}
		}
		
		// Get rid of them
		foreach (Door bd in badDoors)
		{
			_doors.Remove(bd);
		}

		return badDoors.Count;
	}

	public int RemoveBadTiles ()
	{
		if (_tiles == null)
		{
			// TODO: Do we ever have a list of tiles? Do we need one?
			return 0;
		}
		List<GameTile> badTiles = new List<GameTile>();
		foreach (GameTile d in _tiles)
		{
			if (d == null)
			{
				Debug.LogWarning("Missing tile! Clean up tiles in " + this.name);
				
				// Can't modify the list while interating
				badTiles.Add(d);
			}
		}
		
		// Get rid of them
		foreach (GameTile bd in badTiles)
		{
			_tiles.Remove(bd);
		}
		
		return badTiles.Count;
	}

	public void OnTriggerEnter (Collider col)
	{
		// TODO: Original version of AI, remove when new one is reliable
		PersonAI personAI = col.gameObject.GetComponent<PersonAI>();
		if (personAI != null)
		{
			_people.Add(personAI);

			// Notify PersonAI they're touching this room
			personAI.EnterRoomPhysically(this);
		}

		MentalStateControl person = col.gameObject.GetComponent<MentalStateControl>();
		if (person != null)
		{
			_persons.Add(person);

			person.EnterRoom(this);
		}
	}

	public bool IsPersonInRoom (MentalStateControl personToFind)
	{
		foreach (MentalStateControl p in _persons)
		{
			if (p == personToFind)
			{
				return true;
			}
		}
		return false;
	}

	public void OnTriggerExit (Collider col)
	{
		// TODO: Original version of AI, remove when new one is reliable
		PersonAI personAI = col.gameObject.GetComponent<PersonAI>();
		if (personAI != null)
		{
			_people.Remove(personAI);

			// Notify PersonAI they're no longer touching this room
			personAI.LeaveRoomPhysically(this);
		}

		MentalStateControl person = col.gameObject.GetComponent<MentalStateControl>();
		if (person != null)
		{
			_persons.Remove(person);
			
			person.ExitRoom(this);
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
		_traps.Add(newTrap);
	}

	public bool CheckForDoorInRoom (Door d)
	{
		return _doors.Contains(d);
	}
	
	public bool RemoveTileFromRoom (GameTile tile)
	{
		return _tiles.Remove(tile);
	}
	
	public bool IsConnectedToRoom (Room otherRoom)
	{
		return GetConnectedRooms().Contains(otherRoom);
	}

	public bool AddObjectToRoom (InteractiveObject newObject)
	{
		if (!_interactiveObjects.Contains(newObject))
		{
			_interactiveObjects.Add(newObject);
			return true;
		}
		
		return false;
	}
	#endregion
	
	#region Editor Functions
#if UNITY_EDITOR

	/// <summary>
	/// Adds the door to room.
	/// </summary>
	/// <returns><c>true</c>, if door to room was added, <c>false</c> otherwise.</returns>
	/// <param name="newDoor">New door.</param>
	/// <remarks>SHOULD ONLY BE USED BY EDITOR TOOLS</remarks>
	public bool AddDoorToRoom (Door newDoor)
	{
		if (_doors.Contains(newDoor))
		{
			Debug.LogWarning("Trying to add existing door " + newDoor.name + " to room " + this.name);
			return false;
		}

		_doors.Add(newDoor);
		return true;
	}

	public void ClearRoomObjects ()
	{
		_interactiveObjects.Clear();
	}
#endif
	#endregion

	public List<Room> GetConnectedRooms ()
	{
		List<Room> connectedRooms = new List<Room>();
		foreach (Door d in _doors)
		{
			connectedRooms.Add(d.GetOtherRoom(this));
		}

		return connectedRooms;
	}

	#region Debug Functions
	public void PrintConnectedRooms ()
	{
		string roomsPrint = "Rooms connected to " + this.name + ": ";
		List<Room> connectedRooms = GetConnectedRooms();
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
		
		checkedRooms.Add(this);
		
		// Check all doors in this room
		foreach (Door d in _doors)
		{
			// Find an open one
			if (d.IsOpen)
			{
				// Don't search the next room if it's already being searched somewhere down the line
				Room nextRoom = d.GetOtherRoom(this);
				if (!checkedRooms.Contains(nextRoom))
				{
					// If we found an open path, head on back
					if (nextRoom.OpenPathToExitExists(ref checkedRooms))
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

	#endregion

}