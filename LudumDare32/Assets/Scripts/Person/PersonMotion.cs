//////////////////////////////////////////////////////////////////////////
/// @file	PersonMotion.cs
///
/// @author	Sean Turner (ST)
///
/// @brief	Move a person object from A to B
//////////////////////////////////////////////////////////////////////////

/************************ EXTERNAL NAMESPACES ***************************/

using UnityEngine;																// Unity 			(ref http://docs.unity3d.com/Documentation/ScriptReference/index.html)
using System;																	// String / Math 	(ref http://msdn.microsoft.com/en-us/library/system.aspx)
using System.Collections;														// Queue 			(ref http://msdn.microsoft.com/en-us/library/system.collections.aspx)
using System.Collections.Generic;												// List<> 			(ref http://msdn.microsoft.com/en-us/library/system.collections.generic.aspx)

/************************ REQUIRED COMPONENTS ***************************/

/************************** THE SCRIPT CLASS ****************************/

public class PersonMotion : MonoBehaviour
{
	/****************************** CONSTANTS *******************************/
	
	public enum EMotionState
	{
		k_none			= 0,
		k_waitForTarget,
		k_walkToTarget,
		k_backOffForTime,
		k_dead
	}

	private const float 			k_maxWaitTime 		= 2.0f;
	private const float				k_minDistance		= 0.5f;
	private const float 			k_obstructedTime 	= 2.0f;
	private const float 			k_minObstructedVel 	= 0.5f;

	/***************************** SUB-CLASSES ******************************/
	
	/***************************** GLOBAL DATA ******************************/
	
	/*************************** GLOBAL METHODS *****************************/
	
	/***************************** PUBLIC DATA ******************************/
					
	public float					m_walkSpeed = 20.0f;								//!< Speed to walk
	public float					m_speedModifier = 1.0f;									//!< multiply this by m_walkSpeed

	/***************************** PRIVATE DATA *****************************/

	private Rigidbody				m_rigidBody;										//!< Our Rigidbody
	private SphereCollider			m_collider;											//!< Our SphereCollider
	private PersonAI				m_personAI;											//!< Our PersonAI script

	private EMotionState			m_motionState = EMotionState.k_waitForTarget;		//!< Motion FSM
	private	Vector3					m_targetPoint;										//!< Point to walk towards (probably a door)
	private float					m_backOffTime;										//!< Time to back off for
	private float					m_obstructionTimer = 0.0f;							//!< For timing how long we cant move

