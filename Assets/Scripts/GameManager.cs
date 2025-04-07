using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float startTime;
    private static GameManager m_instance;
    private AudioSource audioSource;

    public CardController firstCard, secondCard;
    public Text timeText;
    public AudioClip clip;
    public GameObject endText;
    public bool isGameActive {  get; private set; }
    public int wave { get; private set; }
    public int cardCount;
    public static GameManager instance
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
        if (!PlayerPrefs.HasKey("Wave")) wave = 1;
        else wave = PlayerPrefs.GetInt("Wave");

        if (instance != this) Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = 0;
        isGameActive = true;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameActive) { return; }
        if(cardCount <= 0) { isGameActive = false; endText.SetActive(true); PlayerPrefs.SetInt("Wave", wave++); return; }
        if(startTime >= 30) { isGameActive = false; endText.SetActive(true); PlayerPrefs.SetInt("Wave", 1); return; }
        startTime += Time.deltaTime;
        UpdateTime();
    }

    void UpdateTime()
    {
        timeText.text = startTime.ToString("N2");
    }

    public void MatchCards()
    {
        if(firstCard.index == secondCard.index) 
        {
            audioSource.PlayOneShot(clip);
            firstCard.DestroyCard(); secondCard.DestroyCard();
            cardCount -= 2;
        }
        else
        {
            firstCard.CloseCard(); secondCard.CloseCard();    
        }
        firstCard = secondCard = null;
    }
}
