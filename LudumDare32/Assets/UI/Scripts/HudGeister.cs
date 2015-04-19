using UnityEngine;
using System.Collections;

public class HudGeister : MonoBehaviour {

	/**
	 * Animator is set up to use a scale of 0 (angriest) to 100 (happiest).
	 */
	public int Feeling {
		get {
			return geisterAnimator.GetInteger("Feeling");
		}
		set {
			if(value < 0) {
				value = 0;
			}
			if(value > 100) {
				value = 100;
			}
			geisterAnimator.SetInteger("Feeling", value);
		}
	}

	[SerializeField]
	private Animator geisterAnimator;

}
