using UnityEngine;
using UnityEngine.UI;

public enum Category
{
    Food,
    Game,
    Hobby,
    Movie,
}

public class GameManager : MonoBehaviour
{
    private float startTime;
    private static GameManager m_instance;
    private AudioSource audioSource;

    public CardController FirstCard, SecondCard;
    public Text TimeText;
    public AudioClip Clip;
    public GameObject EndText;
    public bool IsGameActive { get; private set; }
    public int Wave { get; private set; }
    public Category Category { get; private set; }
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
        // Initialize Wave Attribute
        if (!PlayerPrefs.HasKey("Wave")) Wave = 1;
        else Wave = PlayerPrefs.GetInt("Wave");

        // Initialize Category Attribute
        if (!PlayerPrefs.HasKey("Category")) Category = Category.Food;
        else Category = (Category)PlayerPrefs.GetInt("Category");

        // If this gameobject is not belong to previously assigned GameManager, destroy it to prevent double init.
        if (Instance != this) Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Init. Attributes
        startTime = 0;
        IsGameActive = true;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsGameActive) { return; }

        // If All cards destroyed, game ends.
        if(CardCount <= 0) { IsGameActive = false; EndText.GetComponent<Text>().text = "¼º°ø!"; EndText.SetActive(true); PlayerPrefs.SetInt("Wave", Wave++); return; }
        
        // If Time passes over 30 seconds, game ends. -> Need modification ( Change static value(which is 30) to dynamic value )
        if(startTime >= 30) 
        { 
            IsGameActive = false;
            EndText.GetComponent<Text>().text = "Âì..";
            EndText.SetActive(true); 
            PlayerPrefs.SetInt("Wave", 1); 
            return; 
        }

        // Update Time
        startTime += Time.deltaTime;

        // Update Time UI
        UpdateTime();
    }

    void UpdateTime()
    {
        TimeText.text = startTime.ToString("N2");
    }

    public void MatchCards()
    {
        // If both cards are all face images or category images, close cards and reset memory.
        if ((FirstCard.Index >= 10 && SecondCard.Index >= 10) || (FirstCard.Index < 10 && SecondCard.Index < 10)) 
        { 
            FirstCard.CloseCard(); SecondCard.CloseCard(); 
            FirstCard = SecondCard = null;
            return;
        }

        // When FirstCard is not face images,
        // Compare Second Card (Id -> Member Id) and First Card (Parent Id -> Connected Member Id)
        if(FirstCard.Id < 0)
        {
            if(SecondCard.Id == FirstCard.ParentId)
            {
                audioSource.PlayOneShot(Clip);
                FirstCard.DestroyCard(); SecondCard.DestroyCard();
                CardCount -= 2;
            }
            else FirstCard.CloseCard(); SecondCard.CloseCard();
        }
        // When SecondCard is not face images,
        // Compare First Card (Parent Id -> Paired Member Id) and Second Card (Id -> Member Id)
        else
        {
            if (FirstCard.Id == SecondCard.ParentId)
            {
                // Play Audio and Destroy Cards
                audioSource.PlayOneShot(Clip);
                FirstCard.DestroyCard(); SecondCard.DestroyCard();
                CardCount -= 2;
            }
            // Close Cards
            else FirstCard.CloseCard(); SecondCard.CloseCard();
        }

        // Reset Memory
        FirstCard = SecondCard = null;
    }
}
