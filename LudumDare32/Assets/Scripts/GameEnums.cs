using UnityEngine;
using System.Collections;

public class GameEnums
{
	public enum RoomType
	{
		Normal = 0,
		Spawn,
		Escape
	}

	public enum TileSet
	{
		TileSet0 = 0,
		TileSet1 = 1,

		//Count
	}

	public enum TileType
	{
		WallTop = 0,
		WallBottom,
		WallLeft,
		WallRight,
		WallCornerNW,
		WallCornerNE,
		WallCornerSW,
		WallCornerSE,
		Floor,

		Count
	}
}
