using UnityEngine;
using EntityStats;
using System.Collections.Generic;

namespace Entities.Player
{
    //Player entity class
    //Contains all the player Stats, Actions, and equipped Items
    public class Player : BaseEntity
    {
        [Header("Base Stats Values")]
        [SerializeField] protected float baseDamage;
        [SerializeField] protected float baseMoveSpeed;
        [SerializeField] protected float baseAttackSpeed;
        [SerializeField] protected float baseAttackRange;
        [SerializeField] protected float baseArmor;
        [SerializeField] protected float baseCritChance;
        [SerializeField] protected float baseCritDamage;
        [Header("Attack Settings")] 
        [SerializeField] LayerMask damageableMask;
        [SerializeField] ComboDataObject comboData;
        bool canAttack;
        int currentComboIndex;
        float currentAttackCooldown;
        float currentComboTime;

        [Header("Debug")]
        [SerializeField] bool drawGizmos = true;
        [SerializeField] int comboIndexDebug;

        #region Properties
        #region Stats
        public float Damage => GetStatValue(StatType.Damage);
        public float MoveSpeed => GetStatValue(StatType.MoveSpeed);
        public float AttackSpeed => GetStatValue(StatType.AttackSpeed);
        public float AttackRange => GetStatValue(StatType.AttackRange);
        public float Armor => GetStatValue(StatType.Armor);
        public float CritChance => GetStatValue(StatType.CritChance);
        public float CritDamage => GetStatValue(StatType.CritDamage);
        #endregion
        public bool IsAttacking { get; private set; }
        #endregion
        protected override void Awake()
        {
            base.Awake();
            GameManager.RegisterPlayer(this);
        }
        protected override void Update()
        {
            base.Update();
            HandleComboReset();
            HandleAttackCooldown();
        }
        protected override void InitializeStats()
        {
            stats.Initialize(new Dictionary<StatType, float>
            {
                { StatType.MaxHealth, baseMaxHealth },
                { StatType.Damage, baseDamage },
                { StatType.MoveSpeed, baseMoveSpeed },
                { StatType.AttackSpeed, baseAttackSpeed },
                { StatType.AttackRange, baseAttackRange },
                { StatType.Armor, baseArmor },
                { StatType.CritChance, baseCritChance },
                { StatType.CritDamage, baseCritDamage },
            });
        }

        //Called from the Player Movement - Attack input method
        //Responsible to handle only the physics side of the attacks
        //Animations and movement are handled in the Player Movement method
        public void Attack()
        {
            if (!canAttack) return;
            canAttack = false;
            IsAttacking = true;
            Vector3 origin = transform.position;
            Vector3 forward = transform.forward;

            ComboHit currentHit = comboData.hits[currentComboIndex];
            var range = AttackRange * currentHit.AttackRangeMultiplier;
            var angle = currentHit.AttackAngle;
            var damage = Damage * currentHit.DamageMultiplier;
            var slow = currentHit.MoveSpeedModifier;
            var slowDuration = currentHit.HitDelay;
            Collider[] hits = Physics.OverlapSphere(origin, range, damageableMask);
            foreach (var hit in hits)
            {
                if (!IsInAttackAngle(hit.transform, origin, forward, angle))
                    continue;

                if (hit.TryGetComponent<BaseEntity>(out var damageable))
                {
                    if (Random.Range(0, 100) < CritChance)
                        damageable.TakeDamage(damage * CritDamage);
                    else
                        damageable.TakeDamage(damage);
                    damageable.AddStatModifier(new StatModifier(
                        StatType.MoveSpeed,
                        slow,
                        StatModifier.StatModifierType.PercentAdd,
                        slowDuration
                        ));
                }
            }
            //Cicles through the hits
            currentComboIndex = (currentComboIndex + 1) % comboData.hits.Length;
            currentAttackCooldown = 1f / AttackSpeed + currentHit.HitDelay;
            currentComboTime = comboData.ComboResetTime;
        }

        void HandleAttackCooldown()
        {
            if (currentAttackCooldown > 0)
                currentAttackCooldown -= Time.deltaTime;
            else if (!canAttack)
                canAttack = true;
        }
        void HandleComboReset()
        {
            if (currentComboTime > 0)
                currentComboTime -= Time.deltaTime;
            else if (currentComboIndex != 0)
                currentComboIndex = 0;
        }
        /// <summary>
        /// Calculate if the target is inside the player attack angle
        /// </summary>
        private bool IsInAttackAngle(Transform target, Vector3 origin, Vector3 forward, float attackAngle)
        {
            Vector3 directionToTarget = (target.position - origin).normalized;

            float angleToTarget = Vector3.Angle(forward, directionToTarget);

            return angleToTarget <= attackAngle;
        }


        #region Debug
        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) return;
            if (comboData == null || comboIndexDebug >= comboData.hits.Length || comboIndexDebug < 0) return;

            Gizmos.color = Color.red;
            var range = AttackRange > 0 ? AttackRange : baseAttackRange;
            range *= comboData.hits[comboIndexDebug].AttackRangeMultiplier;
            Gizmos.DrawWireSphere(transform.position, range);

            Vector3 leftBoundary = Quaternion.Euler(0, -comboData.hits[comboIndexDebug].AttackAngle * 0.5f, 0) * transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, comboData.hits[comboIndexDebug].AttackAngle * 0.5f, 0) * transform.forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary * range);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary * range);
        }
        #endregion
    }
}