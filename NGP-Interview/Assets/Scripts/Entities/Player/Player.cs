using UnityEngine;
using Entities.StatsSystem;
using System.Collections.Generic;
using InventorySystem;

namespace Entities.Player
{
    //Player entity class
    //Contains all the player Stats, Actions, and equipped Items
    public class Player : BaseEntity
    {
        Inventory inventory;
        [SerializeField] PlayerSettingsObject playerSettings;
        [SerializeField] PlayerMovement movement;
        [Header("Attack Settings")] 
        [SerializeField] LayerMask damageableMask;
        [SerializeField] ComboDataObject comboData;
        bool canAttack;
        int currentComboIndex;
        float currentAttackCooldown;
        float currentComboTime;
        [Header("Debug")]
        [SerializeField] bool drawGizmos = true;
        [SerializeField] bool drawAttackMesh = true;
        [SerializeField] int comboIndexDebug;

        #region Properties
        public PlayerSettingsObject PlayerSettings => playerSettings;
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
        public Inventory Inventory => inventory;
        #endregion
        protected override void Awake()
        {
            base.Awake();
            inventory = GetComponent<Inventory>();
            movement = GetComponent<PlayerMovement>();
        }
        protected override void Update()
        {
            base.Update();
            HandleComboReset();
            HandleAttackCooldown();
        }
        #region Stats
        public void Initialize()
        {
            CurrentHealth = MaxHealth;
        }
        protected override void InitializeStats()
        {
            stats.Initialize(new Dictionary<StatType, float>
            {
                { StatType.MaxHealth, baseMaxHealth },
                { StatType.Damage, PlayerSettings.BaseDamage },
                { StatType.MoveSpeed, PlayerSettings.BaseMoveSpeed },
                { StatType.AttackSpeed, PlayerSettings.BaseAttackSpeed },
                { StatType.AttackRange, PlayerSettings.BaseAttackRange },
                { StatType.Armor, PlayerSettings.BaseArmor },
                { StatType.CritChance, PlayerSettings.BaseCritChance },
                { StatType.CritDamage, PlayerSettings.BaseCritDamage },
            });
        }
        public override void TakeDamage(float amount, bool isCritical)
        {
            amount -= Armor;
            base.TakeDamage(Mathf.Max(amount, 1), isCritical);
        }
        #endregion
        #region Attack
        //Called from the Player Movement - Attack input method
        //Responsible to handle only the physics side of the attacks
        //Animations and movement are handled in the Player Movement method
        public void Attack()
        {
            if (!canAttack || comboData == null) return;
            canAttack = false;
            IsAttacking = true;
            Vector3 origin = transform.position;
            Vector3 forward = movement.RotateTowardsMouse();

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
                    if (Random.Range(0, 100) < CritChance * 100)
                        damageable.TakeDamage(damage * CritDamage, true);
                    else
                        damageable.TakeDamage(damage, false);
                    damageable.AddStatModifier(new StatModifier(
                        StatType.MoveSpeed,
                        slow,
                        StatModifier.StatModifierType.PercentAdd,
                        slowDuration
                        ));
                }
            }

            if (drawAttackMesh)
            {
                AttackDebugMesh.Instance.DrawArc(range, angle);
            }

            //Cicles through the hits
            currentComboIndex = (currentComboIndex + 1) % comboData.hits.Length;
            currentAttackCooldown = currentHit.HitDelay / AttackSpeed;
            currentComboTime = comboData.ComboResetTime + currentAttackCooldown;
            Invoke(nameof(ResetIsAttacking), currentAttackCooldown);
        }
        void ResetIsAttacking()
        {
            IsAttacking = false;
        }
        public void SetComboData(ComboDataObject comboData)
        {
            this.comboData = comboData;
        }
        void HandleAttackCooldown()
        {
            if (currentAttackCooldown > 0)
                currentAttackCooldown -= Time.deltaTime;
            else if (!canAttack)
            {
                canAttack = true;
            }
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
        #endregion

        #region Debug
        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) return;
            if (comboData == null || comboIndexDebug >= comboData.hits.Length || comboIndexDebug < 0) return;

            Gizmos.color = Color.red;
            var range = AttackRange > 0 ? AttackRange : PlayerSettings?.BaseAttackRange ?? 0;
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