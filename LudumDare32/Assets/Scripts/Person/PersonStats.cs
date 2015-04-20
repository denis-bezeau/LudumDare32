using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersonStats : MonoBehaviour
{
	private List<string> deathSounds_fatbat = new List<string> ();
	private List<string> spawnSounds_fatbat = new List<string> ();
	private List<string> hitSounds_fatbat = new List<string> ();
	private List<string> deathSounds_redShirt = new List<string> ();
	private List<string> spawnSounds_redShirt = new List<string> ();
	private List<string> hitSounds_redShirt = new List<string> ();

	public enum SOUND_EFFECT
	{
		DEATH,
		SPAWN,
		HIT,
	}

	public enum PERSON_TYPE
	{
		RED_SHIRT,
		FAT_BAT,
	}

	public PERSON_TYPE personType;
	
	public int MaxHP = 30;
	private int hp = 5;

	public void Awake ()
	{
		HP = MaxHP;

		deathSounds_fatbat.Add ("audio/sfx/enemy_fbm_Death");

		deathSounds_fatbat.Add ("audio/sfx/enemy_fbm_GetHit");

		spawnSounds_fatbat.Add ("audio/sfx/enemy_fbm_ImFatBatman");
		spawnSounds_fatbat.Add ("audio/sfx/enemy_fbm_ImGettingOutOfHere");
		spawnSounds_fatbat.Add ("audio/sfx/enemy_fbm_RobinDied");

		deathSounds_redShirt.Add ("audio/sfx/enemy_rds_Death");
		deathSounds_redShirt.Add ("audio/sfx/enemy_rds_GetHit_Plant");

		deathSounds_redShirt.Add ("audio/sfx/enemy_rds_GetHit");

		spawnSounds_redShirt.Add ("audio/sfx/enemy_rds_Random_StopLookingAtMe");
	}

	public int HP {
		get { return hp; }
		set {
			hp = value;

			if (hp <= 0)
			{
				PersonAI person = GetComponent<PersonAI> ();
				person.Kill ();
			}
		}
	}

	public string GetRandomSound (SOUND_EFFECT effect)
	{
		if (personType == PERSON_TYPE.FAT_BAT)
		{
			if (effect == SOUND_EFFECT.DEATH)
			{
				int index = Random.Range (0, (deathSounds_fatbat.Count - 1));
				if (index >= 0)
				{
					return deathSounds_fatbat [index];
				}
			}
			else if (effect == SOUND_EFFECT.HIT)
			{
				int index = Random.Range (0, (hitSounds_fatbat.Count - 1));
				if (index >= 0)
				{
					return hitSounds_fatbat [index];
				}
			}
			else if (effect == SOUND_EFFECT.SPAWN)
			{
				int index = Random.Range (0, (spawnSounds_fatbat.Count - 1));
				if (index >= 0)
				{
					return spawnSounds_fatbat [index];
				}
			}
		}
		else if (personType == PERSON_TYPE.RED_SHIRT)
		{
			if (effect == SOUND_EFFECT.DEATH)
			{
				int index = Random.Range (0, (deathSounds_redShirt.Count - 1));
				if (index >= 0)
				{
					return deathSounds_redShirt [index];
				}
			}
			else if (effect == SOUND_EFFECT.HIT)
			{
				int index = Random.Range (0, (hitSounds_redShirt.Count - 1));
				if (index >= 0)
				{
					return hitSounds_redShirt [index];
				}
			}
			else if (effect == SOUND_EFFECT.SPAWN)
			{
				int index = Random.Range (0, (spawnSounds_redShirt.Count - 1));
				if (index >= 0)
				{
					return spawnSounds_redShirt [index];
				}
			}
		}

		return string.Empty;
	}
}
