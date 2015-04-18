//////////////////////////////////////////////////////////////////////////
/// @file	PersonAI.cs
///
/// @author	Sean Turner (ST)
///
/// @brief	Target doors to walk from room to room towards the exit
//////////////////////////////////////////////////////////////////////////

/************************ EXTERNAL NAMESPACES ***************************/

using UnityEngine;																// Unity 			(ref http://docs.unity3d.com/Documentation/ScriptReference/index.html)
using System;																	// String / Math 	(ref http://msdn.microsoft.com/en-us/library/system.aspx)
using System.Collections;														// Queue 			(ref http://msdn.microsoft.com/en-us/library/system.collections.aspx)
using System.Collections.Generic;												// List<> 			(ref http://msdn.microsoft.com/en-us/library/system.collections.generic.aspx)

/************************ REQUIRED COMPONENTS ***************************/

/************************** THE SCRIPT CLASS ****************************/

public class PersonAI : MonoBehaviour
{
	/****************************** CONSTANTS *******************************/

	public enum EPersonality
	{
		k_aggressive,
		k_explorer,
		k_slowAndSteady
	}

	public enum EAIState
	{
		k_none			= 0,
		k_chooseADoor,
		k_walkToDoor,
		k_bashDownDoor,
		k_dead
	}

	private const float				k_bashIntervalNormal		= 1.0f;					//!< Seconds between each door bash
	private const float				k_bashIntervalAggressive	= 0.5f;					//!< Seconds between each door bash

	/***************************** SUB-CLASSES ******************************/
	
	/***************************** GLOBAL DATA ******************************/
	
	/*************************** GLOBAL METHODS *****************************/
	
	/***************************** PUBLIC DATA ******************************/
	
	public EPersonality				m_personality = EPersonality.k_aggressive;			//!< AI type
	
	/***************************** PRIVATE DATA *****************************/

	private PersonMotion			m_personMotion;										//!< Our PersonMotion script
	
	private EAIState				m_aiState = EAIState.k_chooseADoor;					//!< AI FSM

	private Door					m_targetDoor = null;								//!< Current door
	private Room					m_currentRoom = null;								//!< Current room
	private float					m_nextDoorBashTime = 0.0f;							//!< Time to bash the door next
	
	/**************************** PUBLIC METHODS ****************************/
	
	//////////////////////////////////////////////////////////////////////////
	/// @brief	Render this person dead :)
	//////////////////////////////////////////////////////////////////////////
	public void Kill()
	{
		StateChange(EAIState.k_dead);
	}
	
	/*************************** PRIVATE METHODS ****************************/
	
	//////////////////////////////////////////////////////////////////////////
	/// @brief	Constructor.
	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		// Find other components on the same game object
		m_personMotion = GetComponent<PersonMotion>();
	}
	
	//////////////////////////////////////////////////////////////////////////
	/// @brief	Update AI FSM.
	//////////////////////////////////////////////////////////////////////////
	private void Update()
	{
		switch(m_aiState)
		{
			case EAIState.k_chooseADoor:
			{
			}
			break;
			
			case EAIState.k_walkToDoor:
			{
				// If we're not walking to the best door and our door is closed then switch to the best door?

				// When we reach the door see if we need to bash down the door to walk through it
				if(m_personMotion.HasReachedTarget())
				{
					StateChange(EAIState.k_bashDownDoor);
				}
			}
			break;
			
			case EAIState.k_bashDownDoor:
			{
				if(Time.time >= m_nextDoorBashTime)
				{
					if(m_targetDoor.IsOpen)
					{
						// Walk through door to other room
						m_currentRoom = m_targetDoor.GetOtherRoom(m_currentRoom);
						m_targetDoor = null;
						
						// Choose the next door in the new room to walk to
						StateChange(EAIState.k_chooseADoor);
					}
					else
					{
						// Do some damage to the door
						//m_targetDoor.Hit();
						m_nextDoorBashTime = Time.time + (m_personality==EPersonality.k_aggressive ? k_bashIntervalAggressive : k_bashIntervalNormal);
					}
				}
			}
			break;
			
			case EAIState.k_dead:
			{
			}
			break;
		}
	}
	
	//////////////////////////////////////////////////////////////////////////
	/// @brief	Change AI FSM state.
	//////////////////////////////////////////////////////////////////////////
	private void StateChange(EAIState newState)
	{
		switch(newState)
		{
			case EAIState.k_chooseADoor:
			{
				if(m_currentRoom != null)
				{
					// Choose which door to walk to
					//if(m_personality == EPersonality.k_explorer)
					//m_targetDoor = m_currentRoom.GetDoor

					if(m_targetDoor != null)
					{
						StateChange(EAIState.k_walkToDoor);
					}
				}
			}
			break;
			
			case EAIState.k_walkToDoor:
			{
				m_personMotion.WalkToTarget(m_targetDoor.transform.position);
			}
			break;
			
			case EAIState.k_bashDownDoor:
			{
				m_nextDoorBashTime = 0.0f;
			}
			break;
			
			case EAIState.k_dead:
			{
			}
			break;
		}
		
		m_aiState = newState;
	}
}



