using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : CreatureController
{
    [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;

    [Space(10)] public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;

    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public bool Grounded = true;
    public bool isClimbing = false;


    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private CreatureStateMachine<PlayerController> creatureStateMachine;
    private InputSystem _input;
    private GameObject _mainCamera;
    private LockOn _lockOn;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }


    #region 초기화

    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        InitFSM();
        InitComponent();

        hp = creatureData.maxHp;
        _hpBar.UpdateHealthBar(hp, creatureData.maxHp);
        Managers.ObjectManager.RegisterPlayer(this);
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void InitFSM()
    {
        creatureStateMachine = new CreatureStateMachine<PlayerController>();

        creatureStateMachine.AddState(new PlayerData.IdleAndMoveState(this));
        creatureStateMachine.AddState(new PlayerData.CrouchState(this));
        creatureStateMachine.AddState(new PlayerData.JumpState(this));
        creatureStateMachine.AddState(new PlayerData.GetHitState(this));
        creatureStateMachine.AddState(new PlayerData.LadderState(this));

        #region 상태 전이 조건

        #region IdleState

        creatureStateMachine.AddTransition<PlayerData.IdleAndMoveState, PlayerData.CrouchState>(() => !_lockOn.isFindTarget && _input.crouch);
        creatureStateMachine.AddTransition<PlayerData.IdleAndMoveState, PlayerData.JumpState>(() => !_lockOn.isFindTarget && _AttackCoroutine == null && Grounded && !_input.crouch && _input.jump);
        creatureStateMachine.AddTransition<PlayerData.IdleAndMoveState, PlayerData.LadderState>(() => isClimbing);

        #endregion


        #region CrouchState

        creatureStateMachine.AddTransition<PlayerData.CrouchState, PlayerData.IdleAndMoveState>(() => !_input.crouch || !Grounded);

        #endregion

        #region JumpState

        creatureStateMachine.AddTransition<PlayerData.JumpState, PlayerData.IdleAndMoveState>(() => Grounded);

        #endregion

        #region LadderState

        creatureStateMachine.AddTransition<PlayerData.LadderState, PlayerData.IdleAndMoveState>(() => !isClimbing);

        #endregion

        #endregion

        creatureStateMachine.ChangeState(typeof(PlayerData.IdleAndMoveState));
    }

    private void InitComponent()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<InputSystem>();
        _lockOn = GetComponent<LockOn>();
    }

    #endregion


    private void Update()
    {
        Debug.Log(creatureStateMachine.CurrentState);
        creatureStateMachine.Update();
    }


    #region 이동

    public void MoveLadder()
    {
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        float move = targetDirection.z;
        bool isMove = true;

        targetDirection.x = 0;
        targetDirection.z = 0;

        Grounded = true;

        LookAtTarget(_targetTransform.transform.forward);
        if (_input.move == Vector2.zero)
            isMove = false;

        if (move < 0)
            animator.SetBool(AssignAnimationIDs.AnimIDLadderUpPlay, isMove);
        else
            animator.SetBool(AssignAnimationIDs.AnimIDLadderDownPlay, isMove);
    }

    public void Move()
    {
        if (isClimbing)
        {
            isClimbing = true;
            return;
        }

        float targetSpeed = _input.sprint ? creatureData.sprintSpeed : creatureData.speed;
        targetSpeed = (_input.crouch && Grounded || _lockOn.isFindTarget) ? creatureData.crouchSpeed : targetSpeed;
        targetSpeed = _input.move == Vector2.zero ? 0.0f : targetSpeed;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * creatureData.acceleration);
        _speed = Mathf.Round(_speed * 1000f) / 1000f;

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * creatureData.acceleration);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            if (!_lockOn.isFindTarget)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        if (_hasAnimator)
        {
            animator.SetFloat(AssignAnimationIDs.AnimIDSpeed, _animationBlend);
            animator.SetFloat(AssignAnimationIDs.AnimIDMotionSpeed, inputMagnitude);
        }
    }

    public void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;

            if (_hasAnimator)
            {
                animator.SetBool(AssignAnimationIDs.AnimIDJump, false);
                animator.SetBool(AssignAnimationIDs.AnimIDFreeFall, false);
            }

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(-2.5f * creatureData.weight);

                if (_hasAnimator)
                {
                    animator.SetBool(AssignAnimationIDs.AnimIDJump, true);
                }

                _input.jump = false;
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (_hasAnimator)
                {
                    animator.SetBool(AssignAnimationIDs.AnimIDFreeFall, true);
                }
            }
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += creatureData.weight * Time.deltaTime;
        }
    }

    #endregion

    #region 공격

    private Coroutine _AttackCoroutine = null;

    public void CheckAttack()
    {
        if (_AttackCoroutine != null || !_input.attack) return;

        _input.attack = false;
        _AttackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        animator.SetTrigger(AssignAnimationIDs.AnimIDAttackTrigger);
        animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, 1);

        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
        {
            yield return null;
        }

        animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, 0);
        _AttackCoroutine = null;
    }

    #endregion

    #region 사다리

    private bool isNearLadder = false;
    private bool isUpLadder = false;
    private bool isDownLadder = false;


    private void NearLadderPosition(Collider other)
    {
        isNearLadder = true;
        _targetTransform = other.gameObject.transform;
    }

    private void EndofLadder(int animName)
    {
        if (isClimbing)
        {
            animator.SetTrigger(animName);
            animator.SetBool(AssignAnimationIDs.AnimIDLadder, false);
        }
    }

    public void CharacterToLadder()
    {
        LockAtTargetPsition();
        LookAtTarget(_targetTransform.transform.forward);
        LadderStart();
    }

    private void LadderStart()
    {
        animator.SetBool(AssignAnimationIDs.AnimIDLadder, true);
        animator.SetTrigger(isUpLadder ? AssignAnimationIDs.AnimIDLadderUpStart : AssignAnimationIDs.AnimIDLadderDownStart);

        _input.interaction = false;
        isUpLadder = isDownLadder = isNearLadder = false;
    }

    #endregion

    #region 상호작용

    public void Interact()
    {
        if (isNearLadder && _input.interaction)
        {
            isClimbing = true;
            return;
        }

        if (_input.interaction && _currentDropItems.Count > 0)
        {
            DropItem closestItem = GetClosestDropItem();
            if (closestItem)
            {
                closestItem.Interact(this);
                _currentDropItems.Remove(closestItem);
            }
        }
    }

    #endregion

    #region 아이템

    private List<DropItem> _currentDropItems = new List<DropItem>();

    public bool AddItemToInventory(ItemData itemData)
    {
        if (Managers.InventoryManager.Add(itemData) >= 0)
        {
            Debug.Log($"{itemData.name} was added to inventory.");
            return true;
        }
        else
        {
            Debug.Log("Failed to add item to inventory.");
            return false;
        }
    }

    private DropItem GetClosestDropItem()
    {
        DropItem closestItem = null;
        float closestDistance = float.MaxValue;

        foreach (var item in _currentDropItems)
        {
            float distance = Vector3.Distance(transform.position, item.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        return closestItem;
    }

    #endregion

    #region 트리거,콜라이더

    public void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, LayerData.GroundLayer | LayerData.DefaultLayer, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            animator.SetBool(AssignAnimationIDs.AnimIDGrounded, Grounded);
        }
    }


    public List<CharacterController> GetMonstersInRange(Vector3 position, float radius = 0.5f)
    {
        List<CharacterController> monstersInRange = new List<CharacterController>();

        Collider[] colliders = Physics.OverlapSphere(position, radius, LayerData.MonsterLayer);

        foreach (Collider collider in colliders)
        {
            CharacterController monster = collider.GetComponent<CharacterController>();

            if (monster != null)
            {
                monstersInRange.Add(monster);
            }
        }

        return monstersInRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") && other.TryGetComponent<DropItem>(out var dropItem))
        {
            Debug.Log("아이템 진입");
            if (!_currentDropItems.Contains(dropItem))
            {
                _currentDropItems.Add(dropItem);
            }
        }
        else if (other.CompareTag("StartDown"))
        {
            NearLadderPosition(other);

            isUpLadder = false;
            isDownLadder = true;
        }
        else if (other.CompareTag("EndDown"))
        {
            EndofLadder(AssignAnimationIDs.AnimIDLadderDownEnd);
        }
        else if (other.CompareTag("StartUp"))
        {
            NearLadderPosition(other);

            isUpLadder = true;
            isDownLadder = false;
        }
        else if (other.CompareTag("EndUp"))
        {
            EndofLadder(AssignAnimationIDs.AnimIDLadderUpEnd);
        }
        else
        {
            isNearLadder = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item") && other.TryGetComponent<DropItem>(out var dropItem))
        {
            Debug.Log("아이템 탈출");
            _currentDropItems.Remove(dropItem);
        }
    }

    #endregion

    #region 애니메이션

    protected virtual void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(creatureData.walkSound, transform.position, 0.5f);
        }
    }

    protected virtual void OnHit(AnimationEvent animationEvent)
    {
        List<CharacterController> monsters = GetMonstersInRange(attackPoint.position);

        foreach (CharacterController monster in monsters)
        {
            monster.gameObject.GetComponent<CreatureController>().GetDamaged(creatureData.attack);
        }
    }

    public void OnExitLadder()
    {
        isClimbing = false;
        LookAtTarget(_targetTransform.transform.forward);
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);

        Gizmos.DrawSphere(
            new Vector3(attackPoint.position.x, attackPoint.position.y - GroundedOffset, attackPoint.position.z),
            0.5f);
    }

#endif
}