using UnityEngine;
using System.Collections;

public class EnemySortOrder : MonoBehaviour
{

	private GameObject levelTopNull;
	public GameObject SpriteObject;


	void Awake ()
	{
		levelTopNull = GameObject.FindGameObjectWithTag ("levelTopNode");
	}
	

	void Update ()
	{
		if (levelTopNull != null)
		{
			Vector3 currentPos = transform.position;
			
			float diff = currentPos.y - levelTopNull.transform.position.y;
			Vector3 newTransPos = new Vector3 (SpriteObject.transform.position.x, SpriteObject.transform.position.y, (diff * 0.1f) - 0.1f);
			
			SpriteObject.transform.position = newTransPos;
		}

	
	}
}
