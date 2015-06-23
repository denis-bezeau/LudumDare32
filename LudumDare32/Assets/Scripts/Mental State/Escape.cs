using System.Collections;
using UnityEngine;

namespace MentalStates
{
	/// <summary>
	/// Idle state is our safety. Return to this if we find ourself in an erronous
	/// situation. From here the state machine should be able to choose a new
	/// state and correctly carry on.
	/// </summary>
	public class Escape : MentalState
	{
		private const float DEFAULT_IDLE_DURATION = 0.5f;
		private float _idleTime = 0.0f;
		private SpriteRenderer _sprite = null;
		
		public Escape (MentalStateControl msc) : base (msc)
		{
			if (_sprite == null)
			{
				_sprite = _parentControl.GetComponentInChildren<SpriteRenderer>();
			}
		}
		
		public override void UpdateState ()
		{
			if (FadeOut())
			{
				// TODO: When complete, do something?
				// - fire escape event
				// - remove person
				// - finish updates
			}
		}

		// TODO: This should probably be somewhere else
		private bool FadeOut ()
		{
			if (_sprite != null)
			{
				if (_sprite.color.a <= 0)
				{
					return true;
				}

				Color temp = _sprite.color;
				temp.a -= .1f;
				_sprite.color = temp; 
			}
			else
			{
				_parentControl.LogWarning("We didn't have a sprite, just escape...");
				return true;
			}

			return false;
		}
	}
}
