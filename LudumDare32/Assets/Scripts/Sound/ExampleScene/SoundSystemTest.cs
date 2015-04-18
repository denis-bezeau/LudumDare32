using UnityEngine;
using System.Collections;

public class SoundSystemTest : MonoBehaviour 
{
	void Start () 
    {
        PlayerPrefs.SetFloat("sfxVolume", 1.0f);
        PlayerPrefs.SetFloat("musicVolume", 0.4f);
        PlayerPrefs.SetFloat("speechVolume", 0.85f);

        SoundManager.GetInstance();
	}

    float timer1;
    float timer2;
    float timer3;

    bool musicStarted = false;
    public void Update()
    {


        timer1 += Time.deltaTime;
        timer2 += Time.deltaTime;
        timer3 += Time.deltaTime;

        if (timer1 > 2.0f)
        {
            CTEventManager.FireEvent(new PlaySFXEvent() { assetName = "audio/sfx/Goat1" });
            timer1 -= 2.0f;
        }

        if (timer2 > 3.3f)
        {
            CTEventManager.FireEvent(new PlaySpeechEvent() { assetName = "audio/sfx/GruntHurt" });
            timer2 -= 3.3f;
        }

        if (musicStarted == false)
        {
            CTEventManager.FireEvent(new PlayMusicEvent() { assetName = "audio/sfx/Hadouken" });
            musicStarted = true;
        }

        if (timer3 > 10.0f)
        {
            timer3 -= 10000000.0f;
            SoundManager.DestroyInstance();
        }
    }
}
