using UnityEngine;
using System;

public class Door : MonoBehaviour
{

	private bool _isOpen = true;
	public bool IsOpen
	{
		get { return _isOpen;}
	}

	// Doors connect 2 rooms
	private Room _room1 = null;
	private Room _room2 = null;

	public void Awake ()
	{
		if (_room1 == null || _room2 == null)
		{
			Debug.LogError ("Doors need to be attached to two rooms!");
		}
	}

	public Room GetOtherRoom (Door thisRoom)
	{
		if (thisRoom == _room1)
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
	}
}


