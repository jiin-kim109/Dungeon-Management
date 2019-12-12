using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour {

    [SerializeField]
    private AudioSource source;

	[System.Serializable]
    public struct Clips
    {
        public AudioClip BGM_1;
    }
    [Space(10)]
    [SerializeField]
    private Clips m_clips;
    public Clips clips { get { return m_clips; } }

    [SerializeField]
    private List<AudioClip> m_sfxFolder = new List<AudioClip>();
    public List<AudioClip> sfxFolder { get { return m_sfxFolder; } }

    //---------------Instance------------------
    private static AudioHandler _instance = null;
    public static AudioHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(AudioHandler)) as AudioHandler;
                if (_instance == null) { Debug.Log("AudioHandler가 없음"); }
            }
            return _instance;
        }
    }
    //-----------------------------------------

    //===
    void Awake()
    {
        source.loop = true;
    }

    //===
    public void Initialize()
    {
        PlayBGM(clips.BGM_1);
    }
    public void Initialize(float timeStart)
    {
        PlayBGM(clips.BGM_1, timeStart);
    }

    //===
    public void PlayBGM(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }
    public void PlayBGM(AudioClip clip, float timeStart)
    {
        source.clip = clip;
        source.Play();
        source.time = timeStart;
    }

    public void PlaySFX(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
    public void PlaySFX(AudioClip clip, float volumeRate)
    {
        source.PlayOneShot(clip, volumeRate);
    }

    public void StopAudio()
    {
        source.Stop();
    }
}
