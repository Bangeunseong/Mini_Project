using System.Collections;
using UnityEngine;

public class CardController : MonoBehaviour
{
    private AudioSource _audioSource;
    private Animator _animator;

    public SpriteRenderer Image;
    public GameObject Front;
    public GameObject Back;
    public GameObject Hint;
    public AudioClip Clip;

    public int Id { get; private set; }
    public int ParentId { get; private set; }
    public int Index { get; private set; }
    public int Category { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    public void Set(int category, int index)
    {
        MemberTable memberTable = TableManager.Instance.GetTable<MemberTable>();

        if (index < 10)
        {
            Id = -1;
            ParentId = index / 2;
            Category = category;
            Index = index;
            Image.sprite = memberTable.GetMemberInfoById(ParentId).PairOfImages[Category].Values[index % 2].Image;
        }
        else
        {
            Category = category;
            Id = (index % 10) / 2;
            ParentId = -1;
            Index = index;
            Image.sprite = memberTable.GetMemberInfoById(Id).Selfies[index % 2];
        }
    }

    public void Open()
    {
        if (!GameManager.Instance.IsGameActive) { return; }

        _audioSource.PlayOneShot(Clip);
        _animator.SetBool("IsOpen", true);
        Front.SetActive(true);
        Back.SetActive(false);
        if (GameManager.Instance.FirstCard == null) 
        {
            GameManager.Instance.FirstCard = this; 
        }
        else
        {
            GameManager.Instance.SecondCard = this;
            GameManager.Instance.MatchCards();
        }
    }

    IEnumerator DestroyCardRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void DestroyCard()
    {
        StartCoroutine(DestroyCardRoutine());
    }

    IEnumerator CloseCardRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("IsOpen", false);
        Front.SetActive(false);
        Back.SetActive(true);
    }

    public void CloseCard()
    {
        StartCoroutine(CloseCardRoutine());
    }
}
