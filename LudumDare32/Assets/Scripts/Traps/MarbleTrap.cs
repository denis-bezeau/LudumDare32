using UnityEngine;
using System.Collections.Generic;

public class MarbleTrap : Trap
{
	public float speedModifier = 0.5f;
	private HauntedMarbles hauntedMarbles = null;

	public void Awake()
	{
		hauntedMarbles = GetComponent<HauntedMarbles>();
	}

	void Update()
	{
		base.UpdateTrap();
	}

	public override void OnEnterTrap(PersonAI person)
	{
		person.GetComponent<PersonMotion>().m_speedModifier = speedModifier;

		hauntedMarbles.isActive = true;
	}

	public override void OnExitTrap(PersonAI person)
	{
		person.GetComponent<PersonMotion>().m_speedModifier = 1.0f;
		hauntedMarbles.isActive = false;
	}

	void OnTriggerEnter(Collider col)
	{
		base.OnTriggerEnter(col);
	}
	
	void OnTriggerExit(Collider col)
	{
		base.OnTriggerExit(col);
	}
}