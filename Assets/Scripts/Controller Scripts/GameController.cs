using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public void GoToStart()
    {
        StartCoroutine(AudioManager.Instance.FadeOutSound(0));
        SceneManager.LoadScene("Start");
    }

    public void GoToCategory()
    {
        StartCoroutine(AudioManager.Instance.FadeOutSound(0));
        SceneManager.LoadScene("Category");
    }

    public void GoToMainGame()
    {
        StartCoroutine(AudioManager.Instance.FadeOutSound(0));
        SceneManager.LoadScene("Main");
    }
}
