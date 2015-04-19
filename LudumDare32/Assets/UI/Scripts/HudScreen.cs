using UnityEngine;
using System.Collections;

public class HudScreen : MonoBehaviour {

	public void Show() {
		animator.SetTrigger("Show");
	}

	public void Hide() {
		animator.SetTrigger("Hide");
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private HudGeister geister;

	[SerializeField]
	private HudTrapButton[] trapButtons;

}
