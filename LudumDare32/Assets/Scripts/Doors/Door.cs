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

	// Closed door has health which enemies can deplete to open the door
	private int _health = 10;

	public void Hit()
	{
		_health--;

		if(_health <= 0)
			_isOpen = true;
	}

	public void Awake ()
	{
		if (_room1 == null || _room2 == null) {
			Debug.LogError (this.name + ": Doors need to be attached to two rooms!");
		}
	}

	public Room GetOtherRoom (Room thisRoom)
	{
		if((thisRoom == null) && isEntrance)
		{
			return _room1;
		}
		else if (thisRoom == _room1) {
			return _room2;
		} else if (thisRoom == _room2) {
			return _room1;
		} else {
			Debug.LogError ("Door isn't in requested room!");
		}
		return null;
	}

	public void ToggleDoorOpen ()
	{
		_isOpen = !_isOpen;
	}
}


