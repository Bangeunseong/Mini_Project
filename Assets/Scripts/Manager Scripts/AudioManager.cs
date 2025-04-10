using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _mainBGMs;
    private AudioSource audioSource;

    public bool IsAudioSourceChanged { get; set; } = false;
    
    private static AudioManager m_instance;
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

    public IEnumerator FadeOutSound(float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0f;
        while(time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();

        IsAudioSourceChanged = true;
    }

    public IEnumerator FadeInSound(int category, float duration)
    {
        IsAudioSourceChanged = false;

        audioSource.clip = _mainBGMs[category];
        audioSource.Play();

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }

        audioSource.volume = 1f;
    }
}
