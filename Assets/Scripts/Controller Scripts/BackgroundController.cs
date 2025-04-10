using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _backgrounds;
    [SerializeField] private List<Material> _materials;
    [SerializeField] private float _speed = 5f;
    private Category _category;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _category = GameManager.Instance.Category;
        switch(_category)
        {
            case Category.Food: foreach(GameObject go in _backgrounds) { go.GetComponent<MeshRenderer>().material = _materials[(int)Category.Food]; } break;
            case Category.Hobby: foreach(GameObject go in _backgrounds) { go.GetComponent<MeshRenderer>().material = _materials[(int)Category.Hobby]; } break;
            case Category.Game: foreach(GameObject go in _backgrounds) { go.GetComponent<MeshRenderer>().material = _materials[(int)Category.Game]; } break;
            case Category.Movie: foreach(GameObject go in _backgrounds) { go.GetComponent<MeshRenderer>().material = _materials[(int)Category.Movie]; } break;
            default: break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGameActive) { return; }

        MoveBackground();
    }

    private void MoveBackground()
    {
        if (transform.position.y < -10) transform.position = Vector3.zero;

        transform.position += Vector3.down * _speed * Time.deltaTime;
    }
}
