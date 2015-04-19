//In a file MaxCamera.cs
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public float mouseSensitivity = 1.0f;
	public float maxDeltaX = 10.0f;
	public float maxDeltaY = 5.0f;

	private Vector3 lastPosition;
	private Vector3 cameraStartPos;
	private Vector3 cameraWorldPos;

	void Start()
	{
		cameraStartPos = this.transform.position;
	}
	
	void LateUpdate()
	{
		cameraWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButtonDown(1))
		{
			lastPosition = cameraWorldPos;
		}
		
		if (Input.GetMouseButton(1))
		{
			Vector3 delta = cameraWorldPos - lastPosition;
			float dX = delta.x * mouseSensitivity;
			float dY = delta.y * mouseSensitivity;

			float totalDeltaX = dX + transform.position.x;
			float totalDeltaY = dY + transform.position.y;

			if (Mathf.Abs (totalDeltaX - cameraStartPos.x) < maxDeltaX)
			{
				transform.Translate(dX, 0, 0);
				lastPosition.x = cameraWorldPos.x;
			}
			else
			{
				lastPosition.x = maxDeltaX * Mathf.Sign (totalDeltaX - cameraStartPos.x);
			}

			if (Mathf.Abs(totalDeltaY - cameraStartPos.y) < maxDeltaY)
			{
				transform.Translate(0, dY, 0);
				lastPosition.y = cameraWorldPos.y;
			}
			else
			{
				lastPosition.y = maxDeltaY * Mathf.Sign (totalDeltaY - cameraStartPos.y);
			}
		}
	}
}