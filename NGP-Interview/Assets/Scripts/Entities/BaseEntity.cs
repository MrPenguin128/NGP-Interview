using UnityEngine;
using System;
using Entities.StatsSystem;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Entities
{
    public abstract class BaseEntity : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected Animator anim;
        [Header("Stats")]
        [SerializeField] protected Stats stats = new Stats();
        [SerializeField] protected float currentHealth;
        [SerializeField] protected float baseMaxHealth;
        bool dead;
        public float MaxHealth => GetStatValue(StatType.MaxHealth);

        public float CurrentHealth 
        { 
            get => currentHealth;
            protected set
            {
                currentHealth = value;
                OnChangeCurrentHealth?.Invoke(currentHealth);
            }
        }

        public event Action OnDeath;
        public event Action<float> OnDamageTaken;
        public event Action<float> OnChangeCurrentHealth;

        protected virtual void Awake()
        {
            InitializeStats();
            CurrentHealth = MaxHealth;
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
        public virtual void TakeDamage(float amount, bool isCritical)
        {
            CurrentHealth -= amount;
            OnDamageTaken?.Invoke(amount);
            DamagePopup.Create(transform.position, amount, isCritical);
            if (CurrentHealth <= 0f)
                Die();
        }

        protected virtual void Die()
        {
            if (dead) return;
            dead = true;
            OnDeath?.Invoke();
        }
        #endregion

        #region Stats
        public StatValue GetStat(StatType type) => stats.GetStat(type);
        public float GetStatValue(StatType type) => stats.GetValue(type);
        public List<Stats.StatPair> GetAllStats() => stats.AllStats;
        public void AddStatModifier(StatModifier modifier)
        {
            stats.AddModifier(modifier);
        }
        public void RemoveStatModifier(StatModifier modifier)
        {
            stats.RemoveModifier(modifier);
        }
        #endregion
    }
}