using System.Collections;
using UnityEngine;

public class PlayerControllerFSM : CreatureController
{
    [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;

    [Space(10)] public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;

    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;
    public bool Grounded = true;

    private PlayerStateMachine playerStateMachine;
    //private BehaviorTree _bt;

    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private Util.CreatureState state;


    private CharacterController _controller;
    private InputSystem _input;
    private GameObject _mainCamera;

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

        //_bt = new BehaviorTree();

        //_bt.AddNode(new ConditionNode(() => playerStateMachine.CurrentState is not JumpState));
    }

    protected override void Init()
    {
        base.Init();
        InitFSM();
        InitComponent();

        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        state = Util.CreatureState.Idle;
    }

    private void InitFSM()
    {
        playerStateMachine = new PlayerStateMachine();

        // FSM 상태 등록
        playerStateMachine.AddState(new IdleState(this));
        playerStateMachine.AddState(new MoveState(this));
        playerStateMachine.AddState(new CrouchState(this));
        playerStateMachine.AddState(new JumpState(this));
        playerStateMachine.AddState(new GetHitState(this));

        #region 상태 전이 조건

        #region IdleState

        playerStateMachine.AddTransition<IdleState, MoveState>(() => _speed != 0);
        playerStateMachine.AddTransition<IdleState, CrouchState>(() => _input.crouch);
        playerStateMachine.AddTransition<IdleState, JumpState>(() => _AttackCoroutine == null && Grounded && !_input.crouch && _input.jump);

        #endregion

        #region MoveState

        playerStateMachine.AddTransition<MoveState, IdleState>(() => _speed == 0);
        playerStateMachine.AddTransition<MoveState, CrouchState>(() => _input.crouch);
        playerStateMachine.AddTransition<MoveState, JumpState>(() => _AttackCoroutine == null && Grounded && !_input.crouch && _input.jump);

        #endregion

        #region CrouchState

        playerStateMachine.AddTransition<CrouchState, IdleState>(() => _speed == 0 && !_input.crouch);
        playerStateMachine.AddTransition<CrouchState, MoveState>(() => _speed != 0 && !_input.crouch);

        #endregion

        #region JumpState

        playerStateMachine.AddTransition<JumpState, IdleState>(() => _speed == 0 && Grounded);
        playerStateMachine.AddTransition<JumpState, MoveState>(() => _speed != 0 && Grounded);

        #endregion

        #endregion

        playerStateMachine.ChangeState(typeof(IdleState));
    }

    private void InitComponent()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<InputSystem>();
    }

    #endregion


    private void Update()
    {
        //_bt.Evaluate(); // BT에서 조건을 평가
        playerStateMachine.Update();
    }

    public void UpdateState(Util.CreatureState state)
    {
    }


    public void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;

            if (_hasAnimator)
            {
                _animator.SetBool(AssignAnimationIDs.AnimIDJump, false);
                _animator.SetBool(AssignAnimationIDs.AnimIDFreeFall, false);
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
                    _animator.SetBool(AssignAnimationIDs.AnimIDJump, true);
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
                    _animator.SetBool(AssignAnimationIDs.AnimIDFreeFall, true);
                }
            }
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += creatureData.weight * Time.deltaTime;
        }
    }


    public void Move()
    {
        float targetSpeed = _input.sprint ? creatureData.sprintSpeed : creatureData.speed;


        if (_input.crouch && Grounded)
        {
            _animator.SetBool(AssignAnimationIDs.AnimIDCrouch, true);
            targetSpeed = creatureData.crouchSpeed;
        }
        else if (!_input.crouch)
        {
            _animator.SetBool(AssignAnimationIDs.AnimIDCrouch, false);
        }


        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * creatureData.acceleration);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * creatureData.acceleration);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        if (_hasAnimator)
        {
            _animator.SetFloat(AssignAnimationIDs.AnimIDSpeed, _animationBlend);
            _animator.SetFloat(AssignAnimationIDs.AnimIDMotionSpeed, inputMagnitude);
        }
    }


    private Coroutine _AttackCoroutine = null;

    public void CheckAttack()
    {
        Debug.Log(_animator.GetCurrentAnimatorStateInfo(1).normalizedTime);
        if (_AttackCoroutine != null || !_input.attack) return;

        _input.attack = false;
        _AttackCoroutine = StartCoroutine(AttackCoroutine());
    }

    public void Hit()
    {
    }

    private IEnumerator AttackCoroutine()
    {
        _animator.SetTrigger(AssignAnimationIDs.AnimIDAttackTrigger);
        _animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, 1);

        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1f);

        _animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, 0);

        _AttackCoroutine = null;
    }

    public void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            _animator.SetBool(AssignAnimationIDs.AnimIDGrounded, Grounded);
        }
    }
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
    }

#endif
}