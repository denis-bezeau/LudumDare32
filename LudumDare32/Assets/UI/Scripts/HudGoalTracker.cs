using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HudGoalTracker : MonoBehaviour 
{
	private int _totalGoal;
	private int _killTally;
	private int _escapeTally;

	[SerializeField]
	private Text _totalText;

	[SerializeField]
	private Text _killText;

	[SerializeField]
	private Text _escapeText;

	private bool _gameStarted = false;

	// Use this for initialization
	void Awake () {
		_totalGoal = 0;
		_killTally = 0;
		_escapeTally = 0;

		_totalText.text = _totalGoal.ToString();
		_killText.text = _killTally.ToString();
		_escapeText.text = _escapeTally.ToString();

		_gameStarted = false;

		CTEventManager.AddListener<InitGameEvent>(OnGameInit);
		CTEventManager.AddListener<KillEnemyEvent> (OnKillEnemyEvent);
		CTEventManager.AddListener<RestartGameEvent> (OnRestartGame);
		CTEventManager.AddListener<EscapeEvent> (OnEscapeEvent);
	}
	
	public void OnDestroy()
	{
		CTEventManager.RemoveListener<InitGameEvent>(OnGameInit);
		CTEventManager.RemoveListener<KillEnemyEvent> (OnKillEnemyEvent);
		CTEventManager.RemoveListener<RestartGameEvent> (OnRestartGame);
		CTEventManager.RemoveListener<EscapeEvent> (OnEscapeEvent);
	}

	void OnGameInit(InitGameEvent eventData)
	{
		_totalGoal = eventData.totalEnemies;
		_killTally = 0;
		_escapeTally = 0;

		_totalText.text = _totalGoal.ToString();
		_killText.text = _killTally.ToString();
		_escapeText.text = _escapeTally.ToString();
		_gameStarted = true;
	}

	public void OnKillEnemyEvent (KillEnemyEvent eventData)
	{
		_killTally++;
		_killText.text = _killTally.ToString();
		_totalGoal--;
		_totalText.text = _totalGoal.ToString();
	}
	
	public void OnEscapeEvent (EscapeEvent eventData)
	{
		_escapeTally++;
		_escapeText.text = _escapeTally.ToString();
		_totalGoal--;
		_totalText.text = _totalGoal.ToString();
	}
	
	public void OnRestartGame (RestartGameEvent eventData)
	{
		_killTally = 0;
		_escapeTally = 0;
		_totalGoal = 0;

		_totalText.text = _totalGoal.ToString();
		_killText.text = _killTally.ToString();
		_escapeText.text = _escapeTally.ToString();
	}
}
