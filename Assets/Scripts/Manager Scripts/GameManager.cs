using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Sprite _off;
    [SerializeField] private Sprite _on;
    [SerializeField] private GameObject _hintPanel;
    [SerializeField] private List<GameObject> _hintObjects;
    [SerializeField] private GameObject _timeLabel;
    [SerializeField] private Text _timeText;
    [SerializeField] private GameObject _countDownText;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private GameObject _currentScore;
    [SerializeField] private GameObject _highScore;
    [SerializeField] private GameObject _hintButton;
    // [SerializeField] private float endTime = 60f;

    private float startTime;
    private static GameManager m_instance;
    private AudioSource audioSource;
    private Animator _timeAnimator;
    private Animator _hintPanelAnimator;
    private Animator _endPanelAnimator;

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

        // Initialize Time Label Animator, Hint Panel Animator
        _timeAnimator = Helper.GetComponentHelper<Animator>(_timeLabel);
        _hintPanelAnimator = Helper.GetComponentHelper<Animator>(_hintPanel);
        _endPanelAnimator = Helper.GetComponentHelper<Animator>(_endPanel);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Init. Attributes
        startTime = 0;
        audioSource = Helper.GetComponentHelper<AudioSource>(gameObject);
        
        // Fade In Audio Sound
        if(AudioManager.Instance.IsAudioSourceChanged)
            StartCoroutine(AudioManager.Instance.FadeInSound((int)Category + 1, 0.5f));

        // Initialize Hint Button Action
        Button button = Helper.GetComponentHelper<Button>(_hintButton);
        button.onClick.AddListener(() => {
            if(!IsGameActive) { return; }

            if (!IsHintActive) { 
                IsHintActive = !IsHintActive;
                Image img = Helper.GetComponentHelper<Image>(_hintButton);
                img.sprite = _on;

                // Activate Hint Panel and Move Up
                StartCoroutine(MovePanelUp(_hintPanel, _hintPanelAnimator, 0.05f));
            }
            else {
                IsHintActive = !IsHintActive;
                Image img = Helper.GetComponentHelper<Image>(_hintButton);
                img.sprite = _off;

                // Move Hint Panel Down and Deactivate
                StartCoroutine(MovePanelDown(_hintPanel, _hintPanelAnimator, 0.7f));
            }
        });

        MemberTable memberTable = TableManager.Instance.GetTable<MemberTable>();
        // Initialize Hint Images
        for(int i = 0; i < 10; i++)
        {
            Image img = Helper.GetComponentHelper<Image>(_hintObjects[i]);
            img.sprite = memberTable.GetMemberInfoById(i / 2).PairOfImages[(int)Category].Values[i % 2].Image;
        }

        // Start Game After 5 seconds and move down time label
        StartCoroutine(ShowCountDown(5));
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsGameActive) { return; }

        // If All cards destroyed, game ends.
        if(CardCount <= 0) 
        {
            // If HintPanel is turned on even if the game ends, close the hint panel
            if (_hintPanel.activeInHierarchy)
            { 
                Image img = Helper.GetComponentHelper<Image>(_hintButton);
                img.sprite = _off;
                
                StartCoroutine(MovePanelDown(_hintPanel, _hintPanelAnimator, 0.7f));
            }
            
            // Set Game as inactive, disable time text, and move up time label
            IsGameActive = false;
            _timeAnimator.SetBool("IsDown_b", false);
            
            // Evaluate which score is best, then refresh Text UI
            float bestScore = PlayerPrefs.GetFloat(Category.ToString(), -1);
            if (bestScore > startTime || bestScore < 0) { 
                bestScore = startTime;
                PlayerPrefs.SetFloat(Category.ToString(), startTime); 
            }
            
            _currentScore.GetComponent<Text>().text = $"현재 기록 : {startTime.ToString("N2")}";
            _highScore.GetComponent<Text>().text = $"최고 기록 : {bestScore.ToString("N2")}";

            // Activate EndPanel
            StartCoroutine(MovePanelUp(_endPanel, _endPanelAnimator, 0.1f));
            return; 
        }
        
        // Deprecated
        // If Time passes over endTime, game ends.
        /*if(startTime >= endTime) 
        { 
            IsGameActive = false;
            _endText.GetComponent<Text>().text = "쯧..";
            _endText.SetActive(true);
            PlayerPrefs.SetFloat(Category.ToString(), startTime);
            return; 
        }*/

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

    private IEnumerator MovePanelUp(GameObject go, Animator animator, float _delay)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(_delay);
        animator.SetBool("IsUp_b", true);
    }

    private IEnumerator MovePanelDown(GameObject go, Animator animator, float _delay)
    {
        animator.SetBool("IsUp_b", false);
        yield return new WaitForSeconds(_delay);
        go.SetActive(false);
    }

    private IEnumerator ShowCountDown(int _delay)
    {
        Text text = Helper.GetComponentHelper<Text>(_countDownText);
        Animator animator = Helper.GetComponentHelper<Animator>(_countDownText);
        int delay = _delay;

        _timeAnimator.SetBool("IsDown_b", true);
        yield return new WaitForSeconds(1.5f);
        
        while(delay >= 0)
        {
            // Change Text
            if (delay == 0) { text.text = "시작!"; delay--; }
            else {
                Debug.Log($"Countdown : {delay}");
                text.text = delay--.ToString();
            }

            // Reset and Set Trigger to play animation
            animator.ResetTrigger("Count_trig");
            animator.SetTrigger("Count_trig");
            yield return new WaitForSeconds(1.1f);
        }

        _countDownText.SetActive(false);
        IsGameActive = true;
    }
}
