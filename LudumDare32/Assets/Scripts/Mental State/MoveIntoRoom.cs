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
			base.Begin ();

			if (_memory.CurrentRoom == null)
			{
				Debug.LogWarning ("Why aren't we in a room");
			}
		}

		public override void UpdateState ()
		{
			base.UpdateState ();

			Vector3 roomPoint = FindRoomPoint ();

			_parentControl.SetNewTarget (roomPoint);
			_parentControl.ChangeMentalState<MoveToTarget> ();
		}

		private Vector3 FindRoomPoint ()
		{
			// TODO: Add more variety
			Vector3 chosenPoint = Vector3.zero;

			if (_memory.NextRoom != null)
			{
				Debug.Log ("Choosing point in " + _memory.NextRoom);
				chosenPoint = _memory.NextRoom.GetRoomCenter ();
			}
			else
			{
				Debug.Log ("Choosing point in " + _memory.CurrentRoom);
				chosenPoint = _memory.CurrentRoom.GetRoomCenter ();
			}
			Debug.Log ("Chosen target point: " + chosenPoint);
			return chosenPoint;
		}
	}
}