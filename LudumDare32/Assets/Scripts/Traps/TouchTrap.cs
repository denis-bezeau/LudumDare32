using UnityEngine;

/// <summary>
/// A type of trap that has to be touched to have an affect
/// </summary>
public class TouchTrap : Trap
{
	private Collider _trapCollider = null;

	public TouchTrap (Room parent) : base(parent)
	{
		_trapCollider = GetComponent<Collider>();
		if (_trapCollider != null)
		{
			// listen for collisions with trap, blah blah
		} else
		{
			Debug.LogError(this.name + " doesn't have a collider!");
		}
	}

	public virtual void ApplyTrapEffect()
	{
		Debug.LogWarning(this.name + " applying effect");
		// nothing in base for now
		// Generally apply to anyone who is in contact with trap
	}
}


