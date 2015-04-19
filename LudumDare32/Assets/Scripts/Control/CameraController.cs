using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	[SerializeField]
	private float _mouseSensitivity = 0.01f;
	private Vector3 _lastPosition;

	// Constraints
	[SerializeField]
	private LevelAnchor _levelBounds; 
	
	void Update()
	{
		Vector3 screenPos = _levelBounds.gameObject.transform.InverseTransformPoint(Input.mousePosition);
		screenPos = Camera.main.ScreenToWorldPoint(screenPos);

		if (Input.GetMouseButtonDown(1))
		{
			if(_levelBounds.GetBounds.Contains(screenPos))
			{
				_lastPosition = screenPos;
			}
		}
		
		if (Input.GetMouseButton(1))
		{
			if(_levelBounds.GetBounds.Contains(screenPos))
			{
				Vector3 delta = screenPos - _lastPosition;
				this.transform.Translate(delta.x * _mouseSensitivity, delta.y * _mouseSensitivity, 0.0f);
				_lastPosition = screenPos;
			}
		}
	}
}
