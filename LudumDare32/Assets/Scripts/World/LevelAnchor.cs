using UnityEngine;
using System.Collections;

// This is a really dumb script.
public class LevelAnchor : MonoBehaviour 
{
	[SerializeField]
	private Bounds LevelBounds;

	public Bounds GetBounds
	{
		get { return LevelBounds; }
	}
}
