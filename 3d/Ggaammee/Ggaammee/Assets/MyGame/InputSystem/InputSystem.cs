using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool crouch;
    public bool sprint;
    public bool attack;
    public bool interaction;
    public bool lockOn;

    public bool analogMovement;
    public bool cursorInputForLook = true;

    private GameStateManager gameStateManager;

    private void Start()
    {
        gameStateManager = Managers.GameStateManager;
    }

    private void LateUpdate()
    {
        ResetActions();
    }

    public void OnMove(InputValue value)
    {
        if (gameStateManager.CanPlayerInput())
            MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (gameStateManager.CanPlayerInput())
            LookInput(value.Get<Vector2>());
    }

    public void OnJump(InputValue value)
    {
        if (gameStateManager.CanPlayerInput())
            JumpInput(value.isPressed);
    }

    public void OnCrouch(InputValue value)
    {
        if (gameStateManager.CanPlayerInput())
            CrouchInput(value.isPressed);
    }

    public void OnAttack(InputValue value)
    {
        if (gameStateManager.CanPlayerInput())
            AttackInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        if (gameStateManager.CanPlayerInput())
            SprintInput(value.isPressed);
    }

    public void OnInterAction(InputValue value)
    {
        if (gameStateManager.CanPlayerInput())
            InterActionInput(value.isPressed);
    }

    public void OnLockOn(InputValue value)
    {
        if (gameStateManager.CanPlayerInput())
            LockOnInput(value.isPressed);
    }

    public void MoveInput(Vector2 newMoveDirection) => move = newMoveDirection;

    public void LookInput(Vector2 newLookDirection)
    {
        if (gameStateManager.CanPlayerInput() && cursorInputForLook)
        {
            look = newLookDirection;
        }
    }

    public void JumpInput(bool newJumpState) => jump = newJumpState;

    public void CrouchInput(bool newCrouchState) => crouch = newCrouchState;

    public void AttackInput(bool newAttackState)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        attack = newAttackState;
    }

    public void SprintInput(bool newValue) => sprint = newValue;

    public void InterActionInput(bool newValue) => interaction = newValue;

    public void LockOnInput(bool newValue) => lockOn = newValue;

    private void ResetActions()
    {
        if (!gameStateManager.CanPlayerInput())
        {
            move = Vector2.zero;
            look = Vector2.zero;
        }
        
        if (attack) attack = false;
        if (jump) jump = false;
        if (lockOn) lockOn = false;
        if (interaction) interaction = false;
    }
}
