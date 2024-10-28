using Google.Protobuf.Protocol;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyPlayerController : PlayerController
{
    private UI_Joystick _Joystick;
    private float currTime = 0.0f;

    private Rigidbody2D _rigidbody2D;


    Coroutine _coSkillCooltime;

    public int WeaponDamage { get; private set; }
    public int ArmorDefence { get; private set; }

    private UI_GameScene gameSceneUI;
    [SerializeField] UI_ExpBar uiExpBar;

    public int Exp
    {
        get => Stat.TotalExp;
        set
        {
            Stat.TotalExp = value;
            UpdateExpBar();
        }
    }

    public void UpdateExpBar()
    {
        if (!uiExpBar)
        {
            uiExpBar = FindObjectOfType<UI_ExpBar>();
        }

        uiExpBar.SetExpBar(Exp);
    }


    protected override void Init()
    {
        base.Init();
        RefreshAdditionalStat();
        gameSceneUI = Managers.UI.CurrentSceneUI as UI_GameScene;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _Joystick = FindObjectOfType<UI_Joystick>();
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
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

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

    void GetUIKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
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

        Move = new Vec2()
        {
            X = _Joystick.inputDirection.x,
            Y = _Joystick.inputDirection.y
        };

        LookDir = new Vec2()
        {
            X = Move.X,
            Y = Move.Y,
        };
    }


    private Tween moveTween;

    protected override void UpdateMoving()
    {
        var destPos = new Vector3(Pos.X, Pos.Y, transform.position.z);
        var distance = Vector2.Distance(_rigidbody2D.position, destPos);

        if (distance < Mathf.Epsilon && Move.X == 0 && Move.Y == 0)
        {
            if (moveTween != null)
            {
                moveTween.Kill();
                moveTween = null;
            }

            State = CreatureState.Idle;
            UpdateAnimation();
        }
        else if (moveTween == null || !moveTween.IsPlaying())
        {
            // 이동 스텝을 계산
            float step = Speed * Time.deltaTime;

            // 이동을 DOTween으로 설정
            if (moveTween == null || !moveTween.IsPlaying())
            {
                // 기존 트윈이 존재하면 종료
                if (moveTween != null)
                {
                    moveTween.Kill();
                }

                // duration 계산
                float duration = distance / Speed;

                // 이동 트윈 설정
                moveTween = _rigidbody2D.DOMove(destPos, duration)
                    .SetEase(Ease.Linear) // 선형 이동
                    .OnComplete(() =>
                    {
                        moveTween = null; // 트윈 완료 시 null로 설정
                    });
            }
        }
    }

    protected override void CheckUpdatedFlag()
    {
        currTime += Time.deltaTime;

        if (statusChanged || currTime >= 2.0f)
        {
            var moveDir = new Vector2(Move.X, Move.Y).normalized;

            C_Move movePacket = new C_Move
            {
                PosInfo = new PositionInfo()
                {
                    Pos = new Vec2()
                    {
                        X = transform.position.x,
                        Y = transform.position.y
                    },
                    Move = new Vec2()
                    {
                        X = moveDir.x,
                        Y = moveDir.y
                    },
                }
            };
            Managers.Network.Send(movePacket);

            statusChanged = false;
            currTime = 0;
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