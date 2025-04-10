using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Category
{
    Food,
    Game,
    Hobby,
    Movie
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sprite _off;
    [SerializeField] private Sprite _on;
    [SerializeField] private GameObject _hintPanel;
    [SerializeField] private List<GameObject> _hintObjects;
    [SerializeField] private GameObject _timeLabel;
    [SerializeField] private Text _timeText;
    [SerializeField] private GameObject _contdownText;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private GameObject _currentScore;
    [SerializeField] private GameObject _highScore;
    [SerializeField] private GameObject _hintButton;

    private float _startTime;
    private static GameManager _instance;
    private AudioSource _audioSource;
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
        get
        {
            if (_instance == null)
                _instance = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            return _instance;
        }
    }

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Category")) 
            Category = Category.Food;
        else 
            Category = (Category)PlayerPrefs.GetInt("Category");

        if (Instance != this) 
        {
            Destroy(gameObject);
            return;
        }

        _timeAnimator = Helper.GetComponentHelper<Animator>(_timeLabel);
        _hintPanelAnimator = Helper.GetComponentHelper<Animator>(_hintPanel);
        _endPanelAnimator = Helper.GetComponentHelper<Animator>(_endPanel);
    }

    void Start()
    {
        _startTime = 0f;
        _audioSource = Helper.GetComponentHelper<AudioSource>(gameObject);

        StartCoroutine(AudioManager.Instance.FadeInSound((int)Category + 1 , 0f));

        Button hintButton = Helper.GetComponentHelper<Button>(_hintButton);
        Image hintImage = Helper.GetComponentHelper<Image>(_hintButton);

        hintButton.onClick.AddListener(() =>
        {
            IsHintActive = !IsHintActive;
            hintImage.sprite = IsHintActive ? _on : _off;

            if (IsHintActive)
                StartCoroutine(MovePanelUp(_hintPanel, _hintPanelAnimator, 0.05f));
            else
                StartCoroutine(MovePanelDown(_hintPanel, _hintPanelAnimator, 0.7f));
        });

        MemberTable memberTable = TableManager.Instance.GetTable<MemberTable>();
        for (int i = 0; i < 10; i++)
        {
            Image img = Helper.GetComponentHelper<Image>(_hintObjects[i]);
            img.sprite = memberTable.GetMemberInfoById(i / 2).PairOfImages[(int)Category].Values[i % 2].Image;
        }

        StartCoroutine(ShowCountDown(5));
    }

    void Update()
    {
        if (!IsGameActive) return;

        if (CardCount <= 0)
        {
            EndGame();
            return;
        }

        _startTime += Time.deltaTime;
        UpdateTimeUI();
    }

    private void UpdateTimeUI()
    {
        _timeText.text = _startTime.ToString("N2");
    }

    private void EndGame()
    {
        if (_hintPanel.activeInHierarchy)
        {
            Image hintImage = Helper.GetComponentHelper<Image>(_hintButton);
            hintImage.sprite = _off;
            StartCoroutine(MovePanelDown(_hintPanel, _hintPanelAnimator, 0.7f));
        }

        Helper.GetComponentHelper<Button>(_hintButton).onClick.RemoveAllListeners();

        IsGameActive = false;
        _timeAnimator.SetBool("IsDown", false);

        float bestScore = PlayerPrefs.GetFloat(Category.ToString(), -1);
        if (bestScore > _startTime || bestScore < 0)
        {
            bestScore = _startTime;
            PlayerPrefs.SetFloat(Category.ToString(), _startTime);
        }

        _currentScore.GetComponent<Text>().text = $"현재 기록 : {_startTime.ToString("N2")}";
        _highScore.GetComponent<Text>().text = $"최고 기록 : {bestScore.ToString("N2")}";
        
        _endPanel.SetActive(true);
        _endPanelAnimator.SetBool("isUp", true);
    }

    public void MatchCards()
    {
        if (FirstCard == null || SecondCard == null)
            return;

        bool isFirstFace = FirstCard.Index >= 10;
        bool isSecondFace = SecondCard.Index >= 10;

        if ((isFirstFace && isSecondFace) || (!isFirstFace && !isSecondFace))
        {
            FirstCard.CloseCard();
            SecondCard.CloseCard();
        }
        else
        {
            bool isMatch = FirstCard.Id < 0 
                ? (SecondCard.Id == FirstCard.ParentId) 
                : (FirstCard.Id == SecondCard.ParentId);

            if (isMatch)
            {
                _audioSource.PlayOneShot(_clip);
                FirstCard.DestroyCard();
                SecondCard.DestroyCard();
                CardCount -= 2;
            }
            else
            {
                FirstCard.CloseCard();
                SecondCard.CloseCard();
            }
        }

        FirstCard = null;
        SecondCard = null;
    }

    private IEnumerator CountDown(int delay)
    {
        _timeAnimator.SetBool("IsDown", true);
        yield return new WaitForSeconds(delay);
        IsGameActive = true;
    }

    private IEnumerator MovePanelUp(GameObject go, Animator animator, float delay)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(delay);
        animator.SetBool("isUp", true);
    }

    private IEnumerator MovePanelDown(GameObject go, Animator animator, float delay)
    {
        animator.SetBool("isUp", false);
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
    }
    private IEnumerator ShowCountDown(int _delay)
    {
        Text text = Helper.GetComponentHelper<Text>(_contdownText);
        Animator animator = Helper.GetComponentHelper<Animator>(_contdownText);
        int delay = _delay;

        _timeAnimator.SetBool("IsDown", true);
        yield return new WaitForSeconds(1); //f를 붙여도 되지 않는 이유 조사

        while(delay >= 0)
        {
            //change text
            if(delay == 0) { text.text = "시작!"; delay--;}
            
            else{text.text = delay--.ToString();}

            animator.SetTrigger("Count_trig");

            yield return new WaitForSeconds(1);
        }

       
        _contdownText.SetActive(false);
        IsGameActive = true;
    }
}
