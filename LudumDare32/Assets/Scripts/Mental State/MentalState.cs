using UnityEngine;

namespace MentalStates
{
	public class MentalState
	{
		protected MentalStateControl _parentControl = null;
		protected MentalStateControl.Memory _memory = null;

		public MentalState (MentalStateControl msc)
		{
			_parentControl = msc;
			_memory = msc.ActiveMemory;
		}

		public virtual void Begin ()
		{

		}

		public virtual void UpdateState ()
		{

		}

		public virtual void End ()
		{

		}
	}
}