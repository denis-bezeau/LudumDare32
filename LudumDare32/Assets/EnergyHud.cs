using UnityEngine;
using System.Collections;

public class UpdateEnergyEvent : CTEvent
{
	public int energy;
}

public class EnergyHud : MonoBehaviour 
{
	public UnityEngine.UI.Text label;

	public void Awake()
	{
		CTEventManager.AddListener<UpdateEnergyEvent>(OnUpdateEnergy);
	}
	public void OnDestroy()
	{
		CTEventManager.RemoveListener<UpdateEnergyEvent>(OnUpdateEnergy);
	}

	public void OnUpdateEnergy(UpdateEnergyEvent eventData)
	{
		label.text = eventData.energy.ToString();
	}
}
