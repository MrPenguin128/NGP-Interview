using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Scriptable Objects/Wave Object")]
public class WaveObject : ScriptableObject
{
    public EnemyWaveData[] enemyWaves;

    [Serializable]
    public class EnemyWaveData
    {
        public EnemyDataObject enemyObject;
        public int enemyAmount;
    }
}
