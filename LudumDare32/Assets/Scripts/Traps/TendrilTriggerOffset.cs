using UnityEngine;
using System.Collections;

public class TendrilTriggerOffset : MonoBehaviour {
	
	private int tendrilCount      = 0;
	public bool isAttack          = false;
	public float randTimeMin;
	public float randTimeMax;


	private float timer           = 0.0f;
	private float randomOffset    = 0.0f;
	public GameObject[] tendrils;

	void Update () 
	{
		timer += 1.0f*Time.deltaTime;
//		Debug.Log(timer);
		if(isAttack == true)
		{
			if(timer >= randomOffset)
			{

				tendrilAttack(tendrilCount);
				tendrilCount+=1;
				timer = 0.0f;
				randomOffset = Random.Range(0.1f, 0.6f);

			}

			if(tendrilCount >= tendrils.Length)
			{
				isAttack = false;
				tendrilCount = 0;
			}

		}
	}

	void tendrilAttack(int tendrilNum)
	{
		Debug.Log (tendrilNum);
		Animator anim = tendrils[tendrilNum].GetComponent<Animator>();
		anim.SetTrigger ("Attack");
	}
}