	/**************************** PUBLIC METHODS ****************************/

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Start walking towards this target point.
	//////////////////////////////////////////////////////////////////////////
	public void WalkToTarget(Vector3 target)
	{
		m_targetPoint = target;
		StateChange(EMotionState.k_walkToTarget);
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Have we reached our current target?
	//////////////////////////////////////////////////////////////////////////
	public bool HasReachedTarget()
	{
		if(m_motionState == EMotionState.k_waitForTarget)
			return true;

		return false;
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	We have died!
	//////////////////////////////////////////////////////////////////////////
	public void Die()
	{
		StateChange(EMotionState.k_dead);
	}
	
	//////////////////////////////////////////////////////////////////////////
	/// @brief	Are we unable to reach our target?
	//////////////////////////////////////////////////////////////////////////
	public bool IsObstructed()
	{
		if(m_motionState == EMotionState.k_backOffForTime)
			return true;
		
		return false;
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Are we stuck and unable to move? (eg. jammed in a wall)
	//////////////////////////////////////////////////////////////////////////
	public bool IsStuck()
	{
		if((m_motionState == EMotionState.k_backOffForTime) && (m_obstructionTimer > k_obstructedTime))
			return true;

		return false;
	}

	/*************************** PRIVATE METHODS ****************************/

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Constructor.
	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		// Find other components on the same game object
		m_rigidBody = GetComponent<Rigidbody>();
		m_personAI = GetComponent<PersonAI>();
		m_collider = GetComponent<SphereCollider>();

		// Set height so we're not colliding with the ground plane
		/*Vector3 startPos = transform.position;
		startPos.y = ((m_collider.radius - m_collider.center.y)*transform.localScale.y) + 0.5f;
		transform.position = startPos;*/

		// WALK TEST
		//WalkToTarget(new Vector3(-100.0f, 0.0f, -100.0f));
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Update motion FSM.
	//////////////////////////////////////////////////////////////////////////
	private void FixedUpdate()
	{
		switch(m_motionState)
		{
			case EMotionState.k_waitForTarget:
			{
			}
			break;

			case EMotionState.k_walkToTarget:
			{
				WalkToTarget();
			}
			break;

			case EMotionState.k_backOffForTime:
			{
				// Back away from target to allow others to get by
				WalkAwayFromTarget();

				// Once we've waited long enough start walking again
				if(Time.time >= m_backOffTime)
					StateChange(EMotionState.k_walkToTarget);
			}
			break;

			case EMotionState.k_dead:
			{
			}
			break;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Change motion FSM state.
	//////////////////////////////////////////////////////////////////////////
	private void StateChange(EMotionState newState)
	{
		switch(newState)
		{
			case EMotionState.k_waitForTarget:
			{
				// Keep momentum as we walk through doors? 
				// Yes = helps get through door and not get stuck on wall, No = stops us over shooting doors when walking at an angle to them
				//m_rigidBody.velocity = Vector3.zero;
			}
			break;

			case EMotionState.k_walkToTarget:
			{
			}
			break;
				
			case EMotionState.k_backOffForTime:
			{
				// Back up for a random amount of time
				m_rigidBody.velocity = Vector3.zero;
				m_obstructionTimer = 0.0f;
				m_backOffTime = Time.time + (UnityEngine.Random.value * k_maxWaitTime);
			}
			break;
				
			case EMotionState.k_dead:
			{
				m_rigidBody.velocity = Vector3.zero;
			}
			break;
		}
		
		m_motionState = newState;
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Update WalkToTarget state.
	//////////////////////////////////////////////////////////////////////////
	private void WalkToTarget()
	{
		Vector3 delta = m_targetPoint - transform.position;
		delta.z = 0.0f;
		float dist = delta.magnitude;

		// Have we arrived at the door?
		if(dist < (m_collider.radius*2.0f*transform.localScale.x))//k_minDistance)
		{
			// Yes - stop and wait for another target
			StateChange(EMotionState.k_waitForTarget);

			// Tell PersonAI we've reached our target point
			//m_personAI.ReachedTarget();
		}
		else
		{
			// No - keep moving towards our target
			m_rigidBody.AddForce(delta.normalized * m_walkSpeed * m_speedModifier, ForceMode.Force);

			// If we're not moving (eg. if colliding with a wall or another person) then bump the obstruction timer
			if(m_rigidBody.velocity.magnitude < k_minObstructedVel)
				m_obstructionTimer += Time.deltaTime;
			else
				m_obstructionTimer = 0.0f;

			// If we're obstructed then back off for a while
			if(m_obstructionTimer > k_obstructedTime)
				StateChange(EMotionState.k_backOffForTime);
		}
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Walk away from target to allow others to get by.
	//////////////////////////////////////////////////////////////////////////
	private void WalkAwayFromTarget()
	{
		Vector3 delta = transform.position - m_targetPoint;
		delta.z = 0.0f;

		m_rigidBody.AddForce(delta.normalized * m_walkSpeed * m_speedModifier, ForceMode.Force);

		// If we're not moving (eg. if colliding with a wall or another person) then bump the obstruction timer
		if(m_rigidBody.velocity.magnitude < k_minObstructedVel)
			m_obstructionTimer += Time.deltaTime;
		else
			m_obstructionTimer = 0.0f;
	}
}



