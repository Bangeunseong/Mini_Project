using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    private int _category;
    public GameObject CardPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PlayerPrefs.HasKey("Category")) { Debug.LogError("Cannot find key of category!"); _category = 0; }
        else _category = PlayerPrefs.GetInt("Category");

        SpawnCards();
    }

    void SpawnCards()
    {
        // Position
        int cardCountRow = 4, cardCountColumn = 5;
        int cardCount = cardCountRow * cardCountColumn;
        float xOffset = (-(cardCountRow * 1.3f + (cardCountRow - 1) * 0.1f) + 1.3f) / 2f;
        float yOffset = -4f;

        // Image Randomization
        int[] arr = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
        arr = arr.OrderBy(x => Random.Range(0f, 19f)).ToArray();

        Vector2 position = new Vector2(xOffset, yOffset);
        for (int i = 0; i < cardCount; i++)
        {
            if(i != 0 && (i % cardCountRow == 0))
            {
                position.x = xOffset;
                position.y += 1.4f;
            }
            GameObject card = Instantiate(CardPrefab, transform);
            card.transform.position = position;
            card.GetComponent<CardController>().Set(_category, arr[i]);
            position.x += 1.4f;
        }

        GameManager.Instance.CardCount = arr.Length;
    }
}
