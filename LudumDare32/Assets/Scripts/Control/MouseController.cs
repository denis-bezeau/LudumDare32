using UnityEngine;
using System.Collections;

/// <summary>
/// Mouse controller. @JamesP Will Flesh this out tomorrow morning! 
/// </summary>
public class MouseController : MonoBehaviour 
{
	void Update() 
	{
		if(Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray mousePosRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if(Physics.Raycast(mousePosRay, out hit))
			{
				// Send events for stuff.
			}
		}
		if(Input.GetMouseButtonDown(1))
		{
			// Do something else.
		}
		if(Input.GetMouseButtonDown(2))
		{
			// yeah.
		}
	}
}
