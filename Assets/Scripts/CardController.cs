using System.Collections;
using UnityEngine;

public class CardController : MonoBehaviour
{
    private AudioSource _audioSource;
    private Animator _animator;
    
    public int Index { get; private set; }
    public SpriteRenderer Image;
    public GameObject Front;
    public GameObject Back;
    public AudioClip Clip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    public void Set(int index) { 
        this.Index = index;
        Image.sprite = Resources.Load<Sprite>($"rtan{index}");
    }

    public void Open()
    {
        if (!GameManager.Instance.IsGameActive) { return; }

        _audioSource.PlayOneShot(Clip);
        _animator.SetBool("IsOpen", true);
        Front.SetActive(true);
        Back.SetActive(false);
        if (GameManager.Instance.FirstCard == null) GameManager.Instance.FirstCard = this;
        else
        {
            GameManager.Instance.SecondCard = this;
            GameManager.Instance.MatchCards();
        }
    }

    IEnumerator DestroyCardRoutine()
    {
        yield return new WaitForSeconds(1f / GameManager.Instance.Wave);
        Destroy(gameObject);
    }

    public void DestroyCard()
    {
        StartCoroutine(DestroyCardRoutine());
    }

    IEnumerator CloseCardRoutine()
    {
        yield return new WaitForSeconds(1f / GameManager.Instance.Wave);
        _animator.SetBool("IsOpen", false);
        Front.SetActive(false);
        Back.SetActive(true);
    }

    public void CloseCard()
    {
        StartCoroutine(CloseCardRoutine());
    }
}
