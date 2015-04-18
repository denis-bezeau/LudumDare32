using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaySFXEvent: CTEvent
{
    public string assetName;
}

public class PlayMusicEvent : CTEvent
{
    public string assetName;
}

public class PlaySpeechEvent : CTEvent
{
    public string assetName;
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    static private AudioSource musicAudioSource;
    static private AudioSource sfxAudioSource;
    static private AudioSource speechAudiosource;

    public float musicVolume { get; set;}
    public float sfxVolume { get; set; }
    public float speechVolume { get; set; }

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();
    

    public static SoundManager GetInstance()
    {
        if (instance == null)
        {
            GameObject soundManagerGameObject = GameObject.FindGameObjectWithTag("sound_manager");
            if (soundManagerGameObject != null)
            {
                instance = soundManagerGameObject.GetComponent<SoundManager>();

                if (instance == null)
                {
                    musicAudioSource = soundManagerGameObject.AddComponent<AudioSource>();
                    sfxAudioSource = soundManagerGameObject.AddComponent<AudioSource>();
                    speechAudiosource = soundManagerGameObject.AddComponent<AudioSource>();
                    instance = soundManagerGameObject.AddComponent<SoundManager>();
                }
            }
            else
            {
                soundManagerGameObject = new GameObject();
                soundManagerGameObject.name = "sound_manager";
                soundManagerGameObject.tag = "sound_manager";

                musicAudioSource = soundManagerGameObject.AddComponent<AudioSource>();
                sfxAudioSource = soundManagerGameObject.AddComponent<AudioSource>();
                speechAudiosource = soundManagerGameObject.AddComponent<AudioSource>();
                instance = soundManagerGameObject.AddComponent<SoundManager>();

            }  
        }

        return instance;
    }

    public static void DestroyInstance()
    {
        if (instance != null)
        {
            instance.UnloadAudioClips();
            instance.UnloadMusic();

            GameObject.Destroy(instance.gameObject);
            instance = null;
        }
    }

	public void Awake () 
    {
        DontDestroyOnLoad(transform.gameObject);

        CTEventManager.AddListener<PlaySFXEvent>(OnPlaySFX);
        CTEventManager.AddListener<PlayMusicEvent>(OnPlayMusic);
        CTEventManager.AddListener<PlaySpeechEvent>(OnPlaySpeech);
	}

    public void OnDestroy()
    {
        CTEventManager.RemoveListener<PlaySFXEvent>(OnPlaySFX);
        CTEventManager.RemoveListener<PlayMusicEvent>(OnPlayMusic);
        CTEventManager.RemoveListener<PlaySpeechEvent>(OnPlaySpeech);  
    }

    public void Start()
    {
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        speechVolume = PlayerPrefs.GetFloat("speechVolume");
    }

    private void OnPlaySFX(PlaySFXEvent eventData)
    {
        if (audioClips.ContainsKey(eventData.assetName))
        {
            sfxAudioSource.PlayOneShot(audioClips[eventData.assetName], musicVolume);
        }
        else
        {
            AudioClip audioClip = Resources.Load(eventData.assetName) as AudioClip;
            audioClips.Add(eventData.assetName, audioClip);
            sfxAudioSource.PlayOneShot(audioClip, sfxVolume);
        }
    }

    private void OnPlaySpeech(PlaySpeechEvent eventData)
    {
        if (audioClips.ContainsKey(eventData.assetName))
        {
            speechAudiosource.PlayOneShot(audioClips[eventData.assetName], musicVolume);
        }
        else
        {
            AudioClip audioClip = Resources.Load(eventData.assetName) as AudioClip;
            audioClips.Add(eventData.assetName, audioClip);
            speechAudiosource.PlayOneShot(audioClip, speechVolume);
        }
    }

    private void OnPlayMusic(PlayMusicEvent eventData)
    {
        if (musicClips.ContainsKey(eventData.assetName))
        {
            musicAudioSource.clip = musicClips[eventData.assetName];
            musicAudioSource.volume = musicVolume;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
        else
        {
            AudioClip musicClip = Resources.Load(eventData.assetName) as AudioClip;
            musicClips.Add(eventData.assetName, musicClip);
            musicAudioSource.clip = musicClip;
            musicAudioSource.volume = musicVolume;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
    }

    public void UnloadAudioClips()
    {
        foreach (KeyValuePair<string, AudioClip> currentAudioClip in audioClips)
        {
            Resources.UnloadAsset(currentAudioClip.Value);
        }
        audioClips.Clear();
    }

    public void UnloadMusic()
    {
        foreach (KeyValuePair<string, AudioClip> currentMusicClip in musicClips)
        {
            Resources.UnloadAsset(currentMusicClip.Value);
        }
        musicClips.Clear();
    }
}
