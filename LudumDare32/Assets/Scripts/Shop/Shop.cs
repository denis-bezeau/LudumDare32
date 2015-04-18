using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Shop : MonoBehaviour {

	[SerializeField] List<TrapButton> _trapButtons;

	[SerializeField] Text currencyDisplay;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SelectTrap(Trap t) {
		//if has enough money
		//create new trap fromm prefab in placement mode
	}

	public void BuyTrap(Trap t) {
		//remove money from user
	}
}
