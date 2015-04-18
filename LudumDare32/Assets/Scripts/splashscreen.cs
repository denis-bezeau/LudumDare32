using UnityEngine;
using System.Collections;

public class splashscreen : MonoBehaviour {

	public enum Scene
	{
		GamePlay,
		YouWin,
		YouLose,
		Title,
		COUNT
	}

	public Scene NextScene;

	private string[] scenenames = new string[(int)Scene.COUNT];

	// Use this for initialization
	void Awake () 
	{
		scenenames[(int)Scene.GamePlay] = "Gameplay";
		scenenames[(int)Scene.YouLose] = "YouLose";
		scenenames[(int)Scene.YouWin] = "YouWin";
		scenenames[(int)Scene.Title] = "Title";
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.Space) == true)
		{
			Application.LoadLevel(scenenames[(int)NextScene]);
		}
	}
}
