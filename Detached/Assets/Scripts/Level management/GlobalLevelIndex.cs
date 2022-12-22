using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalLevelIndex : MonoBehaviour
{
    //Static storage for all level names
    [SerializeField] private string[] Maps;
    [SerializeField] private static string[] levelNames;
    void Start()
    {
        levelNames = Maps;
        Debug.Log("Number of levels in levelNames: " + levelNames.Length);
    }

    public static string GetLevel(int levelNumber) => levelNames[levelNumber - 1];
    public static string GetLevelZeroIndex(int levelNumber) => levelNames[levelNumber];    
    public static string GetNextLevel()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levelNames.Length; i++)
        {
            if (i == levelNames.Length - 1)
                return levelNames[0];
            if (currentLevel == levelNames[i])
                return levelNames[i+1];
        }
        return currentLevel;
    }

    public static int GetLevelIndex()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levelNames.Length; i++)
        {
            if (currentLevel == levelNames[i])
                return i;
        }
        return 0;
    }
}
