using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputSystem : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool crouch;
    public bool sprint;
    public bool attack;
    [FormerlySerializedAs("interAction")] public bool interaction;
    public bool lockOn;

    public bool analogMovement;

    public bool cursorLocked = true;
    public bool cursorInputForLook = true;


    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnCrouch(InputValue value)
    {
        CrouchInput(value.isPressed);
    }

    public void OnAttack(InputValue value)
    {
        AttackInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnInterAction(InputValue value)
    {
        InterActionInput(value.isPressed);
    }

    public void OnLockOn(InputValue value)
    {
        LockOnInput(value.isPressed);
    }
    
    public void OnCursorLock(InputValue value)
    {
        CursorLockInput();
    }


    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void CrouchInput(bool newCrouchState)
    {
        crouch = newCrouchState;
    }

    public void AttackInput(bool newAttackState)
    {
        attack = newAttackState;
    }

    public void SprintInput(bool newValue)
    {
        sprint = newValue;
    }

    public void InterActionInput(bool newValue)
    {
        interaction = newValue;
    }

    public void LockOnInput(bool newValue)
    {
        lockOn = newValue;
    }
    
    public void CursorLockInput()
    {
        cursorLocked = !cursorLocked;
        SetCursorState(cursorLocked);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}