using System;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    Normal, 
    CursorFree, 
    InConversation,
    InMenu, 
    Paused 
}

public class GameStateManager
{
    public GameState CurrentState { get; private set; } = GameState.Normal;

    public event Action OnChangeState;

    public GameStateManager()
    {
        UpdateCursorLock();
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        UpdateCursorLock();
        OnChangeState?.Invoke();
    }

    public bool CanPlayerInput()
    {
        return CurrentState == GameState.Normal;
    }

    public bool CanOpenPopUp()
    {
        return CurrentState != GameState.InConversation;
    }

    private void UpdateCursorLock()
    {
        bool cursorLocked = CurrentState == GameState.Normal;
        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !cursorLocked;
    }
}