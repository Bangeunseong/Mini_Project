using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public GameObject CardPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnCards();
    }

    void SpawnCards()
    {
        // Position
        int cardCountRow = 4;
        int cardCount = cardCountRow * cardCountRow;
        float xOffset = (-(cardCountRow * 1.3f + (cardCountRow - 1) * 0.1f) + 1.3f) / 2f;
        float yOffset = -3f;

        // Image Randomization
        int[] arr = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 };
        arr = arr.OrderBy(x => Random.Range(0f, 7f)).ToArray();

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
            card.GetComponent<CardController>().Set(arr[i]);
            position.x += 1.4f;
        }

        GameManager.Instance.CardCount = arr.Length;
    }
}
