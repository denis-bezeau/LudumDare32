using UnityEngine;
using System.Collections.Generic;

public class InteractiveObject : MonoBehaviour
{

	private bool _isBeingUsed = false;
	public bool IsBeingUsed {
		get { return _isBeingUsed;}
	}

	private List<Interaction> _interactions = new List<Interaction>
	{
		new ToggleLight()
	};

	public List<Interaction> GetInteractions ()
	{
		return _interactions;
	}


	public virtual void BeginInteracting ()
	{
		_isBeingUsed = true;
	}

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
}