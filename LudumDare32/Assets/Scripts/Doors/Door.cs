using UnityEngine;
using System;

public class Door : MonoBehaviour
{
	[SerializeField]
	private bool
		_isOpen = true;
	public bool IsOpen {
		get { return _isOpen;}
	}

	// Doors connect 2 rooms
	[SerializeField]
	private Room
		_room1;
	[SerializeField]
	private Room
		_room2;

	// Entrance/exit doors
	public bool isEntrance = false;
	public bool isExit = false;

	private BoxCollider _doorCollider;

	// Closed door has health which enemies can deplete to open the door
	private int _health = 10;

	public void Hit ()
	{
		_health--;

		if (_health <= 0)
			_isOpen = true;
	}

	public void Awake ()
	{
		IsDoorInfoGood ();

		_doorCollider = GetComponent<BoxCollider> ();
		if (_doorCollider != null)
		{
			_doorCollider.enabled = !_isOpen;
		}
		else
		{
			Debug.LogError (this.name + " is missing a BoxCollider!");
		}
	}

	public Room GetOtherRoom (Room thisRoom)
	{
		if ((thisRoom == null) && isEntrance)
		{
			return _room1;
		}
		else if (thisRoom == _room1)
		{
			return _room2;
		}
		else if (thisRoom == _room2)
		{
			return _room1;
		}
		else
		{
			Debug.LogError ("Door isn't in requested room!");
		}
		return null;
	}

	public void ToggleDoorOpen ()
	{
		_isOpen = !_isOpen;
		_doorCollider.enabled = !_isOpen;
	}

	public void SetConnectedRooms (Room room1, Room room2)
	{
		_room1 = room1;
		_room2 = room2;
	}

	public bool IsDoorInfoGood ()
	{
		bool isGood = true;
		if (_room1 == null || _room2 == null)
		{
			Debug.LogError (this.name + " needs to attach two rooms: " + _room1 + " " + _room2);
			isGood = false;
		}

		if (_room1 != null && !_room1.CheckForDoorInRoom (this))
		{
			Debug.LogError (this.name + " is not in door list of " + _room1);
			isGood = false;
		}

		if (_room2 != null && !_room2.CheckForDoorInRoom (this))
		{
			Debug.LogError (this.name + " is not in door list of " + _room2);
			isGood = false;
		}

		GameTile[] tiles = GetComponentsInChildren<GameTile> ();
		if (tiles.Length > 1)
		{
			Debug.LogWarning ("Doors with two tiles are deprecated, please replace " + this.name);
			isGood = false;
		}
		
		return isGood;
	}

	public bool IsHorizontal {
		get {
			GameTile tile = GetComponentInChildren<GameTile> ();
			if (tile == null)
				return false;
			else
				return (tile.transform.localScale.x > tile.transform.localScale.y);
		}
	}
}


