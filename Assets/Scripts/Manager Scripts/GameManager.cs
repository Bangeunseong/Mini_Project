using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [SerializeField] private float _hintDelayTime = 0.5f;
    [SerializeField] private Sprite _off;
    [SerializeField] private Sprite _on;
    [SerializeField] private GameObject _hintPanel;
    [SerializeField] private List<GameObject> _hintObjects;
    [SerializeField] private Text _timeText;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private GameObject _endText;
    [SerializeField] private GameObject _hintButton;
    [SerializeField] private float endTime = 60f;

    private float startTime;
    private static GameManager m_instance;
    private AudioSource audioSource;
    
    public int CardCount;
    public CardController FirstCard, SecondCard;
    public bool IsHintActive { get; private set; } = false;
    public bool IsGameActive { get; private set; }
    public Category Category { get; private set; }
    
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

        // Initialize Hint Button Action
        Button button = Helper.GetComponentHelper<Button>(_hintButton);
        button.onClick.AddListener(() => {
            if (!IsHintActive) { 
                IsHintActive = !IsHintActive;
                Image img = Helper.GetComponentHelper<Image>(_hintButton);
                img.sprite = _on; 
                _hintPanel.SetActive(true); 
            }
            else {
                IsHintActive = !IsHintActive;
                Image img = Helper.GetComponentHelper<Image>(_hintButton);
                img.sprite = _on;
                _hintPanel.SetActive(false); 
            }
        });

        MemberTable memberTable = TableManager.Instance.GetTable<MemberTable>();
        // Initialize Hint Images
        for(int i = 0; i < 10; i++)
        {
            Image img = Helper.GetComponentHelper<Image>(_hintObjects[i]);
            img.sprite = memberTable.GetMemberInfoById(i / 2).PairOfImages[(int)Category].Values[i % 2].Image;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsGameActive) { return; }

        // If All cards destroyed, game ends.
        if(CardCount <= 0) 
        { 
            IsGameActive = false; 
            _endText.GetComponent<Text>().text = "¼º°ø!";
            _endText.SetActive(true);
            PlayerPrefs.SetFloat(Category.ToString(), startTime);
            return; 
        }
        
        // If Time passes over endTime, game ends.
        if(startTime >= endTime) 
        { 
            IsGameActive = false;
            _endText.GetComponent<Text>().text = "Âì..";
            _endText.SetActive(true);
            PlayerPrefs.SetFloat(Category.ToString(), startTime);
            return; 
        }

        // Update Time
        startTime += Time.deltaTime;

        // Update Time UI
        UpdateTime();
    }

    void UpdateTime()
    {
        _timeText.text = startTime.ToString("N2");
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
                audioSource.PlayOneShot(_clip);
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
                audioSource.PlayOneShot(_clip);
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
