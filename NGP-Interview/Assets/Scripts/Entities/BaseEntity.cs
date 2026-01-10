using UnityEngine;
using System;
using EntityStats;
using System.Collections.Generic;

namespace Entities
{
    public abstract class BaseEntity : MonoBehaviour
    {
        [Header("Base Stats")]
        protected Stats stats = new Stats();
        [SerializeField] protected float baseMaxHealth;
        [SerializeField] protected float baseDamage;
        [SerializeField] protected float baseMoveSpeed;
        protected float currentHealth;

        public float MaxHealth => GetStat(StatType.MaxHealth);
        public float Damage => GetStat(StatType.Damage);
        public float MoveSpeed => GetStat(StatType.MoveSpeed);

        public event Action OnDeath;
        public event Action<float> OnDamageTaken;

        protected virtual void Awake()
        {
            InitializeStats();
            currentHealth = MaxHealth;
        }
        protected virtual void Start()
        {
            
        }
        protected virtual void Update()
        {
            stats.Tick(Time.deltaTime);
        }
        protected virtual void OnDestroy()
        {
            
        }
        protected virtual void InitializeStats()
        {
            stats.Initialize(new Dictionary<StatType, float>
            {
                { StatType.MaxHealth, baseMaxHealth },
                { StatType.Damage, baseDamage },
                { StatType.MoveSpeed, baseMoveSpeed }
            });
        }

        #region Health
        public virtual void TakeDamage(float amount)
        {
            currentHealth -= amount;
            Debug.Log($"Ouch! current health: {currentHealth}");
            OnDamageTaken?.Invoke(amount);

            if (currentHealth <= 0f)
                Die();
        }

        protected virtual void Die()
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
        #endregion

        #region Stats
        public float GetStat(StatType type) => stats.Get(type);
        public void AddStatModifier(StatModifier modifier)
        {
            stats.AddModifier(modifier);
        }
        #endregion
    }
}