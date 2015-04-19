
using UnityEngine;
using UnityEditor;

using System.Collections;

public class DoorBuilder : EditorWindow
{
	private static bool _isHorizontal = false;
	private static Vector2 _scrollPosition = Vector2.zero;
	private static Room _room1 = null;
	private static Room _room2 = null;
	private static string _doorName = "door";

	private static readonly Vector3 DEFAULT_DOOR_POS = new Vector3 (0f, 0f, -0.01f);
	private static readonly string kTileSetPath = "Textures/TileSets/";
	private static readonly string kTileSetMatPath = "Materials";

	private GameEnums.TileSet _tileSet = GameEnums.TileSet.TileSet1;
	private GameObject _doorPrefab;
	private bool _showNameHelp = false;
	private bool _showRoomHelp = false;
	private string _roomHelpString = string.Empty;
	private bool _isSure = false;
	
	[MenuItem("LudumDare32/Build Door")]
	public static void Init ()
	{
		// Get existing open window or if none, make a new one:		
		DoorBuilder window = ScriptableObject.CreateInstance<DoorBuilder> ();
		window.Show ();
	}

	void OnSelectionChange ()
	{
		// Make sure we update when selection is new
		Repaint ();
	}

	void OnGUI ()
	{
		GUILayout.Label ("Create a new door", EditorStyles.boldLabel);

		_scrollPosition = EditorGUILayout.BeginScrollView (_scrollPosition);
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
			}
			else if (_room1 == _room2)
			{
				_showRoomHelp = true;
				_roomHelpString = "Connected rooms cannot be the same";
			}
			else if (_room1 == null || _room2 == null)
			{
				_showRoomHelp = true;
				_roomHelpString = "Door must connect to two rooms";
			}
			else
			{
				// Everything is fine! Build!
				_showNameHelp = false;
				_showRoomHelp = false;
				BuildDoor ();
			}
		}

		if (GUILayout.Button ("Remove missing doors"))
		{
			RemoveMissingDoors ();
		}

		GameObject gO = Selection.activeGameObject;
		Door selectedDoor = null;
		if (gO != null)
		{
			selectedDoor = gO.GetComponent<Door> ();
		}

		// This section is for removing hidden walls behind new doors
		if (!_isSure && selectedDoor != null)
		{
			if (GUILayout.Button ("Delete hidden walls"))
			{
				_isSure = true;
			}
		}
		else if (selectedDoor != null)
		{
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Continue"))
			{
				RemoveHiddenWalls ();
				_isSure = false;
			}
			if (GUILayout.Button ("Reset"))
			{
				_isSure = false;
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.HelpBox ("Are you sure? Will remove underlying walls.", MessageType.Warning);
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

		EditorGUILayout.EndScrollView ();
	}

	private bool NameIsUnique ()
	{
		return (null == GameObject.Find (_doorName)) ? true : false;
	}

	private void BuildDoor ()
	{
		string rootTexPath = kTileSetPath + _tileSet.ToString () + "/" + kTileSetMatPath + "/";

		if (_isHorizontal)
		{
			_doorPrefab = Resources.Load ("Prefabs/HorizontalDoor") as GameObject;
		}
		else
		{
			_doorPrefab = Resources.Load ("Prefabs/VerticalDoor") as GameObject;
		}
		
		GameObject doorObj = GameObject.Find (_doorName);
		if (doorObj == null)
		{
			doorObj = GameObject.Instantiate (_doorPrefab);
		}
		else
		{
			Debug.LogError ("We already have door: " + doorObj.name);
			return;
		}

		doorObj.transform.localPosition = DEFAULT_DOOR_POS;
		doorObj.name = _doorName;

		// Set tile textures
		GameTile[] tiles = doorObj.GetComponentsInChildren<GameTile> ();
		foreach (GameTile gT in tiles)
		{
			// TODO: We are just using the floor for right now
			gT.SetTexture (rootTexPath + "Floor_01");
		}

		Door theDoor = doorObj.GetComponent<Door> ();
		if (theDoor != null)
		{
			theDoor.SetConnectedRooms (_room1, _room2);
		}
		else
		{
			Debug.LogError ("Door prefab needs a door component.");
		}

		// Add the door to the rooms
		AddDoorToRoom (theDoor, _room1);
		AddDoorToRoom (theDoor, _room2);
	}

	private void RemoveMissingDoors ()
	{
		Room[] sceneRooms = GameObject.FindObjectsOfType<Room> ();
		foreach (Room r in sceneRooms)
		{
			Debug.Log (r.name + " dad doors cleaned up: " + r.RemoveBadDoors ());
		}
	}

	private void RemoveHiddenWalls ()
	{
		Debug.Log ("TODO: Remove hidden walls...");

		Door selectedDoor = Selection.activeGameObject.GetComponent<Door> ();
		if (selectedDoor != null)
		{
			GameTile[] tiles = selectedDoor.GetComponentsInChildren<GameTile> ();
			foreach (GameTile gT in tiles)
			{
				RemoveHiddenTiles (gT);
			}
		}
	}

	private void RemoveHiddenTiles (GameTile tile)
	{
		RaycastHit hit;
		Ray detectBehind = new Ray (tile.transform.position, new Vector3 (0f, 0f, 1f));
		
		if (Physics.Raycast (detectBehind, out hit))
		{
			GameObject obj = hit.collider.gameObject;
			Debug.Log ("Removing tile: " + obj.name);
			Undo.DestroyObjectImmediate (obj);
		}
	}

	private void AddDoorToRoom (Door d, Room r)
	{
		if (r.AddDoorToRoom (d))
		{
			EditorUtility.SetDirty (r);
		}
		else
		{
			Debug.LogError ("Failed to add door to " + r.name);
		}
	}
}
