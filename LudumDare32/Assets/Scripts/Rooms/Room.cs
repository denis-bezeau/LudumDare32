using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{

	[SerializeField]
	private List<Door>
		_doors = new List<Door> ();

	private List<Trap> _traps = new List<Trap> ();

	// TODO: change to real people
	List<int> _people = new List<int> ();


	public void Awake()
	{
		BoxCollider coll = GetComponent<BoxCollider>();
		if (coll != null)
		{
			// listen for collisions with room
		} else
		{
			Debug.LogError("Room doesn't have a collider!");
		}
		
		if (_doors.Count < 1)
		{
			Debug.LogWarning(this.name + " doesn't have any doors!");
		}
	}



	// TODO: Maybe decide if this room has no more room for traps
	public virtual bool CanHaveTraps {
		get{ return true;}
	}

	/// <summary>
	/// What to do when a new person enters the room
	/// </summary>
	public void OnEnterRoom()
	{

	}

	/// <summary>
	/// What to do when a person exits a room
	/// </summary>
	public void OnExitRoom()
	{

	}

	private void AddTrapToRoom(Trap newTrap)
	{
		_traps.Add(newTrap);
	}

	public List<Room> GetConnectedRooms()
	{
		List<Room> connectedRooms = new List<Room> ();
		foreach (Door d in _doors)
		{
			connectedRooms.Add(d.GetOtherRoom(this));
		}

		return connectedRooms;
	}

	public void PrintConnectedRooms()
	{
		string roomsPrint = "Rooms connected to " + this.name + ": ";
		List<Room> connectedRooms = GetConnectedRooms();
		foreach (Room r in connectedRooms)
		{
			roomsPrint += "\n " + r.name;
		}
	}

}