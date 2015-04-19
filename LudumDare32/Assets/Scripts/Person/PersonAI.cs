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

	private SpawnRoom				m_spawnRoom = null;									//!< Current level spawn room
	private EscapeRoom 				m_escapeRoom = null;								//!< Current level escape room

	private Door					m_targetDoor = null;								//!< Current door
	private Door					m_previousDoor = null;								//!< Previous door we came through
	private Room					m_currentRoom = null;								//!< Current room
	private float					m_nextDoorBashTime = 0.0f;							//!< Time to bash the door next
	private PersonStats				m_PersonStats = null;								//!< stats of the person e.g. health
	
	/**************************** PUBLIC METHODS ****************************/

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Enter the level in the spawn room
	//////////////////////////////////////////////////////////////////////////
	public void EnterLevel(SpawnRoom spawnRoom, EscapeRoom escapeRoom)
	{
		m_spawnRoom = spawnRoom;
		m_escapeRoom = escapeRoom;

		// Start in spawn room - look for a door
		m_currentRoom = m_spawnRoom;
		StateChange(EAIState.k_chooseADoor);
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Render this person dead :)
	//////////////////////////////////////////////////////////////////////////
	public void Kill()
	{
		KillEnemyEvent killEvent = new KillEnemyEvent();
		killEvent.count = 1;
		killEvent.enemy = this;
		CTEventManager.FireEvent(killEvent);

		StateChange(EAIState.k_dead);
	}

		//////////////////////////////////////////////////////////////////////////
	/// @brief	get gameplay stats about the person e.g. health
	//////////////////////////////////////////////////////////////////////////
	public PersonStats GetPlayerStats()
	{
		return m_PersonStats;
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
				LookForADoor();
			}
			break;
			
			case EAIState.k_walkToDoor:
			{
				// If we're not walking to the best door and our door is closed then switch to the best door?

				// When we reach the door see if we need to bash down the door to walk through it
				if(m_personMotion.HasReachedTarget())
				{
					// Is this the exit door?
					if(m_targetDoor.isExit)
					{
						EscapeEvent escapeEvent = new EscapeEvent();
						escapeEvent.enemy = this;
						CTEventManager.FireEvent(escapeEvent);

						StateChange(EAIState.k_none);
					}
					else
					{
						StateChange(EAIState.k_bashDownDoor);
					}
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
						m_previousDoor = m_targetDoor;
						m_targetDoor = null;
						
						// Choose the next door in the new room to walk to
						StateChange(EAIState.k_chooseADoor);
					}
					else
					{
						// Do some damage to the door
						m_targetDoor.Hit();
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

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Look around for a door to walk to.
	//////////////////////////////////////////////////////////////////////////
	private void LookForADoor()
	{
		if((m_currentRoom != null) && (m_currentRoom.Doors != null) && (m_currentRoom.Doors.Count > 0))
		{
			int doorIndex = 0;
			
			// Choose which door to walk to
			if(m_personality == EPersonality.k_explorer)
			{
				// Explorers pick a random door
				doorIndex = (int)(UnityEngine.Random.value*m_currentRoom.Doors.Count);
				if(doorIndex >= m_currentRoom.Doors.Count)
					doorIndex = m_currentRoom.Doors.Count-1;
				m_targetDoor = m_currentRoom.Doors[doorIndex];
			}
			else
			{
				// Pick the first door (the one that gets us towards the exit fastest)
				doorIndex = 0;
			}
			
			m_targetDoor = m_currentRoom.Doors[doorIndex];
			
			// If this is the door we came through then try another one
			if((m_targetDoor == m_previousDoor) && (m_currentRoom.Doors.Count > 1))
			{
				doorIndex++;
				if(doorIndex >= m_currentRoom.Doors.Count)
					doorIndex = 0;
				m_targetDoor = m_currentRoom.Doors[doorIndex];
			}
			
			if(m_targetDoor != null)
			{
				StateChange(EAIState.k_walkToDoor);
			}
		}
	}
}



