using UnityEditor;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    private static AudioManager m_instance;

    public AudioClip Clip;
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
        audioSource.clip = Clip;
        audioSource.Play();
    }
}
