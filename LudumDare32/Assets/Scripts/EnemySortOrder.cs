using UnityEngine;
using System.Collections;

public class EnemySortOrder : MonoBehaviour
{
	// This node helps us know where the top of the level is
	private GameObject _levelTopNode;

	// TODO: Shouldn't the EnemySortOrder just be party of the sprite GameObject?
	public GameObject SpriteObject;
	
	void Awake ()
	{
		_levelTopNode = GameObject.FindGameObjectWithTag ("levelTopNode");
	}

	void Update ()
	{
		if (_levelTopNode != null)
		{
			// On each update make sure we are at the correct z-depth for our corresponding y-height
			Vector3 currentPos = transform.position;
			
			float diff = currentPos.y - _levelTopNode.transform.position.y;
			Vector3 newTransPos = new Vector3 (SpriteObject.transform.position.x, SpriteObject.transform.position.y, (diff * 0.1f) - 0.01f);
			
			SpriteObject.transform.position = newTransPos;
		}
	}
}
