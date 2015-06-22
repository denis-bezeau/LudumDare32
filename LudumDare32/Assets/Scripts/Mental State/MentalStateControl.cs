//#define PRINT_MENTAL_STATE
using UnityEngine;

/// <summary>
/// Controller for the mental state of one person in the game
/// </summary>
using System.Collections.Generic;


public class MentalStateControl : MonoBehaviour
{
	public enum PersonalityType
	{
		AGGRESSIVE,
		EXPLORER,
		SLOW_AND_STEADY
	}


	public Rigidbody PersonBody;
	public SphereCollider PersonCollider;

	[SerializeField]
	private Memory
		_activeMemory = null;
	private PersonalityType _personality = PersonalityType.EXPLORER; // TODO: for now


	#region Properties
	public Memory ActiveMemory {
		get{ return _activeMemory;}
	}
	
	public PersonalityType Personality {
		get{ return _personality;}
	}
	#endregion

	#region Monobehaviour Stuff
	private void Awake ()
	{

		//Room startingRoom = FindObjectOfType (typeof(SpawnRoom)) as SpawnRoom;
		_activeMemory = new Memory();
		//_activeMemory.CurrentRoom = startingRoom;
		ChangeMentalState<MentalStates.Idle>();

		//PersonBody = GetComponent<Rigidbody> ();
		//PersonCollider = GetComponent<SphereCollider> ();
	}

	private void Update ()
	{
		CheckMentalState();

		// Don't do much here other than make sure the current state is executing correctly
		if (_activeMemory.CurrentState != null)
		{
			_activeMemory.CurrentState.UpdateState();
		}
		else
		{
			Debug.LogError("We've lost the current state somehow! Shit.");
			ChangeMentalState<MentalStates.Idle>();
		}
	}

	private void OnDrawGizmosSelected ()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(transform.position, _activeMemory.TargetPosition);
		}
	}
	#endregion

	public void ChangeMentalState<T> () where T : MentalStates.MentalState
	{
		if (_activeMemory.CurrentState != null)
		{
			_activeMemory.CurrentState.End();
		}

		_activeMemory.PreviousState = _activeMemory.CurrentState;

		try
		{
			_activeMemory.CurrentState = (T)System.Activator.CreateInstance(typeof(T), new object[] { this });
		}
		catch (System.Exception e)
		{
			Debug.LogError("Problem with creating new MentalState: " + e.Message);

			// Default to Idle for now
			_activeMemory.CurrentState = new MentalStates.Idle(this);
		}

		// TODO: We need to restrict these or print them another way
		LogMessage("Changing state from " + _activeMemory.PreviousState + " to " + _activeMemory.CurrentState);

		_activeMemory.CurrentState.Begin();
	}

	/// <summary>
	/// Call when a person enters a room, so they know where they are
	/// </summary>
	/// <param name="newRoom">The room they entered</param>
	public void EnterRoom (Room newRoom)
	{
		if (newRoom == _activeMemory.CurrentRoom)
		{
			LogWarning(this.name + " is entering the room they are already in?");
		}

		if (_activeMemory.CurrentRoom != null && !_activeMemory.CurrentRoom.IsConnectedToRoom(newRoom))
		{
			Debug.LogError(this.name + " is moving between rooms that aren't connected! " + _activeMemory.CurrentRoom + " and " + newRoom);
		}

		LogMessage(this.name + " is entering " + newRoom);
		_activeMemory.CurrentRoom = newRoom;
	}

	/// <summary>
	/// Call when a person leaves a room
	/// </summary>
	/// <param name="oldRoom">The room they left</param>
	public void ExitRoom (Room oldRoom)
	{
		if (oldRoom == _activeMemory.CurrentRoom)
		{
			// We should have entered a new room by now
			// We probably got pushed back into the old one.
			_activeMemory.CurrentRoom = _activeMemory.PreviousRoom;
		}

		_activeMemory.PreviousRoom = oldRoom;
	}

	public void SetTargetDoor (Door newDoor)
	{
		Room nextRoom = newDoor.GetOtherRoom(_activeMemory.CurrentRoom);

		if (nextRoom == null)
			LogWarning("Um, " + newDoor.name + " doesn't have connection to current room?");

		_activeMemory.NextRoom = nextRoom;

		SetNewTarget(newDoor.gameObject);
	}

	public void SetNewTarget (GameObject targetObj)
	{
		_activeMemory.TargetObject = targetObj;
		_activeMemory.TargetPosition = targetObj.transform.position;
	}

	public void SetNewTarget (Vector3 targetPos)
	{
		_activeMemory.TargetObject = null;
		_activeMemory.TargetPosition = targetPos;
	}

	/// <summary>
	/// Error check any information we want to be available
	/// </summary>
	private void CheckMentalState ()
	{

		// Verify the current room and fix if in an error state
		if (_activeMemory.CurrentRoom == null || !_activeMemory.CurrentRoom.IsPersonInRoom(this))
		{
			LogMessage("Resolving CurrentRoom discrepancy: " + _activeMemory.CurrentRoom);

			Room[] allRooms = FindObjectsOfType<Room>();
			if (allRooms != null && allRooms.Length > 0)
			{
				foreach (Room r in allRooms)
				{
					if (!r.gameObject.activeInHierarchy)
						continue;

					if (r.IsPersonInRoom(this))
					{
						_activeMemory.CurrentRoom = r;
						break;
					}
				}

				if (_activeMemory.CurrentRoom == null)
				{
					Debug.LogError(this.name + " couldn't be found in any room!");
				}
			}
			else
			{
				Debug.LogError("No rooms found?");
			}
		}
	}

	#region Debug Functions


	[System.Diagnostics.Conditional("PRINT_MENTAL_STATE")]
	public void LogMessage (string message)
	{
		string logString = "MSC - " + this.name + " - " + _activeMemory.CurrentState.ToString() + ": " + message; 
		Debug.Log(logString);
	}

	[System.Diagnostics.Conditional("PRINT_MENTAL_STATE")]
	public void LogWarning (string warning)
	{
		string warningString = "MSC - " + this.name + " - " + _activeMemory.CurrentState.ToString() + ": " + warning; 
		Debug.Log(warningString);
	}
	#endregion

	/// <summary>
	/// A collection of information the person knows
	/// </summary>
	public class Memory
	{
		public MentalStates.MentalState CurrentState = null;
		public MentalStates.MentalState PreviousState = null;

		public Room NextRoom = null; // The next room we'll be going to
		public Room CurrentRoom = null;
		public Room PreviousRoom = null;

		public Vector3 TargetPosition = Vector3.zero;
		public GameObject TargetObject = null;

		public Door RecentDoor = null; // The most recent door we've come through
	}
}
