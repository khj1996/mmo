using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement
{
    private readonly PlayerController playerController;
    private readonly NavMeshAgent navMeshAgent;
    private readonly InputSystem inputSystem;
    private readonly Animator animator;
    private readonly Camera mainCamera;
    private readonly CreatureData creatureData;


    private float speed;
    private float targetSpeed;
    private float animationBlend;
    private float targetRotation;
    private float rotationVelocity;
    private float verticalVelocity;

    private float jumpTimeoutDelta = 0.9f;
    private float fallTimeoutDelta;
    private readonly float terminalVelocity = 53.0f;


    public PlayerMovement(PlayerController playerController, NavMeshAgent navMeshAgent, InputSystem inputSystem, Animator animator, CreatureData creatureData)
    {
        this.playerController = playerController;
        this.navMeshAgent = navMeshAgent;
        this.inputSystem = inputSystem;
        this.animator = animator;
        this.creatureData = creatureData;
        mainCamera = Camera.main;
    }

    public void Init()
    {
        jumpTimeoutDelta = playerController.JumpTimeout;
        fallTimeoutDelta = playerController.FallTimeout;
    }

    #region 이동

    public void Move()
    {
        SetTargetSpeed();

        if (playerController.isClimbing)
        {
            MoveLadder();
        }
        else if (!playerController.isAutoMove)
        {
            UpdateMovement();
            ApplyRotation();
            ApplyTranslation();
        }

        EventManager.TriggerPlayerMoved(playerController.transform.position);
    }

    public void MoveAuto(Vector3 destination)
    {
        DOTween.Sequence()
            .AppendCallback(() => NoticeTextUI.Instance.ShowText(ShowType.Timed, "자동이동 시작", 1.3f))
            .AppendInterval(0.7f)
            .AppendCallback(() => NoticeTextUI.Instance.ShowText(ShowType.Persistent, "자동이동 중"))
            .AppendCallback(() => StartAutoMove(destination));
    }

    private void StartAutoMove(Vector3 destination)
    {
        SetAutoMove(true);

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            playerController.autoMoveDestination = hit.position;
            navMeshAgent.SetDestination(hit.position);
        }
    }

    private void SetTargetSpeed()
    {
        float maxSpeed;

        if (playerController.isAutoMove)
        {
            maxSpeed = navMeshAgent.speed;
            if (!navMeshAgent.pathPending && navMeshAgent.hasPath)
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    DOTween.Sequence()
                        .AppendCallback(() => NoticeTextUI.Instance.StopPersistentText())
                        .AppendInterval(0.1f)
                        .AppendCallback(() => NoticeTextUI.Instance.ShowText(ShowType.Timed, "자동이동 종료"));

                    NoticeTextUI.Instance.StopPersistentText();

                    SetAutoMove(false);
                }
                else
                {
                    targetSpeed = navMeshAgent.speed;
                }
            }
        }
        else
        {
            maxSpeed = creatureData.sprintSpeed;

            if (inputSystem.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }
            else if (playerController.playerLockOn.isFindTarget || (inputSystem.crouch && playerController.isGrounded))
            {
                targetSpeed = creatureData.crouchSpeed;
            }
            else if (inputSystem.sprint)
            {
                targetSpeed = creatureData.sprintSpeed;
            }
            else
            {
                targetSpeed = creatureData.speed;
            }
        }

        animationBlend = Mathf.Lerp(
            animationBlend,
            playerController.isAutoMove ? navMeshAgent.speed : targetSpeed,
            playerController.isAutoMove ? Time.deltaTime * navMeshAgent.acceleration : Time.deltaTime * creatureData.acceleration
        );

        if (animationBlend < 0.01f) animationBlend = 0f;

        if (animator != null)
        {
            animator.SetFloat(AssignAnimationIDs.AnimIDSpeed, targetSpeed);

            float motionSpeedRatio = (maxSpeed > 0) ? animationBlend / maxSpeed : 0f;
            animator.SetFloat(AssignAnimationIDs.AnimIDMotionSpeed, motionSpeedRatio);
        }
    }

    private void UpdateMovement()
    {
        float currentHorizontalSpeed = new Vector3(playerController.controller.velocity.x, 0.0f, playerController.controller.velocity.z).magnitude;
        float inputMagnitude = inputSystem.analogMovement ? inputSystem.move.magnitude : 1f;

        speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * creatureData.acceleration);
        speed = Mathf.Round(speed * 1000f) / 1000f;
    }

    private void ApplyRotation()
    {
        Vector3 inputDirection = new Vector3(inputSystem.move.x, 0.0f, inputSystem.move.y).normalized;
        if (inputSystem.move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(playerController.transform.eulerAngles.y, targetRotation, ref rotationVelocity, playerController.RotationSmoothTime);

            if (!playerController.playerLockOn.isFindTarget)
            {
                playerController.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
    }

    private void ApplyTranslation()
    {
        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        playerController.controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

        if (!playerController.isAutoMove)
        {
            navMeshAgent.nextPosition = playerController.transform.position;
        }
    }

    private void MoveLadder()
    {
        playerController.isGrounded = true;
        playerController.LookAtTarget(playerController.targetTransform.transform.forward);

        var isMove = inputSystem.move != Vector2.zero;

        if (playerController.isAutoMove)
        {
            Debug.Log(playerController.isUpLadder);
            animator.SetBool(AssignAnimationIDs.AnimIDLadderUpPlay, playerController.isUpLadder);
            animator.SetBool(AssignAnimationIDs.AnimIDLadderDownPlay, !playerController.isUpLadder);
        }
        else
        {
            var move = inputSystem.move.y;
            animator.SetBool(AssignAnimationIDs.AnimIDLadderUpPlay, isMove && move >= 0);
            animator.SetBool(AssignAnimationIDs.AnimIDLadderDownPlay, isMove && move < 0);
        }

        EventManager.TriggerPlayerMoved(playerController.transform.position);
    }

    private void SetAutoMove(bool value)
    {
        if (value)
        {
            playerController.EnableNavMesh(playerController.transform.position);
        }
        else
        {
            playerController.DisableNavMesh();
        }

        playerController.isAutoMove = value;
    }

    #endregion


    #region 점프

    public void JumpAndGravity()
    {
        if (playerController.isGrounded)
        {
            ResetFallTimeout();

            if (animator)
            {
                ResetAnimatorJumpAndFall();
            }

            HandleGroundedVelocity();

            if (inputSystem.jump && jumpTimeoutDelta <= 0.0f)
            {
                PerformJump();
            }

            UpdateJumpTimeout();
        }
        else
        {
            HandleAirborneState();
        }

        ApplyGravity();
    }

    private void ResetFallTimeout()
    {
        fallTimeoutDelta = playerController.FallTimeout;
    }

    private void ResetAnimatorJumpAndFall()
    {
        animator.SetBool(AssignAnimationIDs.AnimIDJump, false);
        animator.SetBool(AssignAnimationIDs.AnimIDFreeFall, false);
    }

    private void HandleGroundedVelocity()
    {
        if (verticalVelocity < 0.0f)
        {
            verticalVelocity = -2f;
        }
    }

    private void PerformJump()
    {
        verticalVelocity = Mathf.Sqrt(-2.5f * creatureData.weight);
        animator?.SetBool(AssignAnimationIDs.AnimIDJump, true);


        inputSystem.jump = false;
    }

    private void UpdateJumpTimeout()
    {
        if (jumpTimeoutDelta >= 0.0f)
        {
            jumpTimeoutDelta -= Time.deltaTime;
        }
    }

    private void HandleAirborneState()
    {
        jumpTimeoutDelta = playerController.JumpTimeout;

        if (fallTimeoutDelta >= 0.0f)
        {
            fallTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            animator?.SetBool(AssignAnimationIDs.AnimIDFreeFall, true);
        }
    }

    private void ApplyGravity()
    {
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += creatureData.weight * Time.deltaTime;
        }
    }

    #endregion
}