using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class MyPlayerController : PlayerController
{
    private bool statusChanged = false;

    private bool _moveKeyPressed = false;

    private float currTime = 0.0f;

    private Rigidbody2D _rigid;

    Coroutine _coSkillCooltime;

    public int WeaponDamage { get; private set; }
    public int ArmorDefence { get; private set; }

    protected override void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        base.Init();
        RefreshAdditionalStat();
    }

    protected override void UpdateController()
    {
        GetUIKeyInput();

        UseSkill();

        UpdateMovDir();

        CheckUpdatedFlag();

        base.UpdateController();
    }

    public void UseSkill()
    {
        if (_coSkillCooltime == null && Input.GetMouseButtonDown(0))
        {
            var mPos = Camera.main.ScreenPointToRay(Input.mousePosition);
            var clickPos = new Vector3(mPos.origin.x, mPos.origin.y, 0);


            var moveDir = (clickPos - transform.position).normalized;


            C_Skill skill = new C_Skill()
            {
                Info = new SkillInfo
                {
                    SkillId = 2
                },
                MoveDir = new Vec2()
                {
                    X = moveDir.x,
                    Y = moveDir.y,
                }
            };
            Managers.Network.Send(skill);

            _coSkillCooltime = StartCoroutine(nameof(CoInputCooltime), 0.2f);
        }
        else if (_coSkillCooltime == null && Input.GetKeyDown(KeyCode.Space))
        {
            C_Skill skill = new C_Skill()
            {
                Info = new SkillInfo
                {
                    SkillId = 1
                }
            };
            Managers.Network.Send(skill);

            _coSkillCooltime = StartCoroutine(nameof(CoInputCooltime), 0.5f);
        }
    }


    IEnumerator CoInputCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCooltime = null;
    }

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    void GetUIKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UI_GameScene gameSceneUI = Managers.UI.CurrentSceneUI as UI_GameScene;
            UI_Inventory invenUI = gameSceneUI.InvenUI;

            if (invenUI.gameObject.activeSelf)
            {
                invenUI.gameObject.SetActive(false);
            }
            else
            {
                invenUI.gameObject.SetActive(true);
                invenUI.RefreshUI(Define.InvenRefreshType.All);
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            UI_GameScene gameSceneUI = Managers.UI.CurrentSceneUI as UI_GameScene;
            UI_Stat statUI = gameSceneUI.StatUI;

            if (statUI.gameObject.activeSelf)
            {
                statUI.gameObject.SetActive(false);
            }
            else
            {
                statUI.gameObject.SetActive(true);
                statUI.RefreshUI();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var gameSceneUI = Managers.UI.CurrentSceneUI as UI_GameScene;
            var shopUI = gameSceneUI.ShopUI;

            if (shopUI.gameObject.activeSelf)
            {
                shopUI.gameObject.SetActive(false);
            }
            else
            {
                shopUI.gameObject.SetActive(true);
                shopUI.RefreshUI();
            }
        }
    }

    public void UpdateMovDir()
    {
        if (State == CreatureState.Skill)
        {
            return;
        }

        Move.X = Input.GetAxisRaw("Horizontal");
        Move.Y = Input.GetAxisRaw("Vertical");
    }

    protected override void UpdateIdle()
    {
        _moveKeyPressed = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        if (_moveKeyPressed)
        {
            State = CreatureState.Moving;
        }
    }

    protected override void UpdateMoving()
    {
        if (State == CreatureState.Skill)
        {
            return;
        }

        _moveKeyPressed = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        if (!_moveKeyPressed)
        {
            State = CreatureState.Idle;
            return;
        }

        var moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        _rigid.DOMove(transform.position + moveDir.normalized * (Time.deltaTime * Speed), Time.deltaTime);
    }

    protected override void CheckUpdatedFlag()
    {
        currTime += Time.deltaTime;

        //초당 5번 위치 데이터 갱신
        if (currTime >= 0.2f)
        {
            C_Move movePacket = new C_Move
            {
                PosInfo = new PositionInfo()
                {
                    Pos = new Vec2()
                    {
                        X = transform.position.x,
                        Y = transform.position.y
                    },
                    State = State,
                    Move = Move,
                }
            };
            Managers.Network.Send(movePacket);

            currTime -= 0.2f;
        }
    }

    public void RefreshAdditionalStat()
    {
        WeaponDamage = 0;
        ArmorDefence = 0;

        foreach (Item item in Managers.Inven.Items.Values)
        {
            if (item.Equipped == false)
                continue;

            switch (item.ItemType)
            {
                case ItemType.Weapon:
                    WeaponDamage += ((Weapon)item).Damage;
                    break;
                case ItemType.Armor:
                    ArmorDefence += ((Armor)item).Defence;
                    break;
            }
        }
    }

    public override void UpdatePosition(S_Move movePacket)
    {/*
        PosInfo = new PositionInfo()
        {
            PosX = movePacket.PosInfo.PosX,
            PosY = movePacket.PosInfo.PosY,
            PosZ = movePacket.PosInfo.PosZ,
            MoveDir = movePacket.PosInfo.MoveDir,
            State = State
        };
        transform.position = new Vector3(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY, movePacket.PosInfo.PosZ);*/
    }
}