using UnityEngine;
using System.Collections.Generic;

public class Lamp : InteractiveObject
{
	[SerializeField]
	Light
		_theLight = null;

	protected List<Interaction> _interactions = new List<Interaction>
	{
		new ToggleLight()
	};

	public override List<Interaction> GetInteractions ()
	{
		return _interactions;
	}

	public bool IsOn {
		get { 
			return _theLight.enabled;
		}
	}
}

public class ToggleLight : Interaction
{
	public ToggleLight ()
	{

	}

	public override bool Interact (InteractiveObject interObject, MentalStateControl mentalState)
	{
		Light theLight = interObject.GetComponentInChildren<Light>();
		if (theLight != null)
		{
			theLight.enabled = !theLight.enabled;
		}
		else
		{
			Debug.LogWarning("No light found");
		}

		// Interaction always completes immediately
		return true;
	}
}
