using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	private static GameManager instance;

	//exposed variables for the user to change
	public int MaximumEscapeeCount = 10;
	public int TotalEnemies;
	public GameObject EnemyPrefab;
	public GameObject[] EnemySpawnLocations;
	public Room GoalRoom;

	private int currentEscapeeCount;
	private GameObject DefaultLevel;
	private List<PersonAI> Enemies = new List<PersonAI>();
	private bool musicStarted;
	private Door _entranceDoor = null;
	private Door _exitDoor = null;

	public static GameManager GetInstance()
	{
		if (instance == null)
		{
			GameObject gameManagerGameObject = GameObject.FindGameObjectWithTag("game_manager");
			if (gameManagerGameObject != null)
			{
				instance = gameManagerGameObject.GetComponent<GameManager>();
				if (instance == null)
				{
					instance = gameManagerGameObject.AddComponent<GameManager>();
				}
			}
			else
			{
				gameManagerGameObject = new GameObject();
				gameManagerGameObject.name = "game_manager";
				gameManagerGameObject.tag = "game_manager";
				instance = gameManagerGameObject.AddComponent<GameManager>();
			}
		}
		return instance;
	}

	void Start()
	{
		ResetGameSettings();
	}

	public void Awake()
	{
		CTEventManager.AddListener<KillEnemyEvent>(OnKillEnemyEvent);
		CTEventManager.AddListener<RestartGameEvent>(OnRestartGame);
		CTEventManager.AddListener<EscapeEvent>(OnEscapeEvent);

		PlayerPrefs.SetFloat("sfxVolume", 1.0f);
		PlayerPrefs.SetFloat("musicVolume", 0.02f);
		PlayerPrefs.SetFloat("speechVolume", 0.85f);

		SoundManager.GetInstance();
	}

	public void OnDestroy()
	{
		CTEventManager.RemoveListener<KillEnemyEvent>(OnKillEnemyEvent);
		CTEventManager.RemoveListener<RestartGameEvent>(OnRestartGame);
		CTEventManager.RemoveListener<EscapeEvent>(OnEscapeEvent);
	}

	public void OnKillEnemyEvent(KillEnemyEvent eventData)
	{
		Enemies.Remove(eventData.enemy);

		if (Enemies.Count <= 0)
		{
			YouWin();
		}
	}

	public void OnEscapeEvent(EscapeEvent eventData)
	{
		currentEscapeeCount++;
		Enemies.Remove(eventData.enemy);

		if (currentEscapeeCount >= MaximumEscapeeCount)
		{
			YouLose();
		}
	}

	public void OnRestartGame(RestartGameEvent eventData)
	{
		ResetGameSettings();
	}

	private void ResetGameSettings()
	{
		for(int i = 0; i < Enemies.Count; ++i)
		{
			GameObject.Destroy(Enemies[i].gameObject);
			Enemies[i] = null;
		}
		Enemies.Clear();

		StartCoroutine(ReLoadLevel());

		currentEscapeeCount = 0;
	}

	private IEnumerator ReLoadLevel()
	{
		GameObject.Destroy(DefaultLevel);
		DefaultLevel = null;

		AsyncOperation levelLoadOperation = Application.LoadLevelAdditiveAsync("DefaultLevel");//"RoomTest");

		while (levelLoadOperation.isDone == false)
		{
			yield return null;
		}

		if (musicStarted == false)
		{
			CTEventManager.FireEvent(new PlayMusicEvent() { assetName = "audio/music/climactic-final-battle" });
			musicStarted = true;
		}

		DefaultLevel = GameObject.FindGameObjectWithTag("LEVEL");

		// Find entrance and exit doors
		Door[] allDoors = FindObjectsOfType(typeof(Door)) as Door[];
		foreach(Door door in allDoors)
		{
			if(door.isEntrance)
				_entranceDoor = door;
			if(door.isExit)
				_exitDoor = door;
		}

		// Spawn enemies (after finding entrance door above)
		for (int i = 0; i < TotalEnemies; ++i)
		{
			SpawnEnemy();
		}
	}

	private void SpawnEnemy()
	{
		int randomSpawnLocation = Random.Range(0, EnemySpawnLocations.Length - 1);
		GameObject enemyGO = GameObject.Instantiate(EnemyPrefab, EnemySpawnLocations[randomSpawnLocation].transform.position, Quaternion.identity) as GameObject;
		enemyGO.name = "enemy number ...  " + (Enemies.Count + 1);
		PersonAI newEnemy = enemyGO.GetComponent<PersonAI>();
		newEnemy.EnterLevel(_entranceDoor);
		Enemies.Add(newEnemy);
	}

	public void YouWin()
	{
		Application.LoadLevel("YouWin");
	}

	public void YouLose()
	{
		Application.LoadLevel("YouLose");
	}
}
