using Google.Protobuf.Protocol;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyPlayerController : PlayerController
{
    public bool isLevelUp = false;

    private UI_Joystick _joystick;
    private float _currTime = 0.0f;
    private Coroutine _coSkillCooltime;
    [SerializeField]private PlayerSkillController _playerSkillController;

    public int WeaponDamage { get; private set; }
    public int ArmorDefence { get; private set; }

    [SerializeField] private UI_ExpBar _uiExpBar;

    public int Exp
    {
        get => Stat.TotalExp;
        set
        {
            if (Stat.TotalExp != value)
            {
                Stat.TotalExp = value;
                UpdateExpBar();
            }
        }
    }

    public void UpdateExpBar()
    {
        if (!_uiExpBar)
        {
            _uiExpBar = FindObjectOfType<UI_ExpBar>();
        }

        _uiExpBar?.SetExpBar(Exp);
    }


    protected override void Init()
    {
        base.Init();
        RefreshAdditionalStat();
        _joystick = FindObjectOfType<UI_Joystick>();
    }


    protected override void UpdateController()
    {
        _playerSkillController.UseSkill();

        UpdateMovDir();

        CheckUpdatedFlag();

        base.UpdateController();
    }

    private void UpdateMovDir()
    {
        if (State == CreatureState.Skill) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0)
        {
            horizontal = _joystick.inputDirection.x;
            vertical = _joystick.inputDirection.y;
        }

        Move = new Vec2 { X = horizontal, Y = vertical };
        LookDir = Move;
    }


    protected override void CheckUpdatedFlag()
    {
        _currTime += Time.deltaTime;

        if (statusChanged || _currTime >= 2.0f)
        {
            var moveDir = new Vector2(Move.X, Move.Y).normalized;
            C_Move movePacket = new C_Move
            {
                PosInfo = new PositionInfo
                {
                    Pos = new Vec2 { X = transform.position.x, Y = transform.position.y },
                    Move = new Vec2() { X = moveDir.x, Y = moveDir.y },
                }
            };
            Managers.Network.Send(movePacket);

            statusChanged = false;
            _currTime = 0;
        }
    }
    

    public void RefreshAdditionalStat()
    {
        WeaponDamage = 0;
        ArmorDefence = 0;

        foreach (Item item in Managers.Inven.Items.Values)
        {
            if (!item.Equipped) continue;

            switch (item.ItemType)
            {
                case ItemType.Weapon:
                    _playerSkillController.SetSkillData((((Weapon)item).WeaponType == WeaponType.Bow) ? 2 : 1);
                    WeaponDamage += ((Weapon)item).Damage;
                    break;
                case ItemType.Armor:
                    ArmorDefence += ((Armor)item).Defence;
                    break;
            }
        }
    }
}