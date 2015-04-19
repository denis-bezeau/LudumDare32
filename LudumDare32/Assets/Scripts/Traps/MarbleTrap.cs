using UnityEngine;
using System.Collections.Generic;

public class MarbleTrap : Trap
{
	public float speedModifier = 0.5f;
	private HauntedMarbles hauntedMarbles = null;

	public void Awake()
	{
		hauntedMarbles = GetComponent<HauntedMarbles>();
		OnEnterTrap(null);

	}
	public override void OnEnterTrap(PersonAI person)
	{
		person.GetComponent<PersonMotion>().m_speedModifier = speedModifier;

		if (hauntedMarbles != null)
		{
			hauntedMarbles.isActive = true;
		}
	}

	public override void OnExitTrap(PersonAI person)
	{
		person.GetComponent<PersonMotion>().m_speedModifier = 1.0f;
	}
	
}