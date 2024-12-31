using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class PlayerController : CreatureController
{
    [Header("Movement Settings")] [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;

    [Header("References")] [SerializeField]
    private NavMeshAgent navMeshAgent;

    [SerializeField] private VisualEffect healEffect;
    [SerializeField] private LockOn lockOn;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private WeaponData[] weaponDataOptions;
    [SerializeField] private Transform equipWeapon;

    private float speed;
    private float animationBlend;
    private float targetRotation;
    private float rotationVelocity;
    private float verticalVelocity;
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;
    private readonly float _terminalVelocity = 53.0f;

    private bool isGrounded;

    private bool isAutoMove;
    private bool isClimbing;
    private bool isNearLadder;
    private bool inLadderMotion;

    private Camera mainCamera;
    private WeaponData currentEquipweaponData;
    private Coroutine attackCoroutine;
    private CreatureStateMachine<PlayerController> stateMachine;
    private List<DropItem> currentDropItems;
    private PlayerData playerData => (PlayerData)creatureData;


    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }


    #region 초기화

    private void Start()
    {
        Init();
        DisableNavMesh();
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
        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
        currentDropItems = new List<DropItem>();
        stat.ChangeHpEvent += OnHeal;
        DisableNavMesh();
    }

    private void InitializeAnimator()
    {
        currentEquipweaponData = weaponDataOptions[0];
        currentEquipweaponData.Initialize(animator);
    }

    private void PrewarmPools()
    {
        Managers.PoolManager.PrewarmPools<Bullet>("Bullet", null, 20);
    }


    private void InitFSM()
    {
        stateMachine = new CreatureStateMachine<PlayerController>();

        stateMachine.AddState(new PlayerData.IdleAndMoveState(this));
        stateMachine.AddState(new PlayerData.CrouchState(this));
        stateMachine.AddState(new PlayerData.JumpState(this));
        stateMachine.AddState(new PlayerData.GetHitState(this));
        stateMachine.AddState(new PlayerData.LadderState(this));

        #region 상태 전이 조건

        #region IdleState

        stateMachine.AddTransition<PlayerData.IdleAndMoveState, PlayerData.CrouchState>(() => !lockOn.isFindTarget && inputSystem.crouch);
        stateMachine.AddTransition<PlayerData.IdleAndMoveState, PlayerData.JumpState>(() => !lockOn.isFindTarget && _AttackCoroutine == null && isGrounded && !inputSystem.crouch && inputSystem.jump);
        stateMachine.AddTransition<PlayerData.IdleAndMoveState, PlayerData.LadderState>(() => isClimbing);

        #endregion


        #region CrouchState

        stateMachine.AddTransition<PlayerData.CrouchState, PlayerData.IdleAndMoveState>(() => !inputSystem.crouch || !isGrounded);

        #endregion

        #region JumpState

        stateMachine.AddTransition<PlayerData.JumpState, PlayerData.IdleAndMoveState>(() => isGrounded);

        #endregion

        #region LadderState

        stateMachine.AddTransition<PlayerData.LadderState, PlayerData.IdleAndMoveState>(() => !isClimbing);

        #endregion

        #endregion

        stateMachine.ChangeState(typeof(PlayerData.IdleAndMoveState));
    }

    #endregion


    private void Update()
    {
        stateMachine.Update();
    }


    #region 이동

    public void MoveAuto(Vector3 destination)
    {
        if (!isAutoMove)
        {
            SwitchAutoMove();
        }

        destination = new Vector3(-0.0166049004f, 1.11998761f, -6.41998768f);
        // NavMesh 위의 유효한 위치인지 확인
        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
    }

    public void Move()
    {
        if (isAutoMove)
        {
            return;
        }

        if (isClimbing)
        {
            MoveLadder();
            return;
        }

        float targetSpeed = inputSystem.sprint ? creatureData.sprintSpeed : creatureData.speed;
        targetSpeed = (inputSystem.crouch && isGrounded || lockOn.isFindTarget) ? creatureData.crouchSpeed : targetSpeed;
        targetSpeed = inputSystem.move == Vector2.zero ? 0.0f : targetSpeed;

        UpdateMovement(targetSpeed);
        ApplyRotation();
        ApplyTranslation(targetSpeed);
    }

    private void UpdateMovement(float targetSpeed)
    {
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float inputMagnitude = inputSystem.analogMovement ? inputSystem.move.magnitude : 1f;

        speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * creatureData.acceleration);
        speed = Mathf.Round(speed * 1000f) / 1000f;

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * creatureData.acceleration);
        if (animationBlend < 0.01f) animationBlend = 0f;
    }

    private void ApplyRotation()
    {
        Vector3 inputDirection = new Vector3(inputSystem.move.x, 0.0f, inputSystem.move.y).normalized;
        if (inputSystem.move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

            if (!lockOn.isFindTarget)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
    }

    private void ApplyTranslation(float targetSpeed)
    {
        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, rotationVelocity, 0.0f) * Time.deltaTime);
        EventManager.TriggerPlayerMoved(transform.position);

        if (hasAnimator)
        {
            animator.SetFloat(AssignAnimationIDs.AnimIDSpeed, animationBlend);
            animator.SetFloat(AssignAnimationIDs.AnimIDMotionSpeed, targetSpeed);
        }

        if (!isAutoMove)
        {
            navMeshAgent.nextPosition = transform.position;
        }
    }

    private void MoveLadder()
    {
        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        bool isMove = inputSystem.move != Vector2.zero;
        targetDirection.x = targetDirection.z = 0;

        isGrounded = true;
        LookAtTarget(targetTransform.transform.forward);

        var move = inputSystem.move.y;
        animator.SetBool(AssignAnimationIDs.AnimIDLadderUpPlay, isMove && move >= 0);
        animator.SetBool(AssignAnimationIDs.AnimIDLadderDownPlay, isMove && move < 0);
        EventManager.TriggerPlayerMoved(transform.position);
    }

    public void SwitchAutoMove()
    {
        if (isAutoMove)
        {
            DisableNavMesh();
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
        if (isGrounded)
        {
            ResetFallTimeout();

            if (hasAnimator)
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
        fallTimeoutDelta = FallTimeout;
    }

    private void ResetAnimatorJumpAndFall()
    {
        animator.SetBool(AssignAnimationIDs.AnimIDJump, false);
        animator.SetBool(AssignAnimationIDs.AnimIDFreeFall, false);
    }

    private void HandleGroundedVelocity()
    {
        if (rotationVelocity < 0.0f)
        {
            rotationVelocity = -2f;
        }
    }

    private void PerformJump()
    {
        rotationVelocity = Mathf.Sqrt(-2.5f * creatureData.weight);

        if (hasAnimator)
        {
            animator.SetBool(AssignAnimationIDs.AnimIDJump, true);
        }

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
        fallTimeoutDelta = JumpTimeout;

        if (fallTimeoutDelta >= 0.0f)
        {
            fallTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            if (hasAnimator)
            {
                animator.SetBool(AssignAnimationIDs.AnimIDFreeFall, true);
            }
        }
    }

    private void ApplyGravity()
    {
        if (rotationVelocity < _terminalVelocity)
        {
            rotationVelocity += creatureData.weight * Time.deltaTime;
        }
    }

    #endregion


    #region 공격

    private Coroutine _AttackCoroutine = null;

    public void CheckAttack()
    {
        if (_AttackCoroutine != null || !inputSystem.attack)
            return;

        inputSystem.attack = false;
        if (currentEquipweaponData is MeleeWeaponData mw)
        {
            _AttackCoroutine = StartCoroutine(mw.AttackCoroutine(this));
        }
        else if (currentEquipweaponData is RangedWeaponData rw)
        {
            _AttackCoroutine = StartCoroutine(rw.AttackCoroutine(this, lockOn.currentTarget));
        }
    }

    public void EndAttack()
    {
        _AttackCoroutine = null;
    }

    #endregion

    #region 사다리

    private void EnterLadderPosition(Collider other)
    {
        if (isClimbing) return;
        isNearLadder = true;
        targetTransform = other.gameObject.transform;
    }

    private void ExitLadderPosition()
    {
        if (isClimbing) return;
        isNearLadder = false;
        targetTransform = null;
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
        LookAtTarget(targetTransform.transform.forward);
        LadderStart();
    }

    private void LadderStart()
    {
        animator.SetBool(AssignAnimationIDs.AnimIDLadder, true);

        animator.SetTrigger(targetTransform.position.y > transform.position.y
            ? AssignAnimationIDs.AnimIDLadderUpStart
            : AssignAnimationIDs.AnimIDLadderDownStart);

        inputSystem.interaction = false;
        isNearLadder = false;
        inLadderMotion = true;
    }

    #endregion

    #region 상호작용

    public void Interact()
    {
        if (isNearLadder && inputSystem.interaction)
        {
            isClimbing = true;
            return;
        }

        if (inputSystem.interaction && currentDropItems.Count > 0)
        {
            DropItem closestItem = GetClosestObject(currentDropItems, item => item.transform.position);
            if (closestItem)
            {
                closestItem.Interact(this);
                currentDropItems.Remove(closestItem);
            }

            return;
        }

        if (inputSystem.interaction)
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
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, LayerData.GroundLayer, QueryTriggerInteraction.Ignore);

        if (hasAnimator)
        {
            animator.SetBool(AssignAnimationIDs.AnimIDGrounded, isGrounded);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") && other.TryGetComponent<DropItem>(out var dropItem))
        {
            if (!currentDropItems.Contains(dropItem))
            {
                currentDropItems.Add(dropItem);
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
            currentDropItems.Remove(dropItem);
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
        if (currentEquipweaponData is MeleeWeaponData mw)
        {
            mw.OnHit(this, attackPoint.position);
        }
    }

    public void OnExitLadder()
    {
        LookAtTarget(targetTransform.transform.forward);
        isClimbing = false;
        targetTransform = null;
    }

    public void OnChangeWeapon()
    {
        if (currentEquipweaponData.attackType == 0)
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

    private void DisableNavMesh()
    {
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
    }

    private void EnableNavMesh(Vector3 newPos)
    {
        navMeshAgent.Warp(newPos);
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
    }

    public void ChangeAttackType()
    {
        if (inputSystem.changeAttackType && currentEquipweaponData.attackType != inputSystem.lastInputAttackType)
        {
            inputSystem.changeAttackType = false;
            currentEquipweaponData = weaponDataOptions[inputSystem.lastInputAttackType];
            currentEquipweaponData.Initialize(animator);
            animator.SetTrigger(AssignAnimationIDs.AnimIDChangeAttackType);
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

        Gizmos.color = isGrounded ? transparentGreen : transparentRed;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);

        //Gizmos.DrawSphere(new Vector3(attackPoint.position.x, attackPoint.position.y - GroundedOffset, attackPoint.position.z), 0.5f);
    }
#endif
}