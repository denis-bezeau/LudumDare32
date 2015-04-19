using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Type of trap that affects an entire room
/// </summary>
public class PlantTrap : Trap
{
	public Room _parentRoom;

	public int Damage;

	private GameObject paintCanArtGameObject;
	private Animator paintCanTrapAnimator;
	bool markForDelete = false;


	public override void OnEnterTrap()
	{
		Debug.Log(name + "OnEnterTrap");
		base.OnEnterTrap();

		Animator[] animators = GetComponentsInChildren<Animator>() as Animator[];

		for (int i = 0; i < animators.Length; ++i)
		{
			paintCanTrapAnimator.SetInteger("Execute", 1);
		}
		
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