using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour 
{
	private static CameraManager instance;

	[SerializeField]
	private GameObject _cameraPrefab;

	[SerializeField]
	private float _camZDepth = -10.0f;

	private Camera _mainCamera;
	public Camera MainCamera
	{
		get { return _mainCamera; }
		set { _mainCamera = value; }
	}

	public static CameraManager GetInstance()
	{
		if (instance == null)
		{
			GameObject cameraManagerGameObject = GameObject.FindGameObjectWithTag("camera_manager");
			if (cameraManagerGameObject != null)
			{
				instance = cameraManagerGameObject.GetComponent<CameraManager>();
				if (instance == null)
				{
					instance = cameraManagerGameObject.AddComponent<CameraManager>();
				}
			}
			else
			{
				cameraManagerGameObject = new GameObject();
				cameraManagerGameObject.name = "camera_manager";
				cameraManagerGameObject.tag = "camera_manager";
				instance = cameraManagerGameObject.AddComponent<CameraManager>();
			}
		}
		return instance;
	}

	void Start()
	{
		Room[] rooms = FindObjectsOfType<Room>();

		Vector2 levelCenter = new Vector2(0.0f, 0.0f);
		Vector3 startPos = new Vector3(0.0f, 0.0f, 0.0f);

		SpawnRoom spawnRoom = new SpawnRoom();
		for(int i = 0; i < rooms.Length; i++)
		{
			levelCenter.x += rooms[i].gameObject.transform.position.x;
			levelCenter.y += rooms[i].gameObject.transform.position.y;

			if(rooms[i].GetType() == typeof(SpawnRoom))
			{
				spawnRoom = (SpawnRoom)rooms[i];
				startPos = spawnRoom.gameObject.transform.position;
			}
		}

		GameObject cameraObj = GameObject.Instantiate(_cameraPrefab, new Vector3(startPos.x, startPos.y, _camZDepth), Quaternion.identity) as GameObject;

		cameraObj.GetComponent<CameraController>().LevelCenter.x = levelCenter.x;
		cameraObj.GetComponent<CameraController>().LevelCenter.y = levelCenter.y;

		cameraObj.GetComponent<CameraController>().maxDeltaX = levelCenter.x * 1.0f;
		cameraObj.GetComponent<CameraController>().maxDeltaY = levelCenter.x * 1.0f;

		cameraObj.tag = "MainCamera";
		_mainCamera = cameraObj.GetComponent<Camera>();
	}
}
