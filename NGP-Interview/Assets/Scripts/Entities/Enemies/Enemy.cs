using UnityEngine;
using UnityEngine.AI;
using Entities.StatsSystem;
using WaveSystem;
using System.Collections.Generic;
using InventorySystem;

namespace Entities.Enemies
{
    public class Enemy : BaseEntity
    {
        EnemyDataObject data;
        [Header("Components")]
        [SerializeField] NavMeshAgent agent;
        [SerializeField] DropSystem dropSystem;
        Transform target;
        float nextAttackTime;

        public float Damage => GetStatValue(StatType.Damage);
        public float MoveSpeed => GetStatValue(StatType.MoveSpeed);
        public float AttackSpeed => GetStatValue(StatType.AttackSpeed);
        public float AttackRange => GetStatValue(StatType.AttackRange);
        public float Armor => GetStatValue(StatType.Armor);
        public float CritChance => GetStatValue(StatType.CritChance);
        public float CritDamage => GetStatValue(StatType.CritDamage);

        protected override void Start()
        {
            base.Start();
            stats.GetStat(StatType.MoveSpeed).OnValueChanged += OnChangeMoveSpeed;
            OnChangeMoveSpeed(MoveSpeed);
            agent.stoppingDistance = AttackRange / 2;
        }
        protected override void Update()
        {
            base.Update();
            if (target == null)
            {
                FindTarget();
                return;
            }
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance > AttackRange)
                Chase();
            else
                Attack();
        }
        public void Initialize(EnemyDataObject data, Dictionary<StatType, float> baseStats)
        {
            this.data = data;
            stats.Initialize(baseStats);
            OnChangeMoveSpeed(MoveSpeed);
            CurrentHealth = MaxHealth;
        }
        protected override void Die()
        {
            base.Die();
            WaveManager.Instance.OnEnemyKilled(data, this);
            gameObject.SetActive(false);
            enabled = false;
            dropSystem.DropItem();
        }
        #region Stats
        protected virtual void OnChangeMoveSpeed(float obj)
        {
            agent.speed = obj;
        }
        #endregion
        #region Attack Behaviour
        protected virtual void Chase()
        {
            if (!agent.pathPending && (agent.destination - target.position).sqrMagnitude > 0.5f)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
        }

        protected virtual void Attack()
        {
            agent.isStopped = true;
            transform.LookAt(target.position);

            if (Time.time < nextAttackTime)
                return;

            nextAttackTime = Time.time + 1 / AttackSpeed;

            if (Random.Range(0, 100) < CritChance * 100)
                GameManager.Player.TakeDamage(Damage * CritDamage, true);
            else
                GameManager.Player.TakeDamage(Damage, false);
        }
        protected virtual void FindTarget()
        {
            if (GameManager.Player == null)
            {
                agent.isStopped = true;
                return;
            }

            target = GameManager.Player.transform;
        }
        #endregion
    }
}