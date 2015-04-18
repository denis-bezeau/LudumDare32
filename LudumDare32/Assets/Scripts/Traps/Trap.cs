
using UnityEngine;
using System.Collections.Generic;

public class Trap : MonoBehaviour
{
	static string _title;
	public string title { get { return _title; } }

	static int _cost;
	public int cost { get { return _cost; } }

	/// <summary>
	/// What to do when a new person enters the room
	/// </summary>
	public void OnEnterTrap ()
	{
		
	}
	
	/// <summary>
	/// What to do when a person exits a room
	/// </summary>
	public void OnExitTrap ()
	{
		
	}
}