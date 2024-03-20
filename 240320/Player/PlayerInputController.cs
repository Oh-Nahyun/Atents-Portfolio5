using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    /// <summary>
    /// 이동 입력을 전달하는 델리게이트 (이동 방향, 누른 상황인지 아닌지(true면 눌렀다))
    /// </summary>
    public Action<Vector2, bool> onMove;

    /// <summary>
    /// 이동 모드 변경 입력을 전달하는 델리게이트
    /// </summary>
    public Action onMoveModeChange;

    /// <summary>
    /// 공격 입력을 전달하는 델리게이트
    /// </summary>
    public Action onAttack;

    // 입력용 인풋 액션
    PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.MoveModeChange.performed += OnMoveModeChange;
        inputActions.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.MoveModeChange.performed -= OnMoveModeChange;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector3 input = context.ReadValue<Vector2>();
        onMove?.Invoke(input, !context.canceled);
    }

    private void OnMoveModeChange(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        onMoveModeChange?.Invoke();
    }

    private void OnAttack(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        onAttack?.Invoke();
    }
}

/// 실습_240308? (240312에 Player로 옮김)
/// 1. 플레이어가 이동하는 방향을 바라보기(카메라 기준)
/// 2. 이동하는 방향으로 턴 하는 모습이 보여야 한다.