using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLevelIndex : MonoBehaviour
{
    //Static storage for all level names
    [SerializeField] private string[] Maps;
    public static string[] levelNames;
    void Start()
    {
        levelNames = Maps;
    }
}
