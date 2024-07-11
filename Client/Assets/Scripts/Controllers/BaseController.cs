﻿using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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

    protected bool _updated = false;

    PositionInfo _positionInfo = new PositionInfo();
    Vector3 _moveDir = new Vector3();

    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            CellPos = new Vector3Int((int)value.PosX, (int)value.PosY, 0);
            State = value.State;
        }
    }

    public void SyncPos()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = destPos;
    }

    public Vector3Int CellPos
    {
        get { return new Vector3Int((int)PosInfo.PosX, (int)PosInfo.PosY, 0); }

        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            _updated = true;
        }
    }

    protected Animator _animator;
    protected SpriteRenderer _sprite;

    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            UpdateAnimation();
            _updated = true;
        }
    }

    public Vector3 Dir
    {
        get { return _moveDir; }
        set
        {
            if (_moveDir.x == value.x && _moveDir.y == value.y && _moveDir.z == value.z)
            {
                return;
            }

            Debug.Log(value);
            _moveDir = value;

            UpdateAnimation();
            _updated = true;
        }
    }


    protected virtual void UpdateAnimation()
    {
        if (!_animator || !_sprite)
            return;

        if (State == CreatureState.Idle)
        {
            if (Dir.y > 0)
            {
                _animator.Play("IDLE_BACK");
                _sprite.flipX = false;
            }
            else if (Dir.y < 0)
            {
                _animator.Play("IDLE_FRONT");
                _sprite.flipX = false;
            }
            else if (Dir.x < 0)
            {
                _animator.Play("IDLE_RIGHT");
                _sprite.flipX = true;
            }
            else if (Dir.x > 0)
            {
                _animator.Play("IDLE_RIGHT");
                _sprite.flipX = false;
            }
        }
        else if (State == CreatureState.Moving)
        {
            if (Dir.y > 0)
            {
                _animator.Play("WALK_BACK");
                _sprite.flipX = false;
            }
            else if (Dir.y < 0)
            {
                _animator.Play("WALK_FRONT");
                _sprite.flipX = false;
            }
            else if (Dir.x < 0)
            {
                _animator.Play("WALK_RIGHT");
                _sprite.flipX = true;
            }
            else if (Dir.x > 0)
            {
                _animator.Play("WALK_RIGHT");
                _sprite.flipX = false;
            }
        }
        else if (State == CreatureState.Skill)
        {
            if (Dir.y > 0)
            {
                _animator.Play("ATTACK_BACK");
                _sprite.flipX = false;
            }
            else if (Dir.y < 0)
            {
                _animator.Play("ATTACK_FRONT");
                _sprite.flipX = false;
            }
            else if (Dir.x < 0)
            {
                _animator.Play("ATTACK_RIGHT");
                _sprite.flipX = true;
            }
            else if (Dir.x > 0)
            {
                _animator.Play("ATTACK_RIGHT");
                _sprite.flipX = false;
            }
        }
        else
        {
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
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        UpdateAnimation();
    }

    protected virtual void Init(Vector3 dir)
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

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

    // 스르륵 이동하는 것을 처리
    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < Speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * Speed * Time.deltaTime;
            State = CreatureState.Moving;
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
}