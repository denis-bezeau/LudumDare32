using UnityEngine;
using UnityEditor;

using System.Collections;

public class GridBuilder : EditorWindow
{
	private int _roomWidthTiles = 5;
	private int _roomHeightTiles = 5;
	private string _roomName = "room";
	private GameEnums.RoomType _roomType = GameEnums.RoomType.Normal;

	private static readonly int kTileWidth = 10;
	private static readonly int kTileHeight = 10;
	private static readonly string kRoomPath = "Assets/Resources/Prefabs/Rooms/";
	private static readonly string kRoomSuffix = ".prefab";

	private GameObject _tilePrefab;
	
	[MenuItem("LudumDare32/Build Room")]
	public static void Init () 
	{
		// Get existing open window or if none, make a new one:		
		GridBuilder window = ScriptableObject.CreateInstance<GridBuilder>();
		window.Show();
	}
	
	void OnGUI () 
	{
		GUILayout.Label ("Build a Grid of size.", EditorStyles.boldLabel);

		_roomWidthTiles = EditorGUILayout.IntField("Room Width: ", _roomWidthTiles);
		_roomHeightTiles = EditorGUILayout.IntField("Room Height ", _roomHeightTiles);
		_roomName = EditorGUILayout.TextField("Room Name: ", _roomName);

		_roomType = (GameEnums.RoomType)EditorGUILayout.EnumPopup ("Room Type", _roomType);

		if(GUILayout.Button("Build Room")) 
		{
			_tilePrefab = Resources.Load("Prefabs/GameTile") as GameObject;

			GameObject roomObj = GameObject.Find(_roomName);
			if (roomObj == null)
			{
				roomObj = new GameObject();
			}

			roomObj.name = _roomName;

			if (_roomType == GameEnums.RoomType.Normal)
			{
				roomObj.gameObject.AddComponent<Room>();
			}
			else if (_roomType == GameEnums.RoomType.Escape)
			{
				roomObj.gameObject.AddComponent<EscapeRoom>();
			}
			else if (_roomType == GameEnums.RoomType.Spawn)
			{
				roomObj.gameObject.AddComponent<SpawnRoom>();
			}


			roomObj.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

			BoxCollider col = roomObj.gameObject.AddComponent<BoxCollider>();

			col.size = new Vector3(kTileWidth * _roomWidthTiles, 5.0f, kTileHeight * _roomHeightTiles);

			float colCenterX = ((kTileWidth * _roomWidthTiles) / 2.0f) - (kTileWidth / 2.0f);
			float colCenterY = ((kTileHeight * _roomHeightTiles) / 2.0f) - (kTileHeight / 2.0f);
		
			col.center = new Vector3(colCenterX , 0.0f, colCenterY);

			for(int i = 0; i < roomObj.transform.childCount; i++)
			{
				GameObject.DestroyImmediate(roomObj.transform.GetChild(i));
			}

			int tileCount = 1;

			for(int i = 0; i < _roomWidthTiles; i++)
			{
				for(int j = 0; j < _roomHeightTiles; j++)
				{
					GameObject tile = GameObject.Instantiate(_tilePrefab);
					tile.transform.localPosition = new Vector3(kTileWidth * i , 0.0f, kTileHeight * j);
					tile.name = "Tile" + tileCount;
					tile.transform.parent = roomObj.transform;
					tile.layer = LayerMask.NameToLayer("Tiles");
					tileCount++;
				}
			}

			// Art needs this rotated.
			roomObj.transform.Rotate(0.0f, 180.0f, 0.0f);

			GameObject prefab = PrefabUtility.CreatePrefab(kRoomPath + _roomName + kRoomSuffix, roomObj, ReplacePrefabOptions.Default); 

			GameObject.DestroyImmediate(roomObj);

			PrefabUtility.InstantiatePrefab(prefab);
		}
	}
}
