using UnityEngine;
using UnityEngine.SceneManagement;

public class GoCategoryController : MonoBehaviour
{
    public void GoCategory()
    {
        SceneManager.LoadScene("Category");
    }
}
