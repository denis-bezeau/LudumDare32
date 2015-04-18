using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public GameObject DefaultLevel;
	public GameObject EnemyPrefab;
	public GameObject[] EnemySpawnLocations;
	public int TotalEnemies;
	private List<PersonAI> Enemies = new List<PersonAI>();

	public void Awake()
	{
		CTEventManager.AddListener<KillEnemyEvent>(OnKillEnemyEvent);
		CTEventManager.AddListener<RestartGameEvent>(OnRestartGame);

		ResetGameSettings();
	}

	public void OnDestroy()
	{
		CTEventManager.RemoveListener<KillEnemyEvent>(OnKillEnemyEvent);
		CTEventManager.RemoveListener<RestartGameEvent>(OnRestartGame);
	}

	public void OnKillEnemyEvent(KillEnemyEvent eventData)
	{
		Debug.Log("OnKillEnemyEvent.enemyType: " + eventData.enemyType);
		Debug.Log("OnKillEnemyEvent.count: " + eventData.count);

		Enemies.Remove(eventData.enemy);
	}

	public void OnRestartGame(RestartGameEvent eventData)
	{
		Debug.Log("OnKillEnemyEvent");

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
		Debug.Log("enemies.count=" + Enemies.Count);
		for (int i = 0; i < TotalEnemies; ++i)
		{
			SpawnEnemy();
		}
		Debug.Log("enemies.count=" + Enemies.Count);
		StartCoroutine(ReLoadLevel());
	}

	private IEnumerator ReLoadLevel()
	{
		GameObject.Destroy(DefaultLevel);
		DefaultLevel = null;

		AsyncOperation levelLoadOperation = Application.LoadLevelAdditiveAsync("DefaultLevel");

		while (levelLoadOperation.isDone == false)
		{
			yield return null;
		}

		DefaultLevel = GameObject.FindGameObjectWithTag("LEVEL");
	}

	private void SpawnEnemy()
	{
		int randomSpawnLocation = Random.Range(0, EnemySpawnLocations.Length - 1);
		GameObject enemyGO = GameObject.Instantiate(EnemyPrefab, EnemySpawnLocations[randomSpawnLocation].transform.position, Quaternion.identity) as GameObject;
		enemyGO.name = "enemy number ...  " + (Enemies.Count + 1);
		PersonAI newEnemy = enemyGO.GetComponent<PersonAI>();
		Enemies.Add(newEnemy);
	}

	float timer = 0;
	public void Update()
	{
		timer += Time.deltaTime;

		if (timer > 5.0f)
		{
			timer = 0.0f;
			ResetGameSettings();
		}
	}
}
