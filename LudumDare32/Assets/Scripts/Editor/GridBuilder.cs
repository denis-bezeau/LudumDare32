using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

public class GridBuilder : EditorWindow
{
	private int _roomWidthTiles = 5;
	private int _roomHeightTiles = 5;
	private string _roomName = "room";
	private bool _showNameError = false;

	private GameEnums.RoomType _roomType = GameEnums.RoomType.Normal;
	private GameEnums.TileSet _tileSet = GameEnums.TileSet.TileSet0;

	private static readonly int kTileWidth = 1;
	private static readonly int kTileHeight = 1;

	private static readonly float kZDepth = 1f;

	private static readonly string kRoomPath = "Assets/Resources/Prefabs/Rooms/";
	private static readonly string kTileSetPath = "Textures/TileSets/";
	private static readonly string kTileSetMatPath = "Materials";
	private static readonly string kRoomSuffix = ".prefab";

	private static Vector2 _scrollPosition = Vector2.zero;

	private static string[] _tileTextureStrings =
	{
		"Wall_Top",
		"Wall_Bottom",
		"Wall_Left",
		"Wall_Right",
		"Wall_Corner_NW",
		"Wall_Corner_NE",
		"Wall_Corner_SW",
		"Wall_Corner_SE",
		"Floor_01"
	};
	
	private GameObject _tilePrefab;
	
	[MenuItem("LudumDare32/Build Room")]
	public static void Init ()
	{
		// Get existing open window or if none, make a new one:		
		GridBuilder window = ScriptableObject.CreateInstance<GridBuilder> ();
		window.Show ();
	}

	void OnGUI ()
	{
		GUILayout.Label ("Build a Grid of size.", EditorStyles.boldLabel);
		_scrollPosition = EditorGUILayout.BeginScrollView (_scrollPosition);
		_roomWidthTiles = EditorGUILayout.IntField ("Room Width: ", _roomWidthTiles);
		_roomHeightTiles = EditorGUILayout.IntField ("Room Height ", _roomHeightTiles);
		_roomName = EditorGUILayout.TextField ("Room Name: ", _roomName);

		_roomType = (GameEnums.RoomType)EditorGUILayout.EnumPopup ("Room Type", _roomType);
		_tileSet = (GameEnums.TileSet)EditorGUILayout.EnumPopup ("Tile Set", _tileSet);

		if (GUILayout.Button ("Build Room"))
		{
			if (!NameIsUnique ())
			{
				_showNameError = true;
			}
			else
			{
				_showNameError = false;
				BuildRoom ();
			}
		}

		// Show error displays
		if (_showNameError)
		{
			EditorGUILayout.HelpBox ("Please enter a unique room name", MessageType.Error);
		}
		EditorGUILayout.EndScrollView ();
	}

