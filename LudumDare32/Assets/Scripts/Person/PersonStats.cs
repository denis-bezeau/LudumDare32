using UnityEngine;
using System.Collections;

public class PersonStats : MonoBehaviour
{
	private int hp = 5;

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
