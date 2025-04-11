using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _mainBGMs;
    private AudioSource audioSource;

    public bool IsAudioSourceChanged { get;  set; } = false;
    private static AudioManager m_instance;

    public AudioClip Clip;
    public static AudioManager Instance
    {
        get
        {
            if(m_instance == null) 
               m_instance = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
            return m_instance;
        }
    }

    void Awake()
    {
        // Prevent Double Init. of AudioManager
        if (Instance != this) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = Helper.GetComponentHelper<AudioSource>(gameObject);

        audioSource.clip = _mainBGMs[0];
        audioSource.volume = 1f;
        audioSource.Play();
    }
}


/*public IEnumertor FadeOutSound ( float duration)
{
    float startVolume = AudioSource.volume;
    float time = 0f;
    while (time < duration)
    {
        time += Time.deltaTime;
        audioSource.volume = Mathf.Lerp(startVolume, 0f , time / duration);
        yield return null;
    }

    audioSource.volume = 0f;
    audioSource.Stop();

    IsAudioSourceChanged = true;
}

public IEnumertor FadeInSound ( float duration)
{
    IsAudioSourceChanged = false;

    audioSource.clip = _mainBGMs[(int)Category +1];
    audioSource.Play();

    float time = 0f;
    while (time < duration)
    {
        time += Time.deltaTime;
        audioSource.volume = Mathf.Lerp(0f, 1f, time / duration);
        yield return null;
    }

    AudioSource.volume = 1f;
}
*/