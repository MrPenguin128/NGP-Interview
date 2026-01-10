using UnityEngine;
using EntityStats;

namespace Entities.Player
{
    //Player entity class
    //Contains all the player Stats, Actions, and equipped Items
    public class Player : BaseEntity
    {
        [Header("Attack Settings")] 
        [SerializeField] LayerMask damageableMask;
        [SerializeField] float attackRange;
        [SerializeField, Range(0, 360)] float attackAngle;

        [Header("Debug")]
        [SerializeField] bool drawGizmos = true;

        protected override void Awake()
        {
            base.Awake();
            GameManager.RegisterPlayer(this);
        }

        //Called from the Player Movement - Attack input method
        //Responsible to handle only the physics side of the attacks
        //Animations and movement are handled in the Player Movement method
        public void Attack()
        {
            Vector3 origin = transform.position;
            Vector3 forward = transform.forward;

            Collider[] hits = Physics.OverlapSphere(origin, attackRange, damageableMask);

            foreach (var hit in hits)
            {
                if (!IsInAttackAngle(hit.transform, origin, forward))
                    continue;

                if (hit.TryGetComponent<BaseEntity>(out var damageable))
                {
                    damageable.TakeDamage(Damage);
                }
            }
        }


        /// <summary>
        /// Calculate if the target is inside the player attack angle
        /// </summary>
        private bool IsInAttackAngle(Transform target, Vector3 origin, Vector3 forward)
        {
            Vector3 directionToTarget = (target.position - origin).normalized;

            float angleToTarget = Vector3.Angle(forward, directionToTarget);

            return angleToTarget <= attackAngle;
        }


        #region Debug
        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Vector3 leftBoundary = Quaternion.Euler(0, -attackAngle * 0.5f, 0) * transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, attackAngle * 0.5f, 0) * transform.forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary * attackRange);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary * attackRange);
        }
        #endregion
    }
}