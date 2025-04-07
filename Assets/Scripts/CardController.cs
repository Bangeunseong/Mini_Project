using System.Collections;
using UnityEngine;

public class CardController : MonoBehaviour
{
    private AudioSource audioSource;
    private Animator animator;
    
    public int index { get; private set; }
    public SpriteRenderer image;
    public GameObject front;
    public GameObject back;
    public AudioClip clip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void Set(int index) { 
        this.index = index;
        image.sprite = Resources.Load<Sprite>($"rtan{index}");
    }

    public void Open()
    {
        if (!GameManager.instance.isGameActive) { return; }

        audioSource.PlayOneShot(clip);
        animator.SetBool("IsOpen", true);
        front.SetActive(true);
        back.SetActive(false);
        if (GameManager.instance.firstCard == null) GameManager.instance.firstCard = this;
        else
        {
            GameManager.instance.secondCard = this;
            GameManager.instance.MatchCards();
        }
    }

    IEnumerator DestroyCardRoutine()
    {
        yield return new WaitForSeconds(1f / GameManager.instance.wave);
        Destroy(gameObject);
    }

    public void DestroyCard()
    {
        StartCoroutine(DestroyCardRoutine());
    }

    IEnumerator CloseCardRoutine()
    {
        yield return new WaitForSeconds(1f / GameManager.instance.wave);
        animator.SetBool("IsOpen", false);
        front.SetActive(false);
        back.SetActive(true);
    }

    public void CloseCard()
    {
        StartCoroutine(CloseCardRoutine());
    }
}
