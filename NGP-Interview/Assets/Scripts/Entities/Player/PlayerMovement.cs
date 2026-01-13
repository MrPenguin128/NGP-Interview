using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        #region Variables
        [Header("Components")]
        Player player;
        [SerializeField] Rigidbody rb;
        [Header("Movement")]
        [SerializeField] float rotationSpeed;
        [Header("Dash")]
        [SerializeField] float dashSpeed;
        [SerializeField] float dashDuration;
        [SerializeField] float dashCooldown;
        Vector2 moveInput;
        //Dash
        bool isDashing;
        float dashTimer;
        float dashCooldownTimer;
        Vector3 dashDirection;
        //Attack
        bool canAttack;
        #endregion
        #region Properties
        #endregion

        private void Start()
        {
            player = GameManager.Player;
        }
        private void Update()
        {
            canAttack = !UIUtils.IsPointerOverUI();
        }
        private void FixedUpdate()
        {
            HandleDashCooldown();
            if (isDashing)
            {
                HandleDash();
                return;
            }
            HandleMovement();
            HandleRotation();
        }

        #region Movement
        public void OnMove(InputAction.CallbackContext obj)
        {
            moveInput = obj.ReadValue<Vector2>();
        }
        //Move the player based on the input direction
        private void HandleMovement()
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
            Vector3 velocity = moveDir * player.MoveSpeed;
            velocity.y = rb.linearVelocity.y;

            rb.linearVelocity = velocity;
        }
        #endregion
        #region Rotation
        //Rotate te player based on the input direction or attack direction
        private void HandleRotation()
        {
            if (player.IsAttacking)
            {
                return;
            }
            if (moveInput.sqrMagnitude < 0.01f)
                return;

            Vector3 lookDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            Quaternion targetRot = Quaternion.LookRotation(lookDir);

            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                targetRot,
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
        //Rotates the player towards the mouse position, which will be the attack direction
        public Vector3 RotateTowardsMouse()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            if (!groundPlane.Raycast(ray, out float distance))
                return Vector3.zero;

            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 dir = targetPoint - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.01f)
                return dir;

            Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                targetRot,
                10
            ));
            return dir;
        }
        #endregion
        #region Dash
        public void OnDash(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            if (isDashing || dashCooldownTimer > 0f)
                return;

            StartDash();
        }

        private void StartDash()
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            if (moveInput.sqrMagnitude > 0.01f)
                dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            else
                dashDirection = transform.forward;
        }

        private void HandleDash()
        {
            rb.linearVelocity = dashDirection * dashSpeed;

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }

        private void HandleDashCooldown()
        {
            if (dashCooldownTimer > 0f)
                dashCooldownTimer -= Time.fixedDeltaTime;
        }
        #endregion
        #region Attack
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.started || !canAttack)
                return;
            player.Attack();
        }
        #endregion
    }
}