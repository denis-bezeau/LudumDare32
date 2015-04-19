using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudTrapButton : MonoBehaviour
{
	[SerializeField]
	private Text quantityLabel;

	[SerializeField]
	private Trap.TrapType type;

	public void Start()
	{
		Debug.Log(name + "Start");
		quantityLabel.text = GameManager.GetInstance().GetTrapCost(type).ToString();
	}

	public void OnClick()
	{
		Debug.Log("click on button: " + type);
		CTEventManager.FireEvent(new BuyTrapEvent() { type = this.type });
	}
}
