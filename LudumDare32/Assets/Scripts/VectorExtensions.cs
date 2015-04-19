using UnityEngine;
using System.Collections;

// Global Game Jam 2015 Vector Extensions
public static class VectorExtensions 
{
	/// Vector 2 Extensions
	public static Vector2 ToVector2YZ (this Vector3 v) 
	{
		return new Vector2 (v.y, v.z);
	}

	public static Vector2 ToVector2XZ (this Vector3 v) 
	{
		return new Vector2 (v.x, v.z);
	}

	public static Vector2 ToVector2XY (this Vector3 v) 
	{
		return new Vector2 (v.x, v.y);
	}

	/// Vector 3 Extensions
	public static Vector3 ToVector3XZ (this Vector2 v) 
	{
		return new Vector3 (v.x, 0.0f, v.y);
	}

}
