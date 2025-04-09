using UnityEngine;

public class Helper : MonoBehaviour
{
    public static T GetComponentHelper<T>(GameObject _obj) where T : Component
    {
        T _component = _obj.GetComponent<T>();
        if (_component == null)
        {
            Debug.LogError($"Component is null {typeof(T)} in {_obj.name}");
        }
        return _component;
    }
}

