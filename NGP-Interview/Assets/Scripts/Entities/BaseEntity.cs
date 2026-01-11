using UnityEngine;
using System;
using EntityStats;
using System.Collections.Generic;
using Random = UnityEngine.Random;
namespace Entities
{
    public abstract class BaseEntity : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected Stats stats = new Stats();
        [SerializeField] protected float currentHealth;
        [SerializeField] protected float baseMaxHealth;

        public float MaxHealth => GetStatValue(StatType.MaxHealth);

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
        }
        #endregion

        #region Stats
        public float GetStatValue(StatType type) => stats.GetValue(type);
        public void AddStatModifier(StatModifier modifier)
        {
            stats.AddModifier(modifier);
        }
        #endregion
    }
}