using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoints : MonoBehaviour
{

    [SerializeField]
    private Transform[] _points;

    private void Start()
    {
        _points = GetComponentsInChildren<Transform>();
    }

    public int GetLength()
    {
        return _points.Length;
    }

    public Transform GetPointPos(int i)
    {
        return _points[i];
    }

}
