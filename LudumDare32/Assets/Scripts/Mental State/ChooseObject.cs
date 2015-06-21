

using UnityEngine;
using System.Collections.Generic;

namespace MentalStates
{
	public class ChooseObject : MentalState
	{
		public ChooseObject (MentalStateControl msc) : base(msc)
		{
			
		}
		
		public override void Begin ()
		{
			base.Begin();
			// If we aren't in a room revert to an idle and hopefully we'll have one next time
			if (_memory.CurrentRoom == null)
			{
				_parentControl.LogWarning(_parentControl.name + " doesn't have a current room.");
				_parentControl.ChangeMentalState<Idle>();
				return;
			}
			
			if (_memory.CurrentRoom.Doors.Count == 0)
			{
				Debug.LogError("Current room " + _memory.CurrentRoom.name + " doesn't have any doors?");
				_parentControl.ChangeMentalState<Idle>();
			}
		}
		
		public override void UpdateState ()
		{
			base.UpdateState();
			
			InteractiveObject chosenObject = ChooseNextObject();
			
			if (chosenObject != null)
			{
				_parentControl.SetNewTarget(chosenObject.gameObject);
				_parentControl.ChangeMentalState<MoveToTarget>();
			}
		}
		
		private InteractiveObject ChooseNextObject ()
		{
			InteractiveObject chosenObject = null;
			Room currentRoom = _memory.CurrentRoom;

			List<InteractiveObject> availableObjects = currentRoom.GetAvailableInteractiveObjects();

			if (availableObjects.Count > 0)
			{
				int objIdx = 0;
				
				// TODO: Choose which object to walk to
				chosenObject = availableObjects[objIdx];
			}
			
			_parentControl.LogMessage("Chosen Object: " + chosenObject + " (" + availableObjects.Count + " available)");
			return chosenObject;
		}
	}
}
