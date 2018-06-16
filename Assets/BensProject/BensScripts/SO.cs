﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapList",menuName ="ScriptObjects/MapList",order = 0)]
public class MapList : ScriptableObject {
    public List<Texture> list = new List<Texture>();
}
