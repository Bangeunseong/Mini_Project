using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class Pair
{
    public int Key;
    public List<ImageInfo> Values;
}

[Serializable] public class ImageInfo
{
    public int ParentId;
    public string Description;
    public Sprite Image;
}

[Serializable] public class MemberInfo
{
    public int Id;
    public string Name;
    public List<Sprite> Selfies;
    public List<Pair> PairOfImages;
}
