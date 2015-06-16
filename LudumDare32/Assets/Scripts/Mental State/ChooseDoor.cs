
using UnityEngine;

namespace MentalStates
{
	public class ChooseDoor : MentalState
	{
		public ChooseDoor (MentalStateControl msc) : base(msc)
		{

		}

		public override void Begin ()
		{
			base.Begin ();
			// If we aren't in a room revert to an idle and hopefully we'll have one next time
			if (_memory.CurrentRoom == null)
			{
				Debug.LogWarning (_parentControl.name + " doesn't have a current room.");
				_parentControl.ChangeMentalState<Idle> ();
				return;
			}
			
			if (_memory.CurrentRoom.Doors.Count == 0)
			{
				Debug.LogError ("Current room " + _memory.CurrentRoom.name + " doesn't have any doors?");
				_parentControl.ChangeMentalState<Idle> ();
			}
		}

		public override void UpdateState ()
		{
			base.UpdateState ();

			Door nextDoor = ChooseNextDoor ();

			if (nextDoor != null)
			{
				_parentControl.SetTargetDoor (nextDoor);
				_parentControl.ChangeMentalState<MoveToTarget> ();
			}
			else
			{
				Debug.LogError ("We didn't Choose a door.");
			}
		}

		private Door ChooseNextDoor ()
		{
			Door chosenDoor = null;
			Room currentRoom = _memory.CurrentRoom;

			if ((currentRoom.Doors != null) && (currentRoom.Doors.Count > 0))
			{
				int doorIdx = 0;

				// Choose which door to walk to
				switch (_parentControl.Personality)
				{
					case MentalStateControl.PersonalityType.EXPLORER:
						{
							// Explorers pick a random door
							doorIdx = UnityEngine.Random.Range (0, currentRoom.Doors.Count);
							if (doorIdx >= currentRoom.Doors.Count)
								doorIdx = currentRoom.Doors.Count - 1;

							break;
						}
					case MentalStateControl.PersonalityType.AGGRESSIVE:
						{
							// Pick the first door (the one that gets us towards the exit fastest)
							doorIdx = 0;
							break;
						}
					case MentalStateControl.PersonalityType.SLOW_AND_STEADY:
						{
							// Sure, why not
							doorIdx = currentRoom.Doors.Count - 1;
							break;
						}
				}

				chosenDoor = currentRoom.Doors [doorIdx];
				
				// If this is the door we came through then try another one, if there are any
				if (chosenDoor == _memory.RecentDoor && (currentRoom.Doors.Count > 1))
				{
					doorIdx++;
					if (doorIdx >= currentRoom.Doors.Count)
						doorIdx = 0;
					chosenDoor = currentRoom.Doors [doorIdx];
				}
			}

			Debug.Log ("Chosen Door: " + chosenDoor.ToString ());
			return chosenDoor;
		}
	}
}
