using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class PlayerController : CreatureController
{
    #region 변수

    [Header("Movement Settings")] [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;


    [Header("References")] [SerializeField]
    public NavMeshAgent navMeshAgent;

    [SerializeField] private VisualEffect healEffect;
    [SerializeField] public PlayerLockOn playerLockOn;
    [SerializeField] private CameraController playerCameraController;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private WeaponData[] weaponDataOptions;
    [SerializeField] private Transform equipWeapon;


    [HideInInspector] public bool isClimbing;
    [HideInInspector] public bool isNearLadder;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isAutoMove;
    [HideInInspector] public bool inLadderMotion;
    [HideInInspector] public bool isUpLadder = false;
    [HideInInspector] public Vector3 autoMoveDestination;

    private WeaponData currentEquipweaponData;
    private Coroutine attackCoroutine;
    private CreatureStateMachine<PlayerController> stateMachine;
    private PlayerData playerData => (PlayerData)creatureData;

    private PlayerInteract playerInteract;
    private PlayerMovement playerMovement;

    #endregion

    #region 초기화

    private void Start()
    {
        Init();
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
        playerMovement = new PlayerMovement(this, navMeshAgent, inputSystem, animator, playerData);
        playerInteract = new PlayerInteract(this, inputSystem);
        DisableNavMesh();
        stat.ChangeHpEvent += OnHeal;
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

        stateMachine.AddTransition<PlayerData.IdleAndMoveState, PlayerData.CrouchState>(() => !playerLockOn.isFindTarget && inputSystem.crouch);
        stateMachine.AddTransition<PlayerData.IdleAndMoveState, PlayerData.JumpState>(() =>
            !playerLockOn.isFindTarget && _AttackCoroutine == null && isGrounded && !inputSystem.crouch && inputSystem.jump);
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

    #region 업데이트

    private void Update()
    {
        stateMachine.Update();
    }

    public void IdleAndMoveStateUpdate()
    {
        playerInteract.Interact();
        playerMovement.Move();
        playerMovement.JumpAndGravity();
        ChangeAttackType();
        GroundedCheck();
        CheckAttack();
    }

    public void CrouchStateUpdate()
    {
        playerInteract.Interact();
        playerMovement.Move();
        CheckAttack();
        GroundedCheck();
    }


    public void JumpStateUpdate()
    {
        playerInteract.Interact();
        playerMovement.Move();
        playerMovement.JumpAndGravity();
        GroundedCheck();
    }

    public void LadderStateUpdate()
    {
        playerMovement.Move();
    }

    #endregion

    public void MoveAuto(Vector3 destination)
    {
        playerMovement.MoveAuto(destination);
    }


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
            _AttackCoroutine = StartCoroutine(rw.AttackCoroutine(this, playerLockOn.currentTarget));
        }
    }

    public void EndAttack()
    {
        _AttackCoroutine = null;
    }

    #endregion

    #region 사다리

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

        animator?.SetBool(AssignAnimationIDs.AnimIDGrounded, isGrounded);
    }


    private void OnTriggerEnter(Collider other)
    {
        playerInteract.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        playerInteract.OnTriggerExit(other);
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
            mw.OnHit(this, Util.GetModifiedPoint(transform, mw.attackPoint));
        }
    }

    public void OnExitLadder()
    {
        LookAtTarget(targetTransform.transform.forward);
        isClimbing = false;
        targetTransform = null;

        if (isAutoMove)
        {
            EnableNavMesh(transform.position);
            navMeshAgent.SetDestination(autoMoveDestination);
        }
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

    public void DisableNavMesh()
    {
        animator.applyRootMotion = true;
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
    }

    public void EnableNavMesh(Vector3 newPos)
    {
        animator.applyRootMotion = false;

        navMeshAgent.Warp(newPos);
        navMeshAgent.nextPosition = newPos;

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

    public void ChangeViewDistance(float distance)
    {
        playerCameraController.ChangeViewDistance(distance);
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