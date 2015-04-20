//In a file MaxCamera.cs
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public float mouseSensitivity = 1.0f;
	public float keySensitivity = 20.0f;

	public float maxDeltaX = 10.0f;
	public float maxDeltaY = 5.0f;

	private Vector3 lastPosition;
	private Vector3 cameraStartPos;
	private Vector3 cameraWorldPos;

	public Vector3 LevelCenter;

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

			//Debug.Log (totalDeltaX);

			if (Mathf.Abs (totalDeltaX) < maxDeltaX + Mathf.Abs (LevelCenter.x))
			{
				transform.Translate(dX, 0, 0);
				lastPosition.x = cameraWorldPos.x;
			}
			else
			{
				lastPosition.x = maxDeltaX + Mathf.Sign (totalDeltaX - LevelCenter.x);
			}

			if (Mathf.Abs(totalDeltaY - LevelCenter.y) < maxDeltaY)
			{
				transform.Translate(0, dY, 0);
				lastPosition.y = cameraWorldPos.y;
			}
			else
			{
				lastPosition.y = maxDeltaY  + Mathf.Sign (totalDeltaY - LevelCenter.y);
			}
		}

		if(Input.GetKey (KeyCode.W))
		{
			transform.Translate(0.0f, keySensitivity * Time.deltaTime, 0.0f);
		}

		if(Input.GetKey (KeyCode.A))
		{
			transform.Translate(-keySensitivity * Time.deltaTime, 0.0f, 0.0f);
		}

		if(Input.GetKey(KeyCode.S))
		{
			transform.Translate(0.0f, -keySensitivity * Time.deltaTime, 0.0f);
		}

		if(Input.GetKey (KeyCode.D))
		{
			transform.Translate(keySensitivity * Time.deltaTime, 0.0f, 0.0f);
		}
	}
}