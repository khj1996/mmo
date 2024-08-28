using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using DG.Tweening;
using UnityEngine;
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
        get { return _positionInfo.State; }
        set
        {
            if (_positionInfo.State == value)
                return;

            _positionInfo.State = value;
            UpdateAnimation();
        }
    }

    public MoveDir MoveDir
    {
        get { return _positionInfo.MoveDir; }
        set
        {
            if (_positionInfo.MoveDir == value)
                return;

            _positionInfo.MoveDir = value;
            UpdateAnimation();
        }
    }


    protected virtual void UpdateAnimation()
    {
        if (!_animator || !_sprite)
            return;

        switch (State)
        {
            case CreatureState.Idle:
                switch (MoveDir)
                {
                    case MoveDir.Up:
                        _animator.Play("IDLE_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("IDLE_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Right:
                        _animator.Play("IDLE_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDir.Left:
                        _animator.Play("IDLE_RIGHT");
                        _sprite.flipX = false;
                        break;
                }

                break;
            case CreatureState.Moving:
                switch (MoveDir)
                {
                    case MoveDir.Up:
                        _animator.Play("WALK_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("WALK_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Right:
                        _animator.Play("WALK_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDir.Left:
                        _animator.Play("WALK_RIGHT");
                        _sprite.flipX = false;
                        break;
                }

                break;
            case CreatureState.Skill:
                switch (MoveDir)
                {
                    case MoveDir.Up:
                        _animator.Play("ATTACK_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("ATTACK_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Right:
                        _animator.Play("ATTACK_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDir.Left:
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
        transform.position = new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);

        UpdateAnimation();
    }

    protected virtual void Init(Vector3 dir)
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);

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
        Vector3 destPos = new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
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
        Vector3 destPos = new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;

        if (dist < Speed * Time.deltaTime)
        {
            transform.position = destPos;
        }
        else
        {
            transform.position += moveDir.normalized * (Speed * Time.deltaTime);
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
        PosInfo = new PositionInfo()
        {
            PosX = movePacket.PosInfo.PosX,
            PosY = movePacket.PosInfo.PosY,
            PosZ = movePacket.PosInfo.PosZ,
            MoveDir = movePacket.PosInfo.MoveDir,
            State = movePacket.PosInfo.State
        };
    }
}