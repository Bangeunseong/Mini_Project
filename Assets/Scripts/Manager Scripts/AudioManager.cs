using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private List<AudioClip> _mainBGMs;
    private AudioSource audioSource;

    public bool IsaudioSourceChanged { get; set; } = false;

    private static AudioManager m_instance;

    public static AudioManager Instance
    {
        get
        {
            if(m_instance == null) m_instance = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
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
        audioSource.volume = 1f;
        audioSource.clip = _mainBGMs[0];
        audioSource.Play();
    }

    public IEnumerator FadeOutSound(float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume,0f,time/duration);      //첫 값과 끝 값을 변화/비율 양 = time/duration = 비율 기준 대로 시작 지점부터 끝 지점 까지 증가 시켜라
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();

        IsaudioSourceChanged = true;
    }
    
    public IEnumerator FadeInSound(int category, float duration)
    {
        IsaudioSourceChanged = false;

        audioSource.clip = _mainBGMs[category];
        audioSource.Play();

        float time = 0f;
        while(time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f,1f,time/duration);               //위와 반대
            yield return null;
        }
        audioSource.volume = 1f;
    }
}
