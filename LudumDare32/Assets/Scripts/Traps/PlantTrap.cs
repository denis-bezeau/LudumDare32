using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Type of trap that affects an entire room
/// </summary>
public class PlantTrap : Trap
{
	public GameObject PaintCanTrapArtPrefab;
	
	public Room _parentRoom;

	public int Damage;

	private GameObject paintCanArtGameObject;
	private Animator paintCanTrapAnimator;
	bool markForDelete = false;

	public void Awake ()
	{
		paintCanArtGameObject = GameObject.Instantiate(PaintCanTrapArtPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		if (_parentRoom != null)
		{
			paintCanArtGameObject.transform.parent = _parentRoom.transform;
		}
		paintCanTrapAnimator = paintCanArtGameObject.GetComponent<Animator>();
		PlantTrapStateMachine paintCanStateMachineBehaviour = paintCanTrapAnimator.GetBehaviour<PlantTrapStateMachine>();
		paintCanStateMachineBehaviour.OnAnimFinished = OnAnimationComplete;
	}

	public override void OnEnterTrap()
	{
		Debug.Log(name + "OnEnterTrap");
		base.OnEnterTrap();

		Animator paintCanTrapAnimator = paintCanArtGameObject.GetComponent<Animator>();
		paintCanTrapAnimator.SetInteger("Execute", 1);
		
		if (_parentRoom != null)
		{
			List<PersonAI> people = _parentRoom.GetPeople();
			for (int i = 0; i < people.Count; ++i)
			{
				people[i].GetPlayerStats().HP -= Damage;
				Debug.Log(people[i].name + ".HP=" + people[i].GetPlayerStats().HP);
			}
		}
	}
	
	public void OnAnimationComplete(eAnimTypes animType)
	{
		Debug.Log(name + "OnAnimationComplete()" + animType);
		markForDelete = true;
	}

	public void Update()
	{
		if(markForDelete == true)
		{
			Debug.Log("trying to destroy " + name);
			GameObject.Destroy(paintCanArtGameObject);
		}
	}
}