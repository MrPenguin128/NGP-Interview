using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Constants
    string MOVE_ACTION_KEY = "Move";
    string ATTACK_ACTION_KEY = "Attack";
    string INTERACT_ACTION_KEY = "Interact";
    #endregion
    #region Variables
    [Header("Components")]
    [SerializeField] Rigidbody rb;
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    [Header("Attack")]
    [SerializeField] float attackMoveMultiplier;
    [SerializeField] float attackDuration;
    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    Vector2 moveInput;
    //Attack
    float attackTimer;
    bool isAttacking;
    //Dash
    bool isDashing;
    float dashTimer;
    float dashCooldownTimer;
    Vector3 dashDirection;
    #endregion
    #region Properties
    #endregion

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
        HandleAttackTimer();
    }

    #region Movement
    public void OnMove(InputAction.CallbackContext obj)
    {
        moveInput = obj.ReadValue<Vector2>();
    }
    //Move the player based on the input direction
    private void HandleMovement()
    {
        float speedMultiplier = isAttacking ? attackMoveMultiplier : 1f;

        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 velocity = moveDir * moveSpeed * speedMultiplier;
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }
    #endregion
    #region Rotation
    //Rotate te player based on the input direction or attack direction
    private void HandleRotation()
    {
        if (isAttacking)
        {
            RotateTowardsMouse();
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
    private void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (!groundPlane.Raycast(ray, out float distance))
            return;

        Vector3 targetPoint = ray.GetPoint(distance);
        Vector3 dir = targetPoint - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        rb.MoveRotation(Quaternion.Slerp(
            rb.rotation,
            targetRot,
            rotationSpeed * Time.fixedDeltaTime
        ));
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

        isAttacking = false;
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
        if (!context.started)
            return;

        isAttacking = true;
        attackTimer = attackDuration;
    }
    private void HandleAttackTimer()
    {
        if (!isAttacking)
            return;

        attackTimer -= Time.fixedDeltaTime;
        if (attackTimer <= 0f)
        {
            isAttacking = false;
        }
    }
    #endregion
}
