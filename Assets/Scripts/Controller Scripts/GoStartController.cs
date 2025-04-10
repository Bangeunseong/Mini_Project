using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoStartController : MonoBehaviour
{
    public void GoStart()
    {
        SceneManager.LoadScene("Start");
    }
}

