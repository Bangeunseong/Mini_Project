using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public void GoToStart()
    {
        SceneManager.LoadScene("Start");
    }

    public void GoToCategory()
    {
        SceneManager.LoadScene("Category");
    }

    public void GoToMainGame()
    {
        SceneManager.LoadScene("Main");
    }
}
