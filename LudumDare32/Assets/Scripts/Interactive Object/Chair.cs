using UnityEngine;
using System.Collections.Generic;

public class Chair : InteractiveObject
{
	private void Awake ()
	{
		_interactions = new List<Interaction>
		{
			new SitOn(gameObject.transform.position)
		};
	}
}

public class SitOn : Interaction
{
	private const float DEFAULT_SIT_DURATION = 2.0f;

	private float _sitDuration = 0.0f;
	private float _sitTimer = 0.0f;
	private Vector3 _sitSpot = Vector3.zero;
	private PersonSpriteFacing _personSprite = null;

	public SitOn (Vector3 sitPosition, float duration = DEFAULT_SIT_DURATION)
	{
		_sitSpot = sitPosition;
		_sitDuration = duration;
	}

	public override bool Interact (InteractiveObject interObject, MentalStateControl mentalState)
	{
		if (_sitTimer == 0.0f)
		{
			_personSprite = mentalState.gameObject.GetComponent<PersonSpriteFacing>();
			if (_personSprite != null)
			{
				// Yeah, we flip them for now
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
			mentalState.gameObject.transform.position = _sitSpot;
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