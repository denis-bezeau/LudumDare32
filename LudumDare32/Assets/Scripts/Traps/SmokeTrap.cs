using UnityEngine;

public class SmokeTrap : RoomTrap
{
	private const string _title = "Smoke Trap";
	public virtual string Title { get { return _title; } }
	
	private const int _cost = 30;
	public virtual int Cost { 
		get { return _cost; } 
	}

	public SmokeTrap (Room parent) : base(parent)
	{

	}

	public override void ApplyTrapEffect ()
	{
		base.ApplyTrapEffect ();

		foreach (PersonAI person in _parentRoom.PeopleInRoom)
		{
			// Apply effect
		}
	}
}