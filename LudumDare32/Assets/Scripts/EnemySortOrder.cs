using UnityEngine;
using System.Collections;

public class EnemySortOrder : MonoBehaviour {

	private GameObject levelTopNull;


	void Start () 
	{
		levelTopNull = GameObject.FindGameObjectWithTag("levelTopNode");
	}
	

	void Update () 
	{
		Vector3 currentPos = transform.localPosition;

		float diff = currentPos.y - levelTopNull.transform.localPosition.y;
		Vector3 newTransPos = new Vector3(currentPos.x, currentPos.y, (diff*0.1f)+0.1f);

		transform.localPosition = newTransPos;
	
	}
}
