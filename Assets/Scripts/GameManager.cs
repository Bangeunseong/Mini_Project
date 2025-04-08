using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float startTime;
    private static GameManager m_instance;
    private AudioSource audioSource;

    public CardController FirstCard, SecondCard;
    public Text TimeText;
    public AudioClip Clip;
    public GameObject EndText;
    public bool IsGameActive {  get; private set; }
    public int Wave { get; private set; }
    public int CardCount;
    public static GameManager Instance
    {
        get { 
            if(m_instance == null) 
                m_instance = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            return m_instance;
        }
    }

    // Awake is called once before Start Method
    void Awake()
    {
        if (!PlayerPrefs.HasKey("Wave")) Wave = 1;
        else Wave = PlayerPrefs.GetInt("Wave");

        if (Instance != this) Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = 0;
        IsGameActive = true;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsGameActive) { return; }
        if(CardCount <= 0) { IsGameActive = false; EndText.SetActive(true); PlayerPrefs.SetInt("Wave", Wave++); return; }
        if(startTime >= 30) { IsGameActive = false; EndText.SetActive(true); PlayerPrefs.SetInt("Wave", 1); return; }
        startTime += Time.deltaTime;
        UpdateTime();
    }

    void UpdateTime()
    {
        TimeText.text = startTime.ToString("N2");
    }

    public void MatchCards()
    {
        if(FirstCard.Index == SecondCard.Index) 
        {
            audioSource.PlayOneShot(Clip);
            FirstCard.DestroyCard(); SecondCard.DestroyCard();
            CardCount -= 2;
        }
        else
        {
            FirstCard.CloseCard(); SecondCard.CloseCard();    
        }
        FirstCard = SecondCard = null;
    }
}
