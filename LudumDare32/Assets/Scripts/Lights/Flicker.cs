using UnityEngine;
using System.Collections;

public class Flicker : MonoBehaviour {

	private Animator anim;
	private float timer;
	private float randomTime;
	public float flickerMin    = 10.0f;
	public float flickerMax    = 120.0f;
	
	void Start () 
	{
		anim = gameObject.GetComponent<Animator>();
		timer = 0.0f;
		randomTime = Random.Range(0.0f, flickerMax);
	}

	void Update () 
	{
		timer += 1.0f * Time.deltaTime;

		if(timer >= randomTime)
		{
			anim.SetTrigger("flicker");
			timer = 0.0f;
			randomTime = Random.Range(flickerMin, flickerMax);
		}
	}
}
