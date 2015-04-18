using UnityEngine;
/// <summary>
/// Type of trap that affects an entire room
/// </summary>
public class RoomTrap : Trap
{
	/// <summary>
	/// The frequency with which we apply the trap's effect, in seconds
	/// </summary>
	[SerializeField]
	private float
		_effectFrequency = 5f;
	private float _timeSinceLastEffect = 0f;

	public RoomTrap (Room parent) : base(parent)
	{

	}

	public void Update()
	{
		_timeSinceLastEffect += Time.deltaTime;
		if (_timeSinceLastEffect > _effectFrequency)
		{
			ApplyTrapEffect();
			_timeSinceLastEffect = 0f;
		}

	}

	public virtual void ApplyTrapEffect()
	{
		Debug.LogWarning(this.name + " applying effect");
		// nothing in base for now
		// Generally apply effect to all _parentRoom.PeopleInRoom
	}
}