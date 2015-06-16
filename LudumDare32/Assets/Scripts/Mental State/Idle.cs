
namespace MentalStates
{
	public class Idle : MentalState
	{
		private const float DEFAULT_IDLE_DURATION = 2.0f;
		private float _idleTime = 0.0f;

		public Idle (MentalStateControl msc) : base (msc)
		{
		}

		public override void Begin ()
		{
			base.Begin ();
		}

		public override void UpdateState ()
		{
			_idleTime += UnityEngine.Time.deltaTime;
			if (_idleTime > DEFAULT_IDLE_DURATION)
			{
				_parentControl.ChangeMentalState<MentalStates.ChooseDoor> ();
			}
		}
	}
}