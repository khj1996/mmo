using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class BaseController : MonoBehaviour
{
    public int Id { get; set; }

    StatInfo _stat = new StatInfo();

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

    private PositionInfo _positionInfo = new PositionInfo();

    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            _positionInfo = value;
            UpdateAnimation();
        }
    }

    protected Animator _animator;
    protected SpriteRenderer _sprite;

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
            _positionInfo.Move = value;
        }
    }

    public Vec2 LookDir
    {
        get => _positionInfo.LookDir;
        set
        {
            if (_positionInfo.Move.Equals(value))
                return;
            _positionInfo.Move = value;
        }
    }

    protected MoveDirection CheckDirection(Vec2 direction)
    {
        // 벡터의 방향을 결정할 임계값 (오차 허용 범위)
        float threshold = 0.1f;

        // 각 방향에 대해 벡터를 확인하는 스위치문
        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            // 좌우 방향 확인 (x값이 더 클 때)
            switch (direction.X)
            {
                case var x when x > threshold:
                    return MoveDirection.Right;
                case var x when x < -threshold:
                    return MoveDirection.Left;
            }
        }
        else
        {
            // 상하 방향 확인 (y값이 더 클 때)
            switch (direction.Y)
            {
                case var y when y > threshold:
                    return MoveDirection.Up;

                    break;
                case var y when y < -threshold:
                    return MoveDirection.Down;
            }
        }

        return MoveDirection.None;
    }

    protected virtual void UpdateAnimation()
    {
        if (!_animator || !_sprite)
            return;

        var dir = CheckDirection(LookDir);

        switch (State)
        {
            case CreatureState.Idle:
                switch (dir)
                {
                    case MoveDirection.Up:
                        _animator.Play("IDLE_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Down:
                        _animator.Play("IDLE_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Right:
                        _animator.Play("IDLE_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDirection.Left:
                        _animator.Play("IDLE_RIGHT");
                        _sprite.flipX = false;
                        break;
                }

                break;
            case CreatureState.Moving:
                switch (dir)
                {
                    case MoveDirection.Up:
                        _animator.Play("WALK_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Down:
                        _animator.Play("WALK_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Right:
                        _animator.Play("WALK_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDirection.Left:
                        _animator.Play("WALK_RIGHT");
                        _sprite.flipX = false;
                        break;
                }

                break;
            case CreatureState.Skill:
                switch (dir)
                {
                    case MoveDirection.Up:
                        _animator.Play("ATTACK_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Down:
                        _animator.Play("ATTACK_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Right:
                        _animator.Play("ATTACK_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDirection.Left:
                        _animator.Play("ATTACK_RIGHT");
                        _sprite.flipX = false;
                        break;
                }

                break;
            default:
                break;
        }
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
        Vector3 destPos = new Vector3(Pos.X, Pos.Y, 0);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;

        if (dist >= Speed * Time.deltaTime)
        {
            State = CreatureState.Moving;
        }
    }

    // 스르륵 이동하는 것을 처리
    protected virtual void UpdateMoving()
    {
        Vector3 destPos = new Vector3(Pos.X, Pos.Y, 0);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;

        if (dist < Speed * Time.deltaTime)
        {
            transform.position = destPos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, destPos, Speed * Time.fixedDeltaTime);
        }
    }

    protected virtual void MoveToNextPos()
    {
    }

    protected virtual void UpdateSkill()
    {
    }

    protected virtual void UpdateDead()
    {
    }


    public virtual void UpdatePosition(S_Move movePacket)
    {
        PosInfo.MergeFrom(movePacket.PosInfo);
    }
}