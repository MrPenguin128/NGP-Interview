using Entities.Player;
using System.Collections;
using System.ComponentModel.Design;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static Player Player { get; private set; }
    [Header("Waves Settings")]
    [SerializeField] int maxEnemiesSpawned;
    [SerializeField] float timeBetweenWaves;

    public int MaxEnemiesSpawned => maxEnemiesSpawned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WaveManager.Instance.OnWaveCompleted += CheckNextWave;
        StartCoroutine(StartWave());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void CheckNextWave(int currentWave)
    {
        if (currentWave < WaveManager.Instance.WaveLenght)
            StartCoroutine(StartWave());
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
        Debug.Log("You Win!");
    }
    //Static reference to the Player entity to maintain easy and simple access.
    #region Player Reference
    public static void RegisterPlayer(Player player)
    {
        if (Player != null && Player != player)
        {
            Debug.LogWarning($"Trying to register a second player {player.gameObject?.name}.");
            return;
        }
        Player = player;
    }
    public static void Unregister(Player player)
    {
        if (Player == player)
            Player = null;
    }
    #endregion

    public enum GameState
    {
        Waiting,
        EnemyWave,
        BetweenWaves,
        Ending
    }
}
