using UnityEngine;
using System.Collections;

public class HauntedMarbles : MonoBehaviour {

	public bool isActive;

	private Animator anim;
	public GameObject marbleSprite;



	void Awake () 
	{
		anim = marbleSprite.GetComponent<Animator>();
	}

	void Update () 
	{
		if(isActive)
		{
			anim.SetBool ("isMoving", true);
		}
		else
		{
			anim.SetBool ("isMoving", false);
		}
	}
}
