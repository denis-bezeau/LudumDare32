﻿using UnityEngine;
using System.Collections;
using System;

public class AnimBase : StateMachineBehaviour 
{
	[SerializeField] private eAnimTypes _animType;

	public Action<eAnimTypes> OnAnimFinished = delegate {};
	public Action<eAnimTypes> OnAnimStarted = delegate {};
	public Action<eAnimTypes> OnAnimUpdate = delegate {};
	public Action<eAnimTypes> OnAnimMoved = delegate {};
	public Action<eAnimTypes> OnAnimIK = delegate {};

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		OnAnimStarted(_animType);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (_animType == eAnimTypes.Idle)
		{
			//check parameter
			animator.SetInteger("adsgad", 1);
		}

		OnAnimUpdate(_animType);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		OnAnimFinished(_animType);
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		OnAnimMoved(_animType);
	}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		OnAnimIK(_animType);
	}
}
