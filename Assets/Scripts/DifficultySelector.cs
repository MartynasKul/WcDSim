using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DifficultyMode { Normal, Hard, Extreme }

public class DifficultySelector : MonoBehaviour
{
    public static DifficultySelector Instance;

    public DifficultyMode CurrentDifficulty { get; private set; } = DifficultyMode.Normal;

    // Spawn interval settings
    public float normalSpawnInterval = 15f;
    public float hardSpawnInterval = 10f;
    public float extremeSpawnInterval = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectNormalMode()
    {
        CurrentDifficulty = DifficultyMode.Normal;
        GameState.Instance.StartGame(GameMode.Normal);
        Debug.Log("Normal mode selected. Spawn interval: " + normalSpawnInterval);
    }

    public void SelectHardMode()
    {
        CurrentDifficulty = DifficultyMode.Hard;
        GameState.Instance.StartGame(GameMode.Hard);
        Debug.Log("Hard mode selected. Spawn interval: " + hardSpawnInterval);
    }

    public void SelectExtremeMode()
    {
        CurrentDifficulty = DifficultyMode.Extreme;
        GameState.Instance.StartGame(GameMode.Extreme);
        Debug.Log("Extreme mode selected. Spawn interval: " + extremeSpawnInterval);
    }

    public float GetSpawnInterval()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyMode.Hard:
                return hardSpawnInterval;
            case DifficultyMode.Extreme:
                return extremeSpawnInterval;
            case DifficultyMode.Normal:
            default:
                return normalSpawnInterval;
        }
    }
}
