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
		k_walkToCenter,
		k_bashDownDoor,
		k_escaped,
		k_dead
	}

	private const float				k_bashIntervalNormal = 1.0f;					//!< Seconds between each door bash
	private const float				k_bashIntervalAggressive = 0.5f;					//!< Seconds between each door bash

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
	private Room					m_currentRoom = null;								//!< Room we walked through a door in to
	private List<Room>				m_physicalRooms = new List<Room> ();					//!< Rooms we are physically touching
	private float					m_nextDoorBashTime = 0.0f;							//!< Time to bash the door next
	private PersonStats				m_PersonStats = null;								//!< stats of the person e.g. health

	[SerializeField]
	private Animator
		m_animator;

	[SerializeField]
	private SphereCollider
		m_collider;
	
	/**************************** PUBLIC METHODS ****************************/

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Enter the level in the spawn room
	//////////////////////////////////////////////////////////////////////////
	public void EnterLevel (SpawnRoom spawnRoom, EscapeRoom escapeRoom)
	{
		m_spawnRoom = spawnRoom;
		m_escapeRoom = escapeRoom;

		// Start in spawn room - look for a door
		m_currentRoom = m_spawnRoom;
		StateChange (EAIState.k_chooseADoor);

		CTEventManager.FireEvent (new PlaySFXEvent () { assetName = m_PersonStats.GetRandomSound(PersonStats.SOUND_EFFECT.SPAWN)});
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Render this person dead :)
	//////////////////////////////////////////////////////////////////////////
	public void Kill ()
	{
		if (m_aiState == EAIState.k_dead)
			return;

		Debug.Log ("kill");
		KillEnemyEvent killEvent = new KillEnemyEvent ();
		killEvent.count = 1;
		killEvent.enemy = this;
		CTEventManager.FireEvent (killEvent);
		CTEventManager.FireEvent (new PlaySFXEvent () { assetName = m_PersonStats.GetRandomSound(PersonStats.SOUND_EFFECT.DEATH)});

		StateChange (EAIState.k_dead);
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	get gameplay stats about the person e.g. health
	//////////////////////////////////////////////////////////////////////////
	public PersonStats GetPlayerStats ()
	{
		return m_PersonStats;
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	sets gameplay stats about the person e.g. health
	//////////////////////////////////////////////////////////////////////////
	public void SetPlayerStats (PersonStats newStats)
	{
		m_PersonStats = newStats;
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Add this room to the list we are physically touching.
	//////////////////////////////////////////////////////////////////////////
	public void EnterRoomPhysically (Room newRoom)
	{
		if (!m_physicalRooms.Contains (newRoom))
			m_physicalRooms.Add (newRoom);

		m_currentRoom = newRoom;

		if (m_currentRoom is EscapeRoom)
		{
			StateChange (EAIState.k_escaped);
		}
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Remove this room from the list we are physically touching.
	//////////////////////////////////////////////////////////////////////////
	public void LeaveRoomPhysically (Room oldRoom)
	{
		if (m_physicalRooms.Contains (oldRoom))
			m_physicalRooms.Remove (oldRoom);
	}

	/*************************** PRIVATE METHODS ****************************/
	
	//////////////////////////////////////////////////////////////////////////
	/// @brief	Constructor.
	//////////////////////////////////////////////////////////////////////////
	private void Awake ()
	{
		// Find other components on the same game object
		m_personMotion = GetComponent<PersonMotion> ();
		m_PersonStats = GetComponent<PersonStats> ();
	}
	
	//////////////////////////////////////////////////////////////////////////
	/// @brief	Update AI FSM.
	//////////////////////////////////////////////////////////////////////////
	private void Update ()
	{
		switch (m_aiState)
		{
			case EAIState.k_chooseADoor:
				{
					LookForADoor ();
				}
				break;
			case EAIState.k_walkToCenter:
				{
					if (m_personMotion.HasReachedTarget ())
					{
						Debug.Log ("Has reached target");
						StateChange (EAIState.k_chooseADoor);
					}
					else if (m_personMotion.IsObstructed () && !m_physicalRooms.Contains (m_currentRoom))
					{
						m_currentRoom = m_physicalRooms [0];
						StateChange (EAIState.k_walkToCenter);
					}
					break;
				}
			case EAIState.k_walkToDoor:
				{
					// If we're not walking to the best door and our door is closed then switch to the best door?

					// When we reach the door see if we need to bash down the door to walk through it
					if (m_personMotion.HasReachedTarget ())
					{
						StateChange (EAIState.k_bashDownDoor);
					}
					else
				// If we're obstructed because we've been physically pushed out of the room then retarget a door from a room we are physically in
				if (m_personMotion.IsObstructed () && !m_physicalRooms.Contains (m_currentRoom))
					{
						if (m_physicalRooms != null && m_physicalRooms.Count > 0)
						{
							m_currentRoom = m_physicalRooms [0];
							StateChange (EAIState.k_chooseADoor);
						}
						StateChange (EAIState.k_chooseADoor);	
					}
					else
				// If we're stuck in a wall then kill us
				if (m_personMotion.IsStuck ())
					{
						Kill ();
					}
				}
				break;
			
			case EAIState.k_bashDownDoor:
				{
					if (Time.time >= m_nextDoorBashTime)
					{
						if (m_targetDoor.IsOpen)
						{
							// Walk through door to other room
							m_currentRoom = m_targetDoor.GetOtherRoom (m_currentRoom);
							m_previousDoor = m_targetDoor;
							m_targetDoor = null;
						
							// Have we reached the escape room?
							if (m_currentRoom == m_escapeRoom)
							{
								// ESCAPE!
								EscapeEvent escapeEvent = new EscapeEvent ();
								escapeEvent.enemy = this;
								CTEventManager.FireEvent (escapeEvent);
							
								StateChange (EAIState.k_escaped);
							}
							else
							{
								// Walk into room
								StateChange (EAIState.k_walkToCenter);
							}
						}
						else
						{
							// Do some damage to the door
							m_targetDoor.Hit ();
							m_nextDoorBashTime = Time.time + (m_personality == EPersonality.k_aggressive ? k_bashIntervalAggressive : k_bashIntervalNormal);
						}
					}
				}
				break;
			
			case EAIState.k_dead:
				{
					if (m_animator != null)
					{
						m_animator.SetBool ("death", true);
					}
					this.GetComponent<SphereCollider> ().enabled = false;
				}
				break;
		}
	}

	private void OnCollisionEnter (Collision collInfo)
	{
		if (collInfo.gameObject.layer == LayerMask.NameToLayer ("Walls"))
		{
			//CTEventManager.FireEvent (new PlaySFXEvent (){assetName = m_PersonStats.GetRandomSound(PersonStats.SOUND_EFFECT.HIT)});
		}
	}

	public void ChooseNewTarget ()
	{
		// try ROOM again
		m_personMotion.WalkToTarget (m_currentRoom.transform.position + m_currentRoom.RoomCollider.center);
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Change AI FSM state.
	//////////////////////////////////////////////////////////////////////////
	private void StateChange (EAIState newState)
	{
		switch (newState)
		{
			case EAIState.k_chooseADoor:
				{
				}
				break;
			case EAIState.k_walkToCenter:
				{
					Debug.Log (this.name + "Walking to center: " + m_currentRoom.name + " " + m_currentRoom.transform.position + m_currentRoom.RoomCollider.center);
					m_personMotion.WalkToTarget (m_currentRoom.transform.position + m_currentRoom.RoomCollider.center);
					break;
				}
			case EAIState.k_walkToDoor:
				{
					m_personMotion.WalkToTarget (m_targetDoor.transform.position);
				}
				break;
			
			case EAIState.k_bashDownDoor:
				{
					m_nextDoorBashTime = 0.0f;
				}
				break;
			
			case EAIState.k_dead:
				{
				
					m_personMotion.Die ();
				}
				break;
			case EAIState.k_escaped:
				{
					StartCoroutine (FadeOut ());
					m_collider.enabled = false;
				}
				break;
		}
		
		m_aiState = newState;
	}

	private IEnumerator FadeOut ()
	{
		SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer> ();

		while (sprite.color.a > 0)
		{
			Color temp = sprite.color;
			temp.a -= .1f;
			sprite.color = temp; 
			yield return null;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Look around for a door to walk to.
	//////////////////////////////////////////////////////////////////////////
	private void LookForADoor ()
	{
		if ((m_currentRoom != null) && (m_currentRoom.Doors != null) && (m_currentRoom.Doors.Count > 0))
		{
			int doorIndex = 0;
			
			// Choose which door to walk to
			if ((m_personality == EPersonality.k_aggressive) || (m_personality == EPersonality.k_explorer))
			{
				// Explorers pick a random door
				doorIndex = (int)(UnityEngine.Random.value * m_currentRoom.Doors.Count);
				if (doorIndex >= m_currentRoom.Doors.Count)
					doorIndex = m_currentRoom.Doors.Count - 1;
				m_targetDoor = m_currentRoom.Doors [doorIndex];
			}
			else
			{
				// Pick the first door (the one that gets us towards the exit fastest)
				doorIndex = 0;
			}
			
			m_targetDoor = m_currentRoom.Doors [doorIndex];
			
			// If this is the door we came through then try another one
			if (((m_targetDoor == m_previousDoor) || (m_targetDoor.GetOtherRoom (m_currentRoom) == m_spawnRoom)) && (m_currentRoom.Doors.Count > 1))
			{
				doorIndex++;
				if (doorIndex >= m_currentRoom.Doors.Count)
					doorIndex = 0;
				m_targetDoor = m_currentRoom.Doors [doorIndex];
			}
			
			if (m_targetDoor != null)
			{
				StateChange (EAIState.k_walkToDoor);
			}
		}
	}
}



