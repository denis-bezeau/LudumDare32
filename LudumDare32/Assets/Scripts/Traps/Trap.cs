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

	public Room _parentRoom;

	protected const string _title = "default";
	public virtual string Title { get { return _title; } }

	private int	_cost = 1;
	public virtual int Cost { 
		get { return _cost; } 
	}

	private float _timeTicker;
	public float TrapDuration = 3.0f;

	private BoxCollider _trapCol;

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

	void Awake()
	{
		_timeTicker = 0.0f;
		_trapCol = this.gameObject.GetComponent<BoxCollider>();
	}

	public void UpdateTrap()
	{
		if(_timeTicker >= TrapDuration)
		{
			GameObject.Destroy(this.gameObject);
		}
		_timeTicker += Time.deltaTime;
	}

	public void OnTriggerEnter(Collider col)
	{
		PersonAI person = col.gameObject.GetComponent<PersonAI>();
		if(person != null)
		{
			OnEnterTrap(person);
		}
	}

	public void OnTriggerExit(Collider col)
	{
		PersonAI person = col.gameObject.GetComponent<PersonAI>();
		if(person != null)
		{
			OnExitTrap(person);
		}
	}
}