	private void BuildRoom ()
	{
		string rootTexPath = kTileSetPath + _tileSet.ToString () + "/" + kTileSetMatPath + "/";

		_tilePrefab = Resources.Load ("Prefabs/GameTile") as GameObject;
		
		GameObject roomObj = GameObject.Find (_roomName);
		if (roomObj == null)
		{
			roomObj = new GameObject ();
		}
		
		roomObj.name = _roomName;
		
		if (_roomType == GameEnums.RoomType.Normal)
		{
			roomObj.gameObject.AddComponent<Room> ();
		}
		else if (_roomType == GameEnums.RoomType.Escape)
		{
			roomObj.gameObject.AddComponent<EscapeRoom> ();
		}
		else if (_roomType == GameEnums.RoomType.Spawn)
		{
			roomObj.gameObject.AddComponent<SpawnRoom> ();
		}

		roomObj.GetComponent<Room>().GameTiles = new List<GameTile>();
		
		roomObj.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
		
		// Build the Room Collider
		BoxCollider col = roomObj.gameObject.AddComponent<BoxCollider> ();
		
		// Height is X, width is Y
		col.size = new Vector3 (kTileHeight * _roomHeightTiles, kTileWidth * _roomWidthTiles, kZDepth);
		
		float colCenterX = ((kTileWidth * _roomWidthTiles) / 2.0f) - (kTileWidth / 2.0f);
		float colCenterY = ((kTileHeight * _roomHeightTiles) / 2.0f) - (kTileHeight / 2.0f);
		
		col.center = new Vector3 (colCenterY, -1.0f * colCenterX, 0.0f);
		
		for (int i = 0; i < roomObj.transform.childCount; i++)
		{
			GameObject.DestroyImmediate (roomObj.transform.GetChild (i));
		} 
		
		int tileCount = 1;
		
		for (int i = 0; i < _roomWidthTiles; i++)
		{
			for (int j = 0; j < _roomHeightTiles; j++)
			{
				GameObject tile = GameObject.Instantiate (_tilePrefab);
				tile.transform.localPosition = new Vector3 (kTileWidth * j, -1.0f * kTileHeight * i, 0.0f);
				tile.name = "Tile" + tileCount;
				tile.transform.parent = roomObj.transform;
				tile.layer = LayerMask.NameToLayer ("Tiles");
				
				GameTile gameTile = tile.GetComponent<GameTile> ();
				
				// Build the tile colliders and Textures.
				
				// First, Build the NW Tile
				if ((i == 0 && j == 0))
				{
					GameObject wall1 = new GameObject();

					BoxCollider col1 = wall1.AddComponent<BoxCollider> ();
					col1.size = new Vector3 (1f, 0.25f, 0.5f);
					col1.center = new Vector3 (0.0f, 0.375f, 0.0f);
					
					BoxCollider col2 = wall1.AddComponent<BoxCollider> ();
					col2.size = new Vector3 (0.25f, 1.0f, 0.5f);
					col2.center = new Vector3 (-0.375f, 0.0f, 0.0f);
					
					gameTile.SetTexture (rootTexPath + _tileTextureStrings [(int)GameEnums.TileType.WallCornerNW]);
					wall1.layer = LayerMask.NameToLayer ("Walls");

					wall1.transform.parent = gameTile.transform;
					wall1.transform.localPosition = Vector3.zero;

					wall1.name = "Walls";
					gameTile.IsWallTile = true;
				}
				// Then, Build the NE Tile
				else if (i == 0 && j == _roomHeightTiles - 1)
				{
					GameObject wall1 = new GameObject();

					BoxCollider col1 = wall1.AddComponent<BoxCollider> ();
					col1.size = new Vector3 (1.0f, 0.25f, 0.5f);
					col1.center = new Vector3 (0.0f, 0.375f, 0.0f);
					
					BoxCollider col2 = wall1.AddComponent<BoxCollider> ();
					col2.size = new Vector3 (0.25f, 1.0f, 0.5f);
					col2.center = new Vector3 (0.375f, 0.0f, 0.0f);
					
					gameTile.SetTexture (rootTexPath + _tileTextureStrings [(int)GameEnums.TileType.WallCornerNE]);

					wall1.layer = LayerMask.NameToLayer ("Walls");

					wall1.transform.parent = gameTile.transform;
					wall1.transform.localPosition = Vector3.zero;

					wall1.name = "Walls";
					gameTile.IsWallTile = true;
				}
				// Then, build the SW Tile
				else if (i == _roomWidthTiles - 1 && j == 0)
				{
					GameObject wall1 = new GameObject();

					BoxCollider col1 = wall1.AddComponent<BoxCollider> ();
					col1.size = new Vector3 (1.0f, 0.25f, 0.5f);
					col1.center = new Vector3 (0.0f, -0.375f, 0.0f);
					
					BoxCollider col2 = wall1.AddComponent<BoxCollider> ();
					col2.size = new Vector3 (0.25f, 1.0f, 0.5f);
					col2.center = new Vector3 (-0.375f, 0.0f, 0.0f);
					
					gameTile.SetTexture (rootTexPath + _tileTextureStrings [(int)GameEnums.TileType.WallCornerSW]);
					wall1.layer = LayerMask.NameToLayer ("Walls");

					wall1.transform.parent = gameTile.transform;
					wall1.transform.localPosition = Vector3.zero;

					wall1.name = "Walls";
					gameTile.IsWallTile = true;
				}
				// Then, build the SE Tile
				else if (i == _roomWidthTiles - 1 && j == _roomHeightTiles - 1)
				{
					GameObject wall1 = new GameObject();

					BoxCollider col1 = wall1.AddComponent<BoxCollider> ();
					col1.size = new Vector3 (1.0f, 0.25f, 0.5f);
					col1.center = new Vector3 (0.0f, -0.375f, 0.0f);
					
					BoxCollider col2 = wall1.AddComponent<BoxCollider> ();
					col2.size = new Vector3 (0.25f, 1.0f, 0.5f);
					col2.center = new Vector3 (0.375f, 0.0f, 0.0f);
					
					gameTile.SetTexture (rootTexPath + _tileTextureStrings [(int)GameEnums.TileType.WallCornerSE]);
					wall1.layer = LayerMask.NameToLayer ("Walls");

					wall1.transform.parent = gameTile.transform;
					wall1.transform.localPosition = Vector3.zero;

					wall1.name = "Walls";
					gameTile.IsWallTile = true;
				}
				// Then, left wall colliders
				else if (j == 0)
				{
					GameObject wall1 = new GameObject();

					BoxCollider col1 = wall1.AddComponent<BoxCollider> ();
					col1.size = new Vector3 (0.25f, 1.0f, 0.5f);
					col1.center = new Vector3 (-0.375f, 0.0f, 0.0f);
					
					gameTile.SetTexture (rootTexPath + _tileTextureStrings [(int)GameEnums.TileType.WallLeft]);

					wall1.name = "Walls";
					gameTile.IsWallTile = true;

					wall1.layer = LayerMask.NameToLayer ("Walls");
					wall1.transform.parent = gameTile.transform;
					wall1.transform.localPosition = Vector3.zero;
				}
				// Then, bottom wall colliders
				else if (i == _roomWidthTiles - 1)
				{
					GameObject wall1 = new GameObject();

					BoxCollider col1 = wall1.AddComponent<BoxCollider> ();
					col1.size = new Vector3 (1.0f, 0.25f, 0.5f);
					col1.center = new Vector3 (0.0f, -0.375f, 0.0f);
					
					gameTile.SetTexture (rootTexPath + _tileTextureStrings [(int)GameEnums.TileType.WallBottom]);
					gameTile.IsWallTile = true;

					wall1.layer = LayerMask.NameToLayer ("Walls");

					wall1.name = "Walls";
					wall1.transform.parent = gameTile.transform;
					wall1.transform.localPosition = Vector3.zero;
				}
				// Then, top wall colliders
				else if (i == 0)
				{
					GameObject wall1 = new GameObject();

					BoxCollider col1 = wall1.AddComponent<BoxCollider> ();
					col1.size = new Vector3 (1.0f, 0.25f, 0.5f);
					col1.center = new Vector3 (0.0f, 0.375f, 0.0f);
					
					gameTile.SetTexture (rootTexPath + _tileTextureStrings [(int)GameEnums.TileType.WallTop]);
					gameTile.IsWallTile = true;

					wall1.layer = LayerMask.NameToLayer ("Walls");

					wall1.name = "Walls";
					wall1.transform.parent = gameTile.transform;
					wall1.transform.localPosition = Vector3.zero;
				}
				// finally, right wall colliders
				else if (j == _roomHeightTiles - 1)
				{
					GameObject wall1 = new GameObject();

					BoxCollider col1 = wall1.AddComponent<BoxCollider> ();
					col1.size = new Vector3 (0.25f, 1.0f, 0.5f);
					col1.center = new Vector3 (0.375f, 0.0f, 0.0f);
					
					gameTile.SetTexture (rootTexPath + _tileTextureStrings [(int)GameEnums.TileType.WallRight]);
					gameTile.IsWallTile = true;

					wall1.layer = LayerMask.NameToLayer ("Walls");

					wall1.name = "Walls";
					wall1.transform.parent = gameTile.transform;
					wall1.transform.localPosition = Vector3.zero;
				}
				else
				{
					gameTile.SetTexture (rootTexPath + _tileTextureStrings [(int)GameEnums.TileType.Floor]);
					gameTile.IsWallTile = false;
				}
				roomObj.GetComponent<Room>().GameTiles.Add (gameTile);
				tileCount++;
			}
		}
		
		GameObject prefab = PrefabUtility.CreatePrefab (kRoomPath + _roomName + kRoomSuffix, roomObj, ReplacePrefabOptions.Default); 
		
		GameObject.DestroyImmediate (roomObj);
		
		PrefabUtility.InstantiatePrefab (prefab);
	}

	private bool NameIsUnique ()
	{
		return (null == GameObject.Find (_roomName)) ? true : false;
	}
}