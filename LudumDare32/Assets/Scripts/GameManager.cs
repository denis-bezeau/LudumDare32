using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{	
	[System.Serializable]
	public class AttackWave
	{
		public GameObject prefab;
		public int count = 10;
		public PersonAI.EPersonality personality;
		public float duration = 60.0f;
	}

	private static GameManager instance;

	//exposed variables for the user to change
	public int MaximumEscapeeCount = 10;
	public int TotalEnemies;
	private int totalKills = 0;
	public float energyRegenSpeed = 10.0f;
	public GameObject[] EnemySpawnLocations;
	public Room GoalRoom;
	public Animator Hud;
	public AttackWave[] attackWaves;

	private GameObject PlantTrapPrefab;
	private GameObject MarbleTrapPrefab;
	private GameObject DoorTrapPrefab;


	private int currentEscapeeCount;
	private GameObject DefaultLevel;
	private List<PersonAI> Enemies = new List<PersonAI>();
	private bool musicStarted;
	private SpawnRoom _spawnRoom = null;
	private EscapeRoom _escapeRoom = null;
	private int [] trapCosts;
	private float energy;
	private Trap.TrapType currentSelectedTrap = Trap.TrapType.None;

	private float _nextAttackWaveTime = 0.0f;
	private int _attackWaveNumber = 1;

	[SerializeField]
	private CameraManager _cameraManager;

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
		instance = this;
		SetUpTrapData();

		CTEventManager.AddListener<KillEnemyEvent>(OnKillEnemyEvent);
		CTEventManager.AddListener<RestartGameEvent>(OnRestartGame);
		CTEventManager.AddListener<EscapeEvent>(OnEscapeEvent);
		CTEventManager.AddListener<BuyTrapEvent>(OnBuyTrapEvent);
		CTEventManager.AddListener<PlaceTrapEvent>(OnPlaceTrapEvent);

		PlayerPrefs.SetFloat("sfxVolume", 1.0f);
		PlayerPrefs.SetFloat("musicVolume", 0.02f);
		PlayerPrefs.SetFloat("speechVolume", 0.85f);

		//CTEventManager.FireEvent(new PlaySFXEvent() {assetName =""});
		
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
		totalKills += eventData.count;
		if (totalKills  >= TotalEnemies)
		{
			YouWin();
		}

		CTEventManager.FireEvent(new PlaySFXEvent() {assetName = "fbm_Death"}); //events for everyone
		Debug.Log("DEATH!!!");

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

		AsyncOperation levelLoadOperation = Application.LoadLevelAdditiveAsync("Level_00");//"RoomTest");

		while (levelLoadOperation.isDone == false)
		{
			yield return null;
		}

		GameObject.Instantiate(_cameraManager, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

		if (musicStarted == false)
		{
			CTEventManager.FireEvent(new PlayMusicEvent() { assetName = "audio/music/climactic-final-battle" });
			musicStarted = true;
		}

		DefaultLevel = GameObject.FindGameObjectWithTag("LEVEL");

		// Find spawn and escape rooms rooms
		_spawnRoom = FindObjectOfType(typeof(SpawnRoom)) as SpawnRoom;
		_escapeRoom = FindObjectOfType(typeof(EscapeRoom)) as EscapeRoom;
		
		// Start level
		_attackWaveNumber = 0;
		_nextAttackWaveTime = 0.0f;
	}

	private void SpawnEnemy(GameObject prefab, Vector3 pos, PersonAI.EPersonality personality)
	{
		GameObject enemyGO = GameObject.Instantiate(prefab, pos, Quaternion.identity) as GameObject;
		enemyGO.name = "enemy number ...  " + (Enemies.Count + 1);
		PersonAI newEnemy = enemyGO.GetComponent<PersonAI>();
		newEnemy.m_personality = personality;
		newEnemy.EnterLevel(_spawnRoom, _escapeRoom);
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
		UpdateAttackWaves();

		energy += energyRegenSpeed * Time.deltaTime;

		if (energy > 100)
		{
			energy = 100;
		}
		CTEventManager.FireEvent(new UpdateEnergyEvent() { energy = (int)this.energy });


	}

	/// @brief	Manage attack waves
	private void UpdateAttackWaves()
	{
		// More attack waves to generate?
		if(_attackWaveNumber < attackWaves.Length)
		{
			// Time to generate the next attack wave?
			if(Time.time > _nextAttackWaveTime)
			{
				// Generate enemies in spawn room
				if(_spawnRoom != null)
				{
					// Does the spawn room have tiles we can use to position new enemies?
					int numEnemies = 0;
					List<GameTile> spawnTiles = _spawnRoom.GameTiles;
					
#if false
					if((spawnTiles != null) && (spawnTiles.Count > 0))
					{
						// Generate enemies on room tiles
					foreach(GameTile tile in spawnTiles)
					{
						if(!tile.IsWallTile)
						{
							SpawnEnemy(attackWaves[_attackWaveNumber].prefab, tile.transform.position, attackWaves[_attackWaveNumber].personality);
							
							// Got enough for attack wave?
							numEnemies++;
							if(numEnemies >= attackWaves[_attackWaveNumber].count)
								break;
						}
					}
					}
					else
#endif
					{
					// Spawn enemies in centre pf the spawn room
					BoxCollider spawnBox = _spawnRoom.GetComponent<BoxCollider>();
					if(spawnBox != null)
					{
						for(int i=0; i<attackWaves[_attackWaveNumber].count; i++)
						{
								// 80% of box size used to avoid walls
								Vector3 min = spawnBox.center-(spawnBox.size*0.4f);	
								Vector3 pos = new Vector3(min.x+(spawnBox.size.x*UnityEngine.Random.value*0.8f),
								                          min.y+(spawnBox.size.y*UnityEngine.Random.value*0.8f),
								                          min.z+(spawnBox.size.z*UnityEngine.Random.value*0.8f));
								SpawnEnemy(attackWaves[_attackWaveNumber].prefab, pos, attackWaves[_attackWaveNumber].personality);
						}
					}
				}
				}
				
				_nextAttackWaveTime = Time.time + attackWaves[_attackWaveNumber].duration;
				_attackWaveNumber++;
			}
		}
	}

	public void SetUpTrapData()
	{
		energy = 0;

		trapCosts = new int [(int)Trap.TrapType.COUNT];
		trapCosts[(int)Trap.TrapType.None] = 0;
		trapCosts[(int)Trap.TrapType.Door] = 10;
		trapCosts[(int)Trap.TrapType.Plant] = 30;
		trapCosts[(int)Trap.TrapType.Marble] = 20;
	}

	public int GetTrapCost(Trap.TrapType type)
	{
		if ((int)type < (int)Trap.TrapType.COUNT)
		{
			return trapCosts[(int)type];
		}

		return 0;
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
					if (DoorTrapPrefab == null)
					{
						DoorTrapPrefab = Resources.Load("Prefabs/traps/DoorTrap/DoorTrap") as GameObject;
					}
					GameObject newTrapGameObject = GameObject.Instantiate(DoorTrapPrefab, tile.transform.position, Quaternion.identity) as GameObject;
					newTrapGameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
					Trap newTrap = newTrapGameObject.GetComponent<Trap>();
					tile.currentTrap = newTrap;
					if (tile.parentRoom != null)
					{
						tile.parentRoom.AddTrapToRoom(newTrap);
						newTrap._parentRoom = tile.parentRoom;
					}
					
					break;
				}
			case Trap.TrapType.Plant:
				{
					currentSelectedTrap = Trap.TrapType.None;
					if (PlantTrapPrefab == null)
					{
						PlantTrapPrefab = Resources.Load("Prefabs/traps/plantTrap/fernTrap") as GameObject;
					}
					GameObject newTrapGameObject = GameObject.Instantiate(PlantTrapPrefab, tile.transform.position, Quaternion.identity) as GameObject;
					newTrapGameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
					Trap newTrap = newTrapGameObject.GetComponent<Trap>();
					tile.currentTrap = newTrap;
					if (tile.parentRoom != null)
					{
						tile.parentRoom.AddTrapToRoom(newTrap);
						newTrap._parentRoom = tile.parentRoom;
					}
					break;
				}
			case Trap.TrapType.Marble:
				{
					currentSelectedTrap = Trap.TrapType.None;
					if (MarbleTrapPrefab == null)
					{
						MarbleTrapPrefab = Resources.Load("Prefabs/traps/MarbleTrap/MarbleTrap") as GameObject;
					}
					Debug.Log(MarbleTrapPrefab);
					GameObject newTrapGameObject = GameObject.Instantiate(MarbleTrapPrefab, tile.transform.position, Quaternion.identity) as GameObject;
					newTrapGameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
					Trap newTrap = newTrapGameObject.GetComponent<Trap>();
					tile.currentTrap = newTrap;
					if (tile.parentRoom != null)
					{
						tile.parentRoom.AddTrapToRoom(newTrap);
						newTrap._parentRoom = tile.parentRoom;
					}
					
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

		Debug.Log("tile.currentTrap=" + tile.currentTrap);
		if (tile.currentTrap != null)
		{
			
			return false;
		}

		return true;
	}

	public bool IsPlacingTrap()
	{
		return ((currentSelectedTrap != Trap.TrapType.None) && (currentSelectedTrap != Trap.TrapType.COUNT));
	}
}
