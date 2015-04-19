using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	private static GameManager instance;

	//exposed variables for the user to change
	public int MaximumEscapeeCount = 10;
	public int TotalEnemies;
	public float energyRegenSpeed = 10.0f;
	public GameObject EnemyPrefab;
	public GameObject[] EnemySpawnLocations;
	public Room GoalRoom;
	public Animator Hud;
	public GameObject PlantTrapPrefab;
	public GameObject MarbleTrapPrefab;
	public GameObject DoorTrapPrefab;


	private int currentEscapeeCount;
	private GameObject DefaultLevel;
	private List<PersonAI> Enemies = new List<PersonAI>();
	private bool musicStarted;
	private Door _entranceDoor = null;
	private Door _exitDoor = null;
	private int [] trapCosts;
	private float energy;
	private Trap.TrapType currentSelectedTrap = Trap.TrapType.None;

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
		CTEventManager.AddListener<BuyTrapEvent>(OnBuyTrapEvent);
		CTEventManager.AddListener<PlaceTrapEvent>(OnPlaceTrapEvent);

		PlayerPrefs.SetFloat("sfxVolume", 1.0f);
		PlayerPrefs.SetFloat("musicVolume", 0.02f);
		PlayerPrefs.SetFloat("speechVolume", 0.85f);

		SoundManager.GetInstance();
		SetUpTrapData();
	}

	public void OnDestroy()
	{
		CTEventManager.RemoveListener<KillEnemyEvent>(OnKillEnemyEvent);
		CTEventManager.RemoveListener<RestartGameEvent>(OnRestartGame);
		CTEventManager.RemoveListener<EscapeEvent>(OnEscapeEvent);
		CTEventManager.RemoveListener<BuyTrapEvent>(OnBuyTrapEvent);
		CTEventManager.RemoveListener<PlaceTrapEvent>(OnPlaceTrapEvent);
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

		ShowHud();

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

	void ShowHud()
	{
		Hud.SetBool("Show", true);
	}

	public void Update()
	{
		energy += energyRegenSpeed * Time.deltaTime;
	}

	public void SetUpTrapData()
	{
		energy = 0;

		trapCosts = new int [(int)Trap.TrapType.COUNT];
		trapCosts[(int)Trap.TrapType.Door] = 10;
		trapCosts[(int)Trap.TrapType.Plant] = 30;
		trapCosts[(int)Trap.TrapType.Marble] = 20;
	}

	public int GetTrapCost(Trap.TrapType type)
	{
		return trapCosts[(int)type];
	}

	public void OnBuyTrapEvent(BuyTrapEvent eventData)
	{
		int cost = GetTrapCost(eventData.type);
		if (cost < energy)
		{
			currentSelectedTrap = eventData.type;
			Debug.Log("current selected trap = " + eventData.type);
		}
		else
		{
			Debug.Log("not enough mana!");
		}
	}

	public void OnPlaceTrapEvent(PlaceTrapEvent eventData)
	{
		Debug.Log("OnPlaceTrapEvent");
		if (currentSelectedTrap != Trap.TrapType.None)
		{
			Debug.Log("OnPlaceTrapEvent currentSelectedTrap =" + currentSelectedTrap);
			int cost = GetTrapCost(currentSelectedTrap);
			if (cost < energy)
			{
				Debug.Log("OnPlaceTrapEvent cost =" + cost + " , energy=" + energy);
				energy -= (float)cost;
				InstantiateTrapAtLocation(currentSelectedTrap, eventData.gameTile);
			}
		}
		else
		{
			currentSelectedTrap = Trap.TrapType.None;
		}
	}
	
	public void InstantiateTrapAtLocation(Trap.TrapType type, GameTile tile)
	{
		Debug.Log("InstantiateTrapAtLocation type=" + type + ", position=" + tile.transform.position);
		switch (type)
		{
			case Trap.TrapType.Door:
				{
					currentSelectedTrap = Trap.TrapType.None;
					GameObject.Instantiate(DoorTrapPrefab, tile.transform.position, Quaternion.identity);
					break;
				}
			case Trap.TrapType.Plant:
				{
					currentSelectedTrap = Trap.TrapType.None;
					GameObject.Instantiate(PlantTrapPrefab, tile.transform.position, Quaternion.identity);
					break;
				}
			case Trap.TrapType.Marble:
				{
					currentSelectedTrap = Trap.TrapType.None;
					GameObject.Instantiate(MarbleTrapPrefab, tile.transform.position, Quaternion.identity);
					
					break;
				}
		}
		
	}

	public bool IsTileValid(GameTile tile)
	{
		if (tile.GetComponent<BoxCollider>() != null)
		{
			return false;
		}


		//door traps need to be on doors
		if (currentSelectedTrap == Trap.TrapType.Door)
		{
			Door doorComponent = tile.transform.parent.GetComponent<Door>();
			if (doorComponent == null)
			{
				return false;
			}
		}
		//regular traps need to be not on doors
		else
		{
			Door doorComponent = tile.transform.parent.GetComponent<Door>();
			if (doorComponent != null)
			{
				return false;
			}
		}

		return true;
	}

	public bool IsPlacingTrap()
	{
		return ((currentSelectedTrap != Trap.TrapType.None) && (currentSelectedTrap != Trap.TrapType.COUNT));
	}
}
