using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base class for interactive objects.
/// </summary>
public abstract class InteractiveObject : MonoBehaviour
{
	///<summary>
	/// Tags to describe an interactive object. Used to decide an NPC's
	/// preference for using an object.
	/// </summary>
	/// <remarks>Please keep in alphabetical order for ease of use</remarks>
	public enum AttributeTag
	{
		COLD,
		COMFY,
		COMPLICATED,
		HARD,
		HEALTHY,
		LOUD,
		QUIET,
		SOFT,
		SOUR,
		SWEET,
		WARM
	}

	private const int INFINITE_USES_VAL = -1;

	private int _numUses = 0;
	private bool _isBeingUsed = false;

	protected List<Interaction> _interactions = new List<Interaction>();
	protected int _maxNumUses = -1; // This will allow infinite uses

	#region Properties
	public bool IsBeingUsed {
		get { return _isBeingUsed;}
	}

	public bool CanBeUsed {
		get {
			return HasUsesLeft && !IsBeingUsed;
		}
	}

	public bool HasUsesLeft {
		get {
			return (_maxNumUses == INFINITE_USES_VAL) || (_numUses <= _maxNumUses);
		}
	}

	public List<Interaction> Interactions {
		get{ return _interactions;}
	}
	#endregion



	#region Abstract Functions & Properties
	#endregion

	#region Virtual Functions & Properties
	public virtual void BeginInteracting ()
	{
		_isBeingUsed = true;
		_numUses++;
	}

	// TODO: Not sure yet if the interactive object should be running the interaction, we'll see
	/*public virtual bool Interact (Interaction chosenAction)
	{

		if(chosenAction is ToggleLight)
		{

		}

		// Return whether or not the interaction was successful
		return true;
	}*/

	public virtual void FinishInteracting ()
	{
		_isBeingUsed = false;
	}
	#endregion
}