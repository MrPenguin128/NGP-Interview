using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using WaveSystem;
using UnityEngine;
using NUnit.Framework;
using InventorySystem;
using UI;

public class GameManager : Singleton<GameManager>
{
    public static Player Player => Instance.player;
    [Header("Waves Settings")]
    [SerializeField] int maxEnemiesSpawned;
    [SerializeField] float timeBetweenWaves;
    [SerializeField] Player player;
    public int MaxEnemiesSpawned => maxEnemiesSpawned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WaveManager.Instance.OnWaveCompleted += CheckNextWave;
        StartCoroutine(StartWave());
    }

    void CheckNextWave(int currentWave)
    {
        if (currentWave < WaveManager.Instance.WaveLenght)
        {
            SaveManager.Instance.Save();
            StartCoroutine(StartWave());
        }
        else
            GameEnding();
    }
    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(WaveManager.Instance.SpawnNextWave());
    }
    void GameEnding()
    {
        HUDManager.Instance.GameEnding();
    }

    public void LoadData(List<object> loadData)
    {
        foreach (var item in loadData)
        {
            switch (item)
            {
                case InventoryData inventoryData:
                    Player.Inventory.LoadData(inventoryData);
                    break;
                case WaveSpawnerData waveSpawnerData:
                    WaveManager.Instance.LoadData(waveSpawnerData);
                    break;
                default:
                    break;
            }
        }
    }

    public enum GameState
    {
        Waiting,
        EnemyWave,
        BetweenWaves,
        Ending
    }
}
