using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CategoryManager : MonoBehaviour
{
    private static CategoryManager _instance;
    public static CategoryManager Instance 
    { 
        get 
        { 
            if (_instance == null) _instance = GameObject.FindWithTag("CategoryManager").GetComponent<CategoryManager>(); 
            return _instance; 
        } 
    }

    public List<GameObject> Buttons;
    public List<GameObject> Records;

    void Awake()
    {
        if(Instance != this) Destroy(gameObject);
    }

    void Start()
    {
        foreach(Category category in Enum.GetValues(typeof(Category)))
        {
            Button btn = Helper.GetComponentHelper<Button>(Buttons[(int)category]);
            Text record = Helper.GetComponentHelper<Text>(Records[(int)category]);
            btn.onClick.AddListener(() => StartCoroutine(FadeOutSound_BeforeGameStart(category, 0.4f)));
            record.text = PlayerPrefs.GetFloat(category.ToString(), 0f).ToString("N2");
        }   
    }

    private void StartMainGame(int category)
    {
        PlayerPrefs.SetInt("Category", category);
        SceneManager.LoadScene("Main");
    }

    private IEnumerator FadeOutSound_BeforeGameStart(Category category, float delay)
    {
        StartCoroutine(AudioManager.Instance.FadeOutSound(delay));
        yield return new WaitForSeconds(delay + 0.5f);
        StartMainGame((int)category);
    }
}
