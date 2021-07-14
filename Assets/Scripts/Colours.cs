using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colours : MonoBehaviour
{

    public Color[] available;

    public Color GetColour(int colour, int tint)
    {
        return available[colour + tint];
    }
}
