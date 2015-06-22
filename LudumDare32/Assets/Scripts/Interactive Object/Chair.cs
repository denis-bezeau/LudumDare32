using UnityEngine;
using System.Collections.Generic;

public class Chair : InteractiveObject
{
	protected List<Interaction> _interactions = new List<Interaction>
	{
		new SitOn()
	};

	public override List<Interaction> GetInteractions ()
	{
		return _interactions;
	}
}

public class SitOn : Interaction
{
	private const float DEFAULT_SIT_DURATION = 2.0f;
	private float _sitDuration = DEFAULT_SIT_DURATION;
	private float _sitTimer = 0.0f;
	private Vector3 _sitSpot = Vector3.zero;
	private PersonSpriteFacing _personSprite = null;

	public override bool Interact (InteractiveObject interObject, MentalStateControl mentalState)
	{
		if (_sitTimer == 0.0f)
		{
			_personSprite = mentalState.gameObject.GetComponent<PersonSpriteFacing>();
			if (_personSprite != null)
			{
				_personSprite.FlipVertical();
			}
			else
			{
				// We don't have a sprite, our interaction is done
				mentalState.LogWarning("Trying to sit, but we don't have a sprite");
				return true;
			}

			// Stop them right here
			mentalState.PersonBody.velocity = Vector3.zero;
			mentalState.gameObject.transform.position = interObject.transform.position;
		}

		_sitTimer += Time.deltaTime;

		// Once we've sat long enough, we are done
		if (_sitTimer > _sitDuration)
		{
			// TODO: Should Reset be a mandatory function?
			Reset();
			_personSprite.FlipVertical();
			return true;
		}
		
		return false;
	}

	private void Reset ()
	{
		_sitTimer = 0.0f;
	}
}