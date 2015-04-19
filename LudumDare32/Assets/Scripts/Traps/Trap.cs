using UnityEngine;
using System.Collections.Generic;

public class Trap : MonoBehaviour
{
	public enum TrapType
	{
		None,
		Plant,
		Marble,
		Door,
		COUNT
	}

	protected const string _title = "default";
	public virtual string Title { get { return _title; } }

	private int	_cost = 1;
	public virtual int Cost { 
		get { return _cost; } 
	}

	/// <summary>
	/// Damage trap can take before being destroyed
	/// </summary>
	private int _hitPoints = -1;

	/// <summary>
	/// Length of time this trap exists
	/// </summary>
	private int _duration = -1;

	/// <summary>
	/// What to do when a new person enters the room
	/// </summary>
	public virtual void OnEnterTrap (PersonAI person)
	{
		
	}
	
	/// <summary>
	/// What to do when a person exits a room
	/// </summary>
	public virtual void OnExitTrap(PersonAI person)
	{
		
	}
}