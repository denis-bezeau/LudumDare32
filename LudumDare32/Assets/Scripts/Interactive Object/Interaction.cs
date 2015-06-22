using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// An interaction occurs between a character and an object. This
/// class and it's children define the requirements and benefits of
/// choosing to perform this interaction.
/// 
/// It is the job of the object to decide how this interaction affects
/// both it and the interacting character, in order to fulfill the benefits.
/// </summary>
public abstract class Interaction
{
	List<InteractionBenefit> _benefits;

	// TODO: An interation may have requirements (be holding an object, npc of certain type, etc)
	//public abstract bool CheckRequirements ();

	// TODO: Should there be other overloads for this?
	public abstract bool Interact (InteractiveObject interObject, MentalStateControl mentalState);

}

/// <summary>
/// Benefits that an interaction can bestow.
/// </summary>
public enum InteractionBenefit
{
	INVALID = -1,
	HEALTH,
	SANITY,
	LIGHT,
	HEAT,
	NUM
}

