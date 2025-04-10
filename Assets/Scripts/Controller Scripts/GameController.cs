using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public void FromStartToCategory()
    {
        SceneManager.LoadScene("Category");
    }

    public void GoToStart()
    {
        StartCoroutine(AudioManager.Instance.FadeOutSound(0));
        SceneManager.LoadScene("Start");
        StartCoroutine(AudioManager.Instance.FadeInSound(0, 0));
    }

    public void GoToCategory()
    {
        StartCoroutine(AudioManager.Instance.FadeOutSound(0));
        SceneManager.LoadScene("Category");
        StartCoroutine(AudioManager.Instance.FadeInSound(0, 0));
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Main");
    }
}
