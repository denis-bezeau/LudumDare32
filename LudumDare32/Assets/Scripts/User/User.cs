using UnityEngine;
using System.Collections;

/// <remarks></remarks>
/// DEPRECATED, we'll just store the gameManager for now.
/// </remarks>
public class User : MonoBehaviour
{

	private int _currentCurrency;
	public int CurrentCurrency {
		get { return _currentCurrency;}
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void GiveCurrency (int amount)
	{
		_currentCurrency += amount;
	}

	void RemoveCurrency (int amount)
	{
		if (amount > _currentCurrency)
		{
			Debug.LogError ("Removing too much currency!");
			_currentCurrency = 0;
		} else
		{
			_currentCurrency -= amount;
		}
	}
}
