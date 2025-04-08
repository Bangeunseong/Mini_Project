using Unity.VisualScripting;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    [SerializeField] private MemberTable memberTable;
    private static TableManager _instance;
    public static TableManager Instance
    {
        get { if (_instance == null) _instance = GameObject.FindWithTag("TableManager").GetComponent<TableManager>(); return _instance; }
    }

    void Awake()
    {
        if (Instance != this) Destroy(gameObject);
    }

    public T GetTable<T>() where T : ScriptableObject
    {
        if (typeof(T) == typeof(MemberTable)) return memberTable as T;

        Debug.Log("Table does not exist!");
        return null;
    }
}
