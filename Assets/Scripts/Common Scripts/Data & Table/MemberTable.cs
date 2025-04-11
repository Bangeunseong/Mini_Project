using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MemberTable", menuName = "Scriptable Objects/MemberTable")]
public class MemberTable : ScriptableObject
{
    [SerializeField] private List<MemberInfo> _members = new List<MemberInfo>();
    public Dictionary<int, MemberInfo> MemberDicts = new();

    private void OnEnable()
    {
        foreach (MemberInfo m in _members)
        {
            MemberDicts[m.Id] = m;
        }
    }

    public MemberInfo GetMemberInfoById(int _id)
    {
        if(MemberDicts.TryGetValue(_id, out MemberInfo m)) return m;

        Debug.Log($"Invalid Member Id! : {_id}");
        return null;
    }
}
