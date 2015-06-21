using UnityEngine;

namespace MentalStates
{
	/// <summary>
	/// State where we determine where in the room we want to go
	/// </summary>
	public class MoveIntoRoom : MentalState
	{

		public MoveIntoRoom (MentalStateControl msc) : base (msc)
		{

		}

		public override void Begin ()
		{
			base.Begin();

			if (_memory.CurrentRoom == null)
			{
				_parentControl.LogWarning("Why aren't we in a room");
			}
		}

		public override void UpdateState ()
		{
			base.UpdateState();

			Vector3 roomPoint = FindRoomPoint();

			_parentControl.SetNewTarget(roomPoint);
			_parentControl.ChangeMentalState<MoveToTarget>();
		}

		private Vector3 FindRoomPoint ()
		{
			// TODO: Add more variety
			Vector3 chosenPoint = Vector3.zero;

			Room roomToChooseFrom = null;

			if (_memory.NextRoom != null)
			{
				roomToChooseFrom = _memory.NextRoom;
			}
			else
			{
				roomToChooseFrom = _memory.CurrentRoom;
			}
			chosenPoint = roomToChooseFrom.GetRoomCenter();

			_parentControl.LogMessage("MoveIntoRoom Chose point in " + roomToChooseFrom.name + " " + chosenPoint);
			return chosenPoint;
		}
	}
}