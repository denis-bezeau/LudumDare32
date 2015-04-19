
using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

public class DoorBuilder : EditorWindow
{
	private static bool _isHorizontal = false;
	private static Vector2 _scrollPosition = Vector2.zero;
	private static Room _room1 = null;
	private static Room _room2 = null;
	private static string _doorName = "door";
	
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
			else if (_room1 == null || _room2 == null)
			{
				_showRoomHelp = true;
				_roomHelpString = "Door must connect to two rooms";
			}
			else if (_room1 == _room2)
			{
				_showRoomHelp = true;
				_roomHelpString = "Connected rooms cannot be the same";
			}
			else
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

		GUILayout.Label ("Door editing tools", EditorStyles.boldLabel);

		if (GUILayout.Button ("Remove missing doors & tiles from rooms"))
		{
			RemoveMissingDoorsAndTiles ();
		}

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Identify bad doors"))
		{
			IdentifyErrorDoors ();
		}
		if (GUILayout.Button ("Remove bad doors"))
		{
			RemoveErrorDoors ();
		}
		EditorGUILayout.EndHorizontal ();

		//Button to turn off all Mesh Renderers on All Doors
		if (GUILayout.Button ("Toggle Tile Mesh Renders"))
		{
			ToggleDoorsMeshRenderer ();
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

		doorObj.transform.localPosition = Vector3.zero;
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

		// And finally, select
		Selection.activeGameObject = doorObj;
	}

	private void RemoveMissingDoorsAndTiles ()
	{
		Room[] sceneRooms = GameObject.FindObjectsOfType<Room> ();
		foreach (Room r in sceneRooms)
		{
			Debug.Log (r.name + " bad doors cleaned up: " + r.RemoveBadDoors ());
			Debug.Log (r.name + " bad tiles cleaned up: " + r.RemoveBadTiles ());
		}
	}

	private void IdentifyErrorDoors ()
	{
		List<Door> badDoors = FindErrorDoors ();
		string errorString = "Number of bad doors found: " + badDoors.Count;
		foreach (Door d in badDoors)
		{
			errorString += "\n " + d.name;
		}
		Debug.LogWarning (errorString);
	}

	/// <summary>
	/// Find doors with bad information.
	/// </summary>
	/// <returns>The error doors.</returns>
	private List<Door> FindErrorDoors ()
	{
		Door[] allDoors = GameObject.FindObjectsOfType<Door> ();
		List<Door> badDoors = new List<Door> ();

		foreach (Door d in allDoors)
		{
			if (!d.IsDoorInfoGood ())
			{
				badDoors.Add (d);
			}
		}

		return badDoors;
	}

	private void RemoveErrorDoors ()
	{
		List<Door> badDoors = FindErrorDoors ();
		foreach (Door d in badDoors)
		{
			Undo.DestroyObjectImmediate (d.gameObject);
		}
	}

	private void RemoveHiddenWalls ()
	{
		Door selectedDoor = Selection.activeGameObject.GetComponent<Door> ();
		if (selectedDoor != null)
		{
			Vector3 checkPos = selectedDoor.gameObject.transform.position;

			// Don't remove your own, move for now
			checkPos.z = -1f;
			selectedDoor.gameObject.transform.position = checkPos;
			checkPos.z = -.5f;

			if (selectedDoor.IsHorizontal)
			{
				checkPos.x -= .5f;
				RemoveBehindPoint (checkPos);
				checkPos.x += 1f;
				RemoveBehindPoint (checkPos);
			}
			else
			{
				checkPos.y -= .5f;
				RemoveBehindPoint (checkPos);
				checkPos.y += 1f;
				RemoveBehindPoint (checkPos);
			}
		}

		// Now that the room tiles are gone, we can flatten these out
		Vector3 newPos = selectedDoor.gameObject.transform.position;
		newPos.z = 0.0f;
		selectedDoor.gameObject.transform.position = newPos;
	}

	private void ToggleDoorsMeshRenderer ()
	{
		//Debug.Log("Hello?");

		Door[] allDoors = GameObject.FindObjectsOfType<Door> ();

		foreach (Door d in allDoors)
		{
			MeshRenderer[] meshRenderers = d.GetComponentsInChildren<MeshRenderer> ();
			//Debug.Log(d);

			foreach (MeshRenderer m in meshRenderers)
			{
				//Debug.Log(m);
				m.enabled = !m.enabled;
			}
		}
	}

	private void RemoveBehindPoint (Vector3 point)
	{
		RaycastHit hit;
		Ray detectBehind = new Ray (point, new Vector3 (0f, 0f, 1f));
		
		if (Physics.Raycast (detectBehind, out hit))
		{
			GameObject obj = hit.collider.gameObject;

			// Only remove GameTiles
			GameTile tile = obj.GetComponent<GameTile> ();
			if (tile != null)
			{
				Debug.Log ("Removing tile: " + tile.name);
				tile.parentRoom.RemoveTileFromRoom (tile);
				Undo.DestroyObjectImmediate (obj);
			}
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
