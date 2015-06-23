
using UnityEngine;

namespace MentalStates
{
	public class MoveToTarget : MentalState
	{
		private const float OBSTRUCTION_TIME_LIMIT = 2.0f;
		private const float MIN_OBSTRUCT_VALUE = 0.5f; // If we moving less than this, we're obstructed

		private float 	m_speedModifier = 1.0f;
		private float	m_walkSpeed = 3.0f;

		private float 	m_obstructionTimer = 0f;

		public MoveToTarget (MentalStateControl msc) : base (msc)
		{

		}

		public override void Begin ()
		{
			base.Begin();

			if (_memory.TargetObject == null && _memory.TargetPosition == null)
			{
				_parentControl.LogWarning("Trying to move without a target!");
				_parentControl.ChangeMentalState<Idle>();
				return;
			}

			if (_memory.TargetPosition == null && _memory.TargetObject != null)
			{
				_parentControl.LogWarning("No target position, but we have target object. This is incorrect, we'll try again.");
				_parentControl.SetNewTarget(_memory.TargetObject);
			}
		}

		public override void UpdateState ()
		{
			base.UpdateState();

			if (HaveArrived())
			{
				if (_memory.TargetObject == null)
				{
					// We were just going to a point, look for something new
					_parentControl.ChangeMentalState<Idle>();
				}
				else if (_memory.TargetObject.GetComponent<Door>() != null)
				{
					// If we've made it to a door, go inside
					_memory.RecentDoor = _memory.TargetObject.GetComponent<Door>();
					_parentControl.ChangeMentalState<MoveIntoRoom>();
				}
				else if (_memory.TargetObject.GetComponent<InteractiveObject>() != null)
				{
					_parentControl.ChangeMentalState<Interact>();
				}
				else
				{
					// We've come to another type of object, figure out what to do
				}
			}
			else if (AreWeObstructed())
			{
				// TODO: We'll probably want at least one random state and
				//       other possible states to move to depending on personality
				_parentControl.ChangeMentalState<Idle>();
			}
			else
			{
				// Keep on truckn'
				WalkToPosition();
			}
		}

		public bool IsMovingToPoint {
			get{ return _memory.TargetObject == null;}
		}

		public bool IsMovingToObject {
			get{ return _memory.TargetObject != null;}
		}

		private bool HaveArrived ()
		{
			Vector3 delta = _memory.TargetPosition - _parentControl.gameObject.transform.position;
			delta.z = 0.0f;
			float distanceToTarget = delta.magnitude;
			float closeEnough = _parentControl.PersonCollider.radius * 2.0f * _parentControl.gameObject.transform.localScale.x;

			// Have we arrived at the door?
			if (distanceToTarget < closeEnough)
			{
				return true;
			}

			return false;
		}

		private bool AreWeObstructed ()
		{
			// If we're not moving (eg. if colliding with a wall or another person) then bump the obstruction timer
			if (_parentControl.PersonBody.velocity.magnitude < MIN_OBSTRUCT_VALUE)
			{
				m_obstructionTimer += Time.deltaTime;
			}
			else
			{
				m_obstructionTimer = 0.0f;
			}

			// If we're obstructed then back off for a while
			return (m_obstructionTimer > OBSTRUCTION_TIME_LIMIT);
		}

		private void WalkToPosition ()
		{
			Vector3 delta = _memory.TargetPosition - _parentControl.gameObject.transform.position;
			delta.z = 0.0f;

			// No - keep moving towards our target
			_parentControl.PersonBody.AddForce(delta.normalized * m_walkSpeed * m_speedModifier, ForceMode.Force);
		}
	}
}