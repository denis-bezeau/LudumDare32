using UnityEngine;

public class SmokeTrap : RoomTrap
{
	private new const string _title = "Smoke Trap";
	public override string Title { get { return _title; } }
	
	private const int _cost = 30;
	public override int Cost { 
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