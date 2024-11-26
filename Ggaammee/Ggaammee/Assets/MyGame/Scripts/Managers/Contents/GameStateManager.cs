using System;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    Normal, // 일반 상태
    InConversation, // 대화 중
    InMenu, // 메뉴 UI 오픈
    Paused // 게임 일시 정지
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