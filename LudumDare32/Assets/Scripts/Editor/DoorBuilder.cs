
using UnityEngine;
using UnityEditor;

using System.Collections;

public class DoorBuilder : EditorWindow
{
	private static bool _isHorizontal = false;

	private static Room _room1 = null;
	private static Room _room2 = null;

	private string _doorName = "door";

	private static readonly Vector3 DEFAULT_DOOR_POS = new Vector3 (0f, 0f, -0.01f);

	private GameObject _doorPrefab;
	private bool _showNameHelp = false;
	private bool _showRoomHelp = false;
	private string _roomHelpString = string.Empty;
	
	[MenuItem("LudumDare32/Build Door")]
	public static void Init ()
	{
		// Get existing open window or if none, make a new one:		
		DoorBuilder window = ScriptableObject.CreateInstance<DoorBuilder> ();
		window.Show ();
	}
	
	void OnGUI ()
	{
		GUILayout.Label ("Create a new door", EditorStyles.boldLabel);
		
		_isHorizontal = EditorGUILayout.Toggle ("Is Horizontal: ", _isHorizontal);

		// TODO: we should have this auto increment or something
		_doorName = EditorGUILayout.TextField ("Door Name: ", _doorName);

		_room1 = (Room)EditorGUILayout.ObjectField (_room1, typeof(Room), true);
		_room2 = (Room)EditorGUILayout.ObjectField (_room2, typeof(Room), true);
		
		if (GUILayout.Button ("Build Door"))
		{
			// Do all error checking for form data here
			if (!NameIsUnique ())
			{
				_showNameHelp = true;
			} else if (_room1 == _room2)
			{
				_showRoomHelp = true;
				_roomHelpString = "Connected rooms cannot be the same";
			} else if (_room1 == null || _room2 == null)
			{
				_showRoomHelp = true;
				_roomHelpString = "Door must connect to two rooms";
			} else
			{
				// Everything is fine! Build!
				_showNameHelp = false;
				_showRoomHelp = false;
				BuildDoor ();
			}
		}

		// Error displays
		if (_showNameHelp)
		{
			EditorGUILayout.HelpBox ("Please enter a unique door name", MessageType.Error);
		}
		if (_showRoomHelp)
		{
			EditorGUILayout.HelpBox (_roomHelpString, MessageType.Error);
		}
	}

	private bool NameIsUnique ()
	{
		return (null == GameObject.Find (_doorName)) ? true : false;
	}

	private void BuildDoor ()
	{
		if (_isHorizontal)
		{
			_doorPrefab = Resources.Load ("Prefabs/HorizontalDoor") as GameObject;
		} else
		{
			_doorPrefab = Resources.Load ("Prefabs/VerticalDoor") as GameObject;
		}
		
		GameObject doorObj = GameObject.Find (_doorName);
		if (doorObj == null)
		{
			doorObj = GameObject.Instantiate (_doorPrefab);
		} else
		{
			Debug.LogError ("We already have door: " + doorObj.name);
			return;
		}

		doorObj.transform.localPosition = DEFAULT_DOOR_POS;
		doorObj.name = _doorName;

		Door theDoor = doorObj.GetComponent<Door> ();
		if (theDoor != null)
		{
			theDoor.SetConnectedRooms (_room1, _room2);
		} else
		{
			Debug.LogError ("Door prefab needs a door component.");
		}

		// Add the door to the rooms
		AddDoorToRoom (theDoor, _room1);
		AddDoorToRoom (theDoor, _room2);
	}

	private void AddDoorToRoom (Door d, Room r)
	{
		if (r.AddDoorToRoom (d))
		{
			EditorUtility.SetDirty (r);
		} else
		{
			Debug.LogError ("Failed to add door to " + r.name);
		}
	}
}
