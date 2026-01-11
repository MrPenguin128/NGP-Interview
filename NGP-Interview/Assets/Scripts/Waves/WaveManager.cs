using Entities.Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : Singleton<WaveManager>
{
    [SerializeField] WaveObject[] waves;
    [SerializeField] Transform[] spawners;
    [Header("Spawn Settings")]
    [SerializeField] float spawnInterval;
    int currentWave;
    int aliveEnemies;
    List<EnemyPoolItem> enemyPool = new List<EnemyPoolItem>();
    public int CurrentWave => currentWave;
    public int WaveLenght => waves.Length;

    public Action<int> OnWaveCompleted;
    public IEnumerator SpawnNextWave()
    {
        if (currentWave >= waves.Length) yield break;
        foreach (var wave in waves[currentWave].enemyWaves)
        {
            for (int i = 0; i < wave.enemyAmount; i++)
            {
                yield return new WaitUntil(() => aliveEnemies < GameManager.Instance.MaxEnemiesSpawned);
                SpawnEnemy(wave.enemyObject);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        currentWave++;
    }

    void SpawnEnemy(EnemyDataObject enemyObject)
    {
        Transform spawnPos = spawners[Random.Range(0, spawners.Length)];
        var enemy = GetEnemy(enemyObject, spawnPos);
        enemy.Initialize(enemyObject, enemyObject.GetBaseStats());
        aliveEnemies++;
    }
    Enemy GetEnemy(EnemyDataObject data, Transform spawnPos)
    {
        foreach (var item in enemyPool)
        {
            if (item.Data == data)
            {
                enemyPool.Remove(item);
                item.EnemyInstance.transform.position = spawnPos.position;
                item.EnemyInstance.enabled = true;
                item.EnemyInstance.gameObject.SetActive(true);
                return item.EnemyInstance;
            }
        }
        return Instantiate(data.EnemyPrefab, spawnPos.position, Quaternion.identity, transform);
    }
    public void OnEnemyKilled(EnemyDataObject data, Enemy prefab)
    {
        aliveEnemies--;
        enemyPool.Add(new EnemyPoolItem(data, prefab));
        if (aliveEnemies <= 0)
            WaveCompleted();
    }
    void WaveCompleted()
    {
        OnWaveCompleted?.Invoke(currentWave);
    }

    class EnemyPoolItem
    {
        public EnemyDataObject Data;
        public Enemy EnemyInstance;
        public bool IsAvailable => !EnemyInstance.gameObject.activeSelf;

        public EnemyPoolItem(EnemyDataObject data, Enemy enemyInstance)
        {
            Data = data;
            EnemyInstance = enemyInstance;
        }


    }
}
