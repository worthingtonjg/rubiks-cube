using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Slice
{
    public List<GameObject> Cubies { get; set; }

    public Slice()
    {
        Cubies = new List<GameObject>();
    }
}