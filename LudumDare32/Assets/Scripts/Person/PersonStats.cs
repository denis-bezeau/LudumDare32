using UnityEngine;
using System.Collections;

public class PersonStats : MonoBehaviour
{
	public int MaxHP = 30;
	private int hp = 5;

	public void Awake()
	{
		HP = MaxHP;
	}

	public int HP
	{
		get { return hp; }
		set 
		{
			hp = value;

			if (hp <= 0)
			{
				PersonAI person=  GetComponent<PersonAI>();
				person.Kill();
			}
		}
	
	}
}
