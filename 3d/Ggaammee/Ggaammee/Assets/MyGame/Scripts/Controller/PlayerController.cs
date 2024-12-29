using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class PlayerController : CreatureController
{
    [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;

    [Space(10)] public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;

    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public bool Grounded = true;

    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private VisualEffect healEffect;


    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private readonly float _terminalVelocity = 53.0f;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private CreatureStateMachine<PlayerController> creatureStateMachine;
    private Camera _mainCamera;
    private List<DropItem> _currentDropItems;
    private PlayerData PlayerData => (PlayerData)creatureData;


    [SerializeField] private LockOn _lockOn;
    [SerializeField] private InputSystem _input;
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private WeaponData[] tempWeaponDatas;
    [SerializeField] private Transform equipWeapon;

    private bool isAutoMove = false;


    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }


    #region 초기화

    private void Start()
    {
        Init();
        DisableNav();
    }

    protected override void Init()
    {
        base.Init();

        InitFSM();
        InitializeManagers();
        InitializePlayer();
        InitializeAnimator();
        PrewarmPools();
    }

    private void InitializeManagers()
    {
        Managers.ObjectManager.MainPlayer = this;
        Managers.ObjectManager.RegisterPlayer(this);
    }

    private void InitializePlayer()
    {
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        _currentDropItems = new List<DropItem>();
        stat.ChangeHpEvent += OnHeal;
        DisableNav();
    }

    private void InitializeAnimator()
    {
        weaponData = tempWeaponDatas[0];
        animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, weaponData.attackType);
        animator.SetFloat(AssignAnimationIDs.AnimIDAttackTypeTemp, weaponData.attackType);
    }

    private void PrewarmPools()
    {
        Managers.PoolManager.PrewarmPools<Bullet>("Bullet", null, 20);
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

    #endregion


    private void Update()
    {
        creatureStateMachine.Update();
    }


    #region 이동

    public void Move()
    {
        if (isClimbing)
        {
            MoveLadder();
            return;
        }

        float targetSpeed = _input.sprint ? creatureData.sprintSpeed : creatureData.speed;
        targetSpeed = (_input.crouch && Grounded || _lockOn.isFindTarget) ? creatureData.crouchSpeed : targetSpeed;
        targetSpeed = _input.move == Vector2.zero ? 0.0f : targetSpeed;

        UpdateMovement(targetSpeed);
        ApplyRotation();
        ApplyTranslation(targetSpeed);

        if (!isAutoMove)
        {
            UpdateNaveMashAgent();
        }

        EventManager.TriggerPlayerMoved(transform.position);
    }

    private void UpdateNaveMashAgent()
    {
        _navMeshAgent.nextPosition = transform.position;
    }

    private void UpdateMovement(float targetSpeed)
    {
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * creatureData.acceleration);
        _speed = Mathf.Round(_speed * 1000f) / 1000f;

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * creatureData.acceleration);
        if (_animationBlend < 0.01f) _animationBlend = 0f;
    }

    private void ApplyRotation()
    {
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
    }

    private void ApplyTranslation(float targetSpeed)
    {
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        if (_hasAnimator)
        {
            animator.SetFloat(AssignAnimationIDs.AnimIDSpeed, _animationBlend);
            animator.SetFloat(AssignAnimationIDs.AnimIDMotionSpeed, targetSpeed);
        }
    }

    private void MoveLadder()
    {
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        bool isMove = _input.move != Vector2.zero;
        targetDirection.x = targetDirection.z = 0;

        Grounded = true;
        LookAtTarget(_targetTransform.transform.forward);

        var move = _input.move.y;
        animator.SetBool(AssignAnimationIDs.AnimIDLadderUpPlay, isMove && move >= 0);
        animator.SetBool(AssignAnimationIDs.AnimIDLadderDownPlay, isMove && move < 0);
        EventManager.TriggerPlayerMoved(transform.position);
    }

    public void SwitchAutoMove()
    {
        if (isAutoMove)
        {
            DisableNav();
        }
        else if (!isAutoMove)
        {
            EnableNavMesh(transform.position);
        }

        isAutoMove = !isAutoMove;
    }

    #endregion


    #region 점프

    public void JumpAndGravity()
    {
        if (Grounded)
        {
            ResetFallTimeout();

            if (_hasAnimator)
            {
                ResetAnimatorJumpAndFall();
            }

            HandleGroundedVelocity();

            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
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
        _fallTimeoutDelta = FallTimeout;
    }

    private void ResetAnimatorJumpAndFall()
    {
        animator.SetBool(AssignAnimationIDs.AnimIDJump, false);
        animator.SetBool(AssignAnimationIDs.AnimIDFreeFall, false);
    }

    private void HandleGroundedVelocity()
    {
        if (_verticalVelocity < 0.0f)
        {
            _verticalVelocity = -2f;
        }
    }

    private void PerformJump()
    {
        _verticalVelocity = Mathf.Sqrt(-2.5f * creatureData.weight);

        if (_hasAnimator)
        {
            animator.SetBool(AssignAnimationIDs.AnimIDJump, true);
        }

        _input.jump = false;
    }

    private void UpdateJumpTimeout()
    {
        if (_jumpTimeoutDelta >= 0.0f)
        {
            _jumpTimeoutDelta -= Time.deltaTime;
        }
    }

    private void HandleAirborneState()
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

    private void ApplyGravity()
    {
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
        if (_AttackCoroutine != null || !_input.attack)
            return;

        _input.attack = false;
        if (weaponData is MeleeWeaponData mw)
        {
            _AttackCoroutine = StartCoroutine(mw.AttackCoroutine(this));
        }
        else if (weaponData is RangedWeaponData rw)
        {
            _AttackCoroutine = StartCoroutine(rw.AttackCoroutine(this, _lockOn.currentTarget));
        }
    }

    public void EndAttack()
    {
        _AttackCoroutine = null;
    }

    #endregion

    #region 사다리

    private bool isClimbing = false;
    private bool isNearLadder = false;
    private bool inLadderMotion = false;


    private void EnterLadderPosition(Collider other)
    {
        if (isClimbing) return;
        isNearLadder = true;
        _targetTransform = other.gameObject.transform;
    }

    private void ExitLadderPosition()
    {
        if (isClimbing) return;
        isNearLadder = false;
        _targetTransform = null;
    }

    private void EndofLadder(int animName)
    {
        if (isClimbing && !inLadderMotion)
        {
            animator.SetTrigger(animName);
            animator.SetBool(AssignAnimationIDs.AnimIDLadder, false);
        }

        inLadderMotion = false;
    }

    public void CharacterToLadder()
    {
        LockAtTargetPosition();
        LookAtTarget(_targetTransform.transform.forward);
        LadderStart();
    }

    private void LadderStart()
    {
        animator.SetBool(AssignAnimationIDs.AnimIDLadder, true);

        animator.SetTrigger(_targetTransform.position.y > transform.position.y
            ? AssignAnimationIDs.AnimIDLadderUpStart
            : AssignAnimationIDs.AnimIDLadderDownStart);

        _input.interaction = false;
        isNearLadder = false;
        inLadderMotion = true;
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
            DropItem closestItem = GetClosestObject(_currentDropItems, item => item.transform.position);
            if (closestItem)
            {
                closestItem.Interact(this);
                _currentDropItems.Remove(closestItem);
            }

            return;
        }

        if (_input.interaction)
        {
            Npc closestNpc = GetClosestNpc();
            if (closestNpc)
            {
                closestNpc.Interact();
            }
        }
    }

    private readonly Collider[] npcList = new Collider[3];

    private Npc GetClosestNpc()
    {
        var result = Physics.OverlapSphereNonAlloc(transform.position, 1.0f, npcList, LayerData.NpcLayer);
        if (result == 0)
        {
            return null;
        }

        var npcs = npcList.Select(x => x ? x.GetComponent<Npc>() : null).Where(npc => npc != null).ToList();

        return GetClosestObject(npcs, npc => npc.transform.position);
    }

    #endregion

    #region 아이템

    public bool AddItemToInventory(ItemData itemData)
    {
        return Managers.InventoryManager.Add(itemData) >= 0;
    }

    #endregion

    #region 트리거,콜라이더

    public void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, LayerData.GroundLayer, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            animator.SetBool(AssignAnimationIDs.AnimIDGrounded, Grounded);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") && other.TryGetComponent<DropItem>(out var dropItem))
        {
            if (!_currentDropItems.Contains(dropItem))
            {
                _currentDropItems.Add(dropItem);
            }
        }
        else if (other.CompareTag(TagData.LadderBottomTag))
        {
            if (isClimbing)
            {
                EndofLadder(AssignAnimationIDs.AnimIDLadderDownEnd);
            }
            else
            {
                EnterLadderPosition(other);
            }
        }
        else if (other.CompareTag(TagData.LadderTopTag))
        {
            if (isClimbing)
            {
                EndofLadder(AssignAnimationIDs.AnimIDLadderUpEnd);
            }
            else
            {
                EnterLadderPosition(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item") && other.TryGetComponent<DropItem>(out var dropItem))
        {
            _currentDropItems.Remove(dropItem);
        }
        else if (other.CompareTag(TagData.LadderBottomTag))
        {
            if (isClimbing)
            {
                EndofLadder(AssignAnimationIDs.AnimIDLadderDownEnd);
            }
            else
            {
                ExitLadderPosition();
            }
        }
        else if (other.CompareTag(TagData.LadderTopTag))
        {
            if (isClimbing)
            {
                EndofLadder(AssignAnimationIDs.AnimIDLadderUpEnd);
            }
            else
            {
                ExitLadderPosition();
            }
        }
    }

    #endregion

    #region 애니메이션

    public void OnHeal(float amount)
    {
        if (amount > 0)
        {
            healEffect.Play();
        }
    }

    protected virtual void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            Managers.SoundManager.PlaySound(creatureData.walkSound, transform.position, 0.5f);
        }
    }


    protected virtual void OnHit(AnimationEvent animationEvent)
    {
        if (weaponData is MeleeWeaponData mw)
        {
            mw.OnHit(this, attackPoint.position);
        }
    }

    public void OnExitLadder()
    {
        LookAtTarget(_targetTransform.transform.forward);
        isClimbing = false;
        _targetTransform = null;
    }

    public void OnChangeWeapon()
    {
        if (weaponData.attackType == 0)
        {
            equipWeapon.gameObject.SetActive(true);
        }
        else
        {
            equipWeapon.gameObject.SetActive(false);
        }
    }

    #endregion

    #region 유틸

    private void DisableNav()
    {
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
    }

    private void EnableNavMesh(Vector3 newPos)
    {
        _navMeshAgent.Warp(newPos);
        _navMeshAgent.updatePosition = true;
        _navMeshAgent.updateRotation = true;
    }

    public void ChangeAttackType()
    {
        if (_input.changeAttackType && weaponData.attackType != _input.lastInputAttackType)
        {
            _input.changeAttackType = false;
            animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, _input.lastInputAttackType);
            animator.SetFloat(AssignAnimationIDs.AnimIDAttackTypeTemp, _input.lastInputAttackType);
            animator.SetTrigger(AssignAnimationIDs.AnimIDChangeAttackType);
            weaponData = tempWeaponDatas[_input.lastInputAttackType];
        }
    }

    private T GetClosestObject<T>(List<T> objects, Func<T, Vector3> positionSelector) where T : class
    {
        T closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (var obj in objects)
        {
            float distance = Vector3.Distance(transform.position, positionSelector(obj));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        return closestObject;
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        Gizmos.color = Grounded ? transparentGreen : transparentRed;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);

        //Gizmos.DrawSphere(new Vector3(attackPoint.position.x, attackPoint.position.y - GroundedOffset, attackPoint.position.z), 0.5f);
    }
#endif
}