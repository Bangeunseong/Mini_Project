using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMainController : MonoBehaviour
{
    public void GoMain()
    {
        SceneManager.LoadScene("Main");
    }
}
