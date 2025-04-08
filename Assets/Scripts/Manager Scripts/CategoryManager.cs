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

    public List<Button> Buttons;

    void Awake()
    {
        if(Instance != this) Destroy(gameObject);
    }

    void Start()
    {
        foreach(Button b in Buttons)
        {
            switch(b.name)
            {
                case "Food" : b.onClick.AddListener(()=> StartMainGame(0)); break;
                case "Game" : b.onClick.AddListener(()=>StartMainGame(1)); break;
                case "Hobby": b.onClick.AddListener(()=> StartMainGame(2)); break;
                case "Movie": b.onClick.AddListener(()=>StartMainGame(3)); break;
                default: Debug.LogError($"Does not match category name with button name : {b.name} "); break;
            }
        }    
    }

    private void StartMainGame(int category)
    {
        PlayerPrefs.SetInt("Category", category);
        SceneManager.LoadScene("Main");
    }
}
