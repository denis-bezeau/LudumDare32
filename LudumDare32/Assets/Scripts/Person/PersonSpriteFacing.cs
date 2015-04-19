using UnityEngine;
using System.Collections;

public class PersonSpriteFacing : MonoBehaviour {


	private float lastPos;
	private Vector3 newScale;
	public Transform enemySprite;

	void Start ()
	{
		newScale = new Vector3 (1.0f, 1.0f, 1.0f);
		lastPos = transform.localPosition.x;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(transform.localPosition.x > lastPos)
		{
			newScale = new Vector3 (1.0f, 1.0f, 1.0f);
		}
		if(transform.localPosition.x < lastPos)
		{
			newScale = new Vector3 (-1.0f, 1.0f, 1.0f);
		}



		enemySprite.localScale = newScale;
		lastPos = transform.localPosition.x;
	
	}
}
