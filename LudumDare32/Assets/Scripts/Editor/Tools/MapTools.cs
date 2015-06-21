
using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

public class MapTools : EditorWindow
{

	private static Vector2 _scrollPosition = Vector2.zero;
	private static Room _room1 = null;
	private static Room _room2 = null;

	private GameEnums.TileSet _tileSet = GameEnums.TileSet.TileSet1;

	private bool _showUnassignedHelp = false;
	private bool _showRoomHelp = false;
	private string _roomHelpString = string.Empty;
	private bool _isSure = false;
	
	[MenuItem("LudumDare32/Map Edit Tools")]
	public static void Init ()
	{
		// Get existing open window or if none, make a new one:		
		MapTools window = ScriptableObject.CreateInstance<MapTools>();
		window.Show();
	}
	
	void OnSelectionChange ()
	{
		// Make sure we update when selection is new
		Repaint();
	}
	
	void OnGUI ()
	{
		GUILayout.Label("Testing", EditorStyles.boldLabel);
		
		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

		
		if (GUILayout.Button("Assign Objects"))
		{
			Room[] theRooms = GameObject.FindObjectsOfType<Room>();
			InteractiveObject[] theObjects = GameObject.FindObjectsOfType<InteractiveObject>();

			Debug.Log("Rooms: " + theRooms.Length + " Objects: " + theObjects.Length);
			int addedObjectCount = 0;

			foreach (Room r in theRooms)
			{
				r.ClearRoomObjects();
				Bounds roomColBounds = r.RoomCollider.bounds;

				foreach (InteractiveObject io in theObjects)
				{
					if (roomColBounds.Contains(io.gameObject.transform.position))
					{
						r.AddObjectToRoom(io);
						addedObjectCount++;
					}
				}
				EditorUtility.SetDirty(r);
			}

			Debug.Log(addedObjectCount + " object(s) assigned to rooms, " + (theObjects.Length - addedObjectCount) + " unassigned.");
		}

		if (GUILayout.Button("Clear Objects"))
		{
			Room[] theRooms = GameObject.FindObjectsOfType<Room>();

			foreach (Room r in theRooms)
			{
				r.ClearRoomObjects();
				EditorUtility.SetDirty(r);
			}
		}
		
		// Error displays
		if (_showUnassignedHelp)
		{
			EditorGUILayout.HelpBox("There are unassigned objects, consult the Console Window", MessageType.Error);
		}

		EditorGUILayout.EndScrollView();
	}
}
