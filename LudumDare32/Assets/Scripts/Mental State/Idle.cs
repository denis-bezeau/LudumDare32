using UnityEngine;

namespace MentalStates
{
	/// <summary>
	/// Idle state is our safety. Return to this if we find ourself in an erronous
	/// situation. From here the state machine should be able to choose a new
	/// state and correctly carry on.
	/// </summary>
	public class Idle : MentalState
	{
		private const float DEFAULT_IDLE_DURATION = 0.5f;
		private float _idleTime = 0.0f;

		public Idle (MentalStateControl msc) : base (msc)
		{
		}

		public override void UpdateState ()
		{
			_idleTime += UnityEngine.Time.deltaTime;
			if (_idleTime > DEFAULT_IDLE_DURATION)
			{
				// TODO: Do something better than this
				if (Random.value > 0.5 && _memory.CurrentRoom.HasObjects)
				{
					_parentControl.ChangeMentalState<MentalStates.ChooseObject>();
				}
				else
				{
					_parentControl.ChangeMentalState<MentalStates.ChooseDoor>();
				}
			}
		}
	}
}