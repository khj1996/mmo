using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
    private bool _moveKeyPressed = false;

    private float currTime = 0.0f;

    public int WeaponDamage { get; private set; }
    public int ArmorDefence { get; private set; }

    protected override void Init()
    {
        base.Init();
        RefreshAdditionalStat();
    }

    protected override void UpdateController()
    {
        GetUIKeyInput();

        GetDirInput();

        CheckUpdatedFlag();

        UpdateMovDir();

        base.UpdateController();
    }

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (_moveKeyPressed)
        {
            return;
        }

        if (_coSkillCooltime == null && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Skill !");

            C_Skill skill = new C_Skill() { Info = new SkillInfo() };
            skill.Info.SkillId = 2;
            Managers.Network.Send(skill);

            _coSkillCooltime = StartCoroutine("CoInputCooltime", 0.2f);
        }
    }

    Coroutine _coSkillCooltime;

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
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            UI_Inventory invenUI = gameSceneUI.InvenUI;

            if (invenUI.gameObject.activeSelf)
            {
                invenUI.gameObject.SetActive(false);
            }
            else
            {
                invenUI.gameObject.SetActive(true);
                invenUI.RefreshUI();
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
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
    }

    // 키보드 입력
    void GetDirInput()
    {
        _moveKeyPressed = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        if (_moveKeyPressed)
        {
            State = CreatureState.Moving;
        }
    }

    public void UpdateMovDir()
    {
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            MoveDir = MoveDir.Up;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            MoveDir = MoveDir.Down;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            MoveDir = MoveDir.Left;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            MoveDir = MoveDir.Right;
        }
    }

    protected override void UpdateMoving()
    {
        if (_moveKeyPressed == false)
        {
            State = CreatureState.Idle;
            return;
        }

        var moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        transform.DOMove(transform.position + moveDir.normalized * (Time.deltaTime * Speed), Time.deltaTime);


        /*if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            State = CreatureState.Idle;
        }
        else*/
        {
            //State = CreatureState.Moving;
        }
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
                    PosX = transform.position.x,
                    PosY = transform.position.y,
                    PosZ = transform.position.z,
                    State = State,
                    MoveDir = MoveDir
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
}