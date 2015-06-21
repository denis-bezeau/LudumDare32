using UnityEngine;

public class Lamp : InteractiveObject
{
	[SerializeField]
	Light
		_theLight = null;

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

	public override bool Interact (InteractiveObject interObject)
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
