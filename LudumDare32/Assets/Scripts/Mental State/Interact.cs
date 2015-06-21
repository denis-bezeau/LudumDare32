using UnityEngine;
using System.Collections.Generic;

namespace MentalStates
{
	public class Interact : MentalState
	{
		Interaction _currentInteraction = null; 
		InteractiveObject _targetObject = null;
		
		public Interact (MentalStateControl msc) : base (msc)
		{
		}
		
		public override void Begin ()
		{
			base.Begin();

			// Error checking
			_targetObject = _memory.TargetObject.GetComponent<InteractiveObject>();

			if (_targetObject == null)
			{
				_parentControl.LogWarning("No interactiveObject found on " + _memory.TargetObject.name);
				_parentControl.ChangeMentalState<Idle>();
				return;
			}

			_currentInteraction = ChooseInteraction();

			if (_currentInteraction == null)
			{
				_parentControl.LogWarning("No interaction found on " + _targetObject.name);
				_parentControl.ChangeMentalState<Idle>();
				return;
			}

			// We have success, set it as being used
			_targetObject.BeginInteracting();

			_parentControl.LogMessage("Chose to " + _currentInteraction.ToString() + " on " + _targetObject.name);
		}
		
		public override void UpdateState ()
		{
			bool isInteractionComplete = _currentInteraction.Interact(_targetObject);

			if (isInteractionComplete)
			{
				_targetObject.FinishInteracting();
				_parentControl.ChangeMentalState<Idle>();
			}
		}

		private Interaction ChooseInteraction ()
		{
			List<Interaction> possibleInteractions = _targetObject.GetInteractions();
			if (possibleInteractions.Count > 0)
			{
				// TODO: Choose an interaction correctly
				return possibleInteractions[0];
			}

			return null;
		}
	}
}