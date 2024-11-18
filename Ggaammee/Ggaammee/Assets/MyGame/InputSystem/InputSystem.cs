using System;
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

    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private void LateUpdate()
    {
        ResetActions();
    }

    public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());
    public void OnLook(InputValue value) => LookInput(value.Get<Vector2>());
    public void OnJump(InputValue value) => JumpInput(value.isPressed);
    public void OnCrouch(InputValue value) => CrouchInput(value.isPressed);
    public void OnAttack(InputValue value) => AttackInput(value.isPressed);
    public void OnSprint(InputValue value) => SprintInput(value.isPressed);
    public void OnInterAction(InputValue value) => InterActionInput(value.isPressed);
    public void OnLockOn(InputValue value) => LockOnInput(value.isPressed);
    public void OnCursorLock(InputValue value) => CursorLockInput();

    public void MoveInput(Vector2 newMoveDirection) => move = newMoveDirection;

    public void LookInput(Vector2 newLookDirection)
    {
        if (cursorInputForLook)
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
        Cursor.visible = !newState;
    }

    private void ResetActions()
    {
        if (attack) attack = false;
        if (jump) jump = false;
        if (lockOn) lockOn = false;
        if (interaction) interaction = false;
    }
}