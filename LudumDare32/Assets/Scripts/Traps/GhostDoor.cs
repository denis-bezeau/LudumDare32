using UnityEngine;
using System.Collections;

public class GhostDoor : MonoBehaviour {

	public bool isHit        = false;
	private bool gettingHit  = false;
	private bool coolDown    = false;
	private float coolDownTimer  = 0.0f;
	private float HitFaceTimer   = 0.0f;

	private float coolDownTime    = 0.75f;
	private float hitfaceVisTime  = 0.25f;
	public GameObject hitFace;

	void Update () 
	{
		if(coolDown == true)
		{
			coolDownTimer += 1.0f * Time.deltaTime;
			isHit = false;
			if (coolDownTimer >= coolDownTime)
			{
				coolDown = false;
				coolDownTime = Random.Range(0.15f, 0.75f);
				coolDownTimer = 0.0f;
			}
		}

		if(isHit == true)
		{
			hitFace.SetActive(true);
			gettingHit = true;

		}

		if (gettingHit == true)
		{
			isHit = false;
			HitFaceTimer += 1.0f * Time.deltaTime;
			if(HitFaceTimer >= hitfaceVisTime)
			{
				HitFaceTimer = 0.0f;
				hitFace.SetActive(false);
				coolDown = true;
				gettingHit = false;
			}
		}



	}
}
