﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomColor
{
    public string name;
    public Color color;

    public CustomColor(string name, Color color)
    {
        this.name = name;
        this.color = color;
    }

    public bool Equals(CustomColor customColor)
    {
        return name.Equals(customColor.name);
    }
}
