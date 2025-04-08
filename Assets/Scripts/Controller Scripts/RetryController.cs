using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryController : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene("Category");
    }
}
