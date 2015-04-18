using UnityEngine;
using UnityEditor;

using System.Collections;

public class GridBuilder : EditorWindow
{
	private int _gridWidth = 5;
	private int _gridHeight = 5;

	private static readonly int kRoomWidth = 10;
	private static readonly int kRoomHeight = 10;

	private GameObject _tilePrefab;
	
	[MenuItem("LudumDare32/Build Grid")]
	public static void Init () 
	{
		// Get existing open window or if none, make a new one:		
		GridBuilder window = ScriptableObject.CreateInstance<GridBuilder>();
		window.Show();

	}
	
	void OnGUI () 
	{
		GUILayout.Label ("Build a Grid of size.", EditorStyles.boldLabel);
		_gridWidth = EditorGUILayout.IntField(_gridWidth);
		_gridHeight = EditorGUILayout.IntField(_gridHeight);

		if(GUILayout.Button("Build Grid")) 
		{
			_tilePrefab = Resources.Load("Prefabs/GameTile") as GameObject;

			GameObject parent = GameObject.Find("tileRoot");
			if (parent == null)
			{
				parent = new GameObject();
				parent = GameObject.Instantiate(parent);
			}

			parent.name = "tileRoot";
		
			for(int i = 0; i < parent.transform.childCount; i++)
			{
				GameObject.Destroy(parent.transform.GetChild(i));
			}

			int tileCount = 1;
			for(int i = 0; i < _gridWidth; i++)
			{
				for(int j = 0; j < _gridHeight; j++)
				{
					GameObject tile = GameObject.Instantiate(_tilePrefab);
					tile.transform.localPosition = new Vector3(kRoomWidth * i , 0.0f, kRoomHeight * j);
					tile.name = "Tile" + tileCount;
					tile.transform.parent = parent.transform;
					tileCount++;
				}
			}
		}

	}
}
