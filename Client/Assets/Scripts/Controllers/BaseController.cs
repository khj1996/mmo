using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseController : MonoBehaviour
{
    protected Animator _animator;
    protected SpriteRenderer _sprite;

    protected bool statusChanged = false;
    public int Id { get; set; }

    private StatInfo _stat = new StatInfo();
    private PositionInfo _positionInfo = new PositionInfo();
    private Vector3 _destination = Vector3.zero;

    public virtual StatInfo Stat
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.MergeFrom(value);
        }
    }

    public float Speed
    {
        get { return Stat.Speed; }
        set { Stat.Speed = value; }
    }

    public virtual int Hp
    {
        get { return Stat.Hp; }
        set { Stat.Hp = value; }
    }

    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            _positionInfo = value;
        }
    }


    public CreatureState State
    {
        get => _positionInfo.State;
        set
        {
            if (_positionInfo.State == value)
                return;

            _positionInfo.State = value;
            UpdateAnimation();
        }
    }

    public Vec2 Pos
    {
        get => _positionInfo.Pos;
        set
        {
            if (_positionInfo.Pos.Equals(value))
                return;
            _positionInfo.Pos = value;
        }
    }

    public Vec2 Move
    {
        get => _positionInfo.Move;
        set
        {
            if (_positionInfo.Move.Equals(value))
                return;
            statusChanged = true;
            _positionInfo.Move = value;
        }
    }

    public Vec2 LookDir
    {
        get => _positionInfo.LookDir;
        set
        {
            if (_positionInfo.LookDir.Equals(value) ||
                (value.X == 0 && value.Y == 0))
                return;
            statusChanged = true;
            _positionInfo.LookDir = value;
        }
    }


    protected MoveDirection CheckDirection(Vec2 direction)
    {
        if (direction == null || (direction.X == 0 && direction.Y == 0))
            return MoveDirection.Down;

        return Mathf.Abs(direction.X) > Mathf.Epsilon
            ? (direction.X > 0 ? MoveDirection.Right : MoveDirection.Left)
            : (direction.Y > 0 ? MoveDirection.Up : MoveDirection.Down);
    }


    protected virtual void UpdateAnimation()
    {
        if (!_animator || !_sprite)
            return;

        var dir = CheckDirection(LookDir);
        string animationName = "";
        bool flipX = false;

        switch (State)
        {
            case CreatureState.Idle:
                animationName = dir == MoveDirection.Up ? "IDLE_BACK" :
                    dir == MoveDirection.Down ? "IDLE_FRONT" : "IDLE_RIGHT";
                flipX = dir == MoveDirection.Left;
                break;

            case CreatureState.Moving:
                animationName = dir == MoveDirection.Up ? "WALK_BACK" :
                    dir == MoveDirection.Down ? "WALK_FRONT" : "WALK_RIGHT";
                flipX = dir == MoveDirection.Left;
                break;

            case CreatureState.Skill:
                animationName = dir == MoveDirection.Up ? "ATTACK_BACK" :
                    dir == MoveDirection.Down ? "ATTACK_FRONT" : "ATTACK_RIGHT";
                flipX = dir == MoveDirection.Left;
                break;
        }

        _animator.Play(animationName);
        _sprite.flipX = flipX;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();

        transform.position = new Vector3(Pos.X, Pos.Y, 0);
        _positionInfo.LookDir = new Vec2 { X = 0, Y = 0 };

        UpdateAnimation();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
    }


    protected virtual void UpdateMoving()
    {
        float step = Speed * Time.deltaTime;
        var distance = (_destination - transform.position).magnitude;

        if (distance < Mathf.Epsilon && Move.X == 0 && Move.Y == 0)
        {
            State = CreatureState.Idle;
            UpdateAnimation();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination, Mathf.Min(step, distance));
        }
    }

    protected virtual void UpdateSkill()
    {
    }

    protected virtual void UpdateDead()
    {
    }

    private Vector3 lastDestination;

    public virtual void UpdatePosition(S_Move movePacket)
    {
        Pos = movePacket.PosInfo.Pos;
        Move = movePacket.PosInfo.Move;

        if (!(Move.X == 0 && Move.Y == 0))
            LookDir = movePacket.PosInfo.Move;

        State = movePacket.PosInfo.State != CreatureState.Idle ? movePacket.PosInfo.State : CreatureState.Idle;
        UpdateAnimation();
    }
}