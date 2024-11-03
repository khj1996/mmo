using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{
    private HpBar _hpBar;
    private const float hpBarOffsetY = 0.5f;
    private const float destroyEffectDelay = 0.5f;

    public override StatInfo Stat
    {
        get => base.Stat;
        set
        {
            if (!base.Stat.Equals(value))
            {
                base.Stat = value;
                UpdateHpBar();
            }
        }
    }

    public override int Hp
    {
        get => base.Hp;
        set
        {
            if (base.Hp != value)
            {
                base.Hp = value;
                UpdateHpBar();
            }
        }
    }


    protected void AddHpBar()
    {
        if (_hpBar != null) return;

        GameObject go = Managers.Resource.Instantiate("UI/HpBar.prefab", transform);
        go.transform.localPosition = new Vector3(0, hpBarOffsetY, 0);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HpBar>();

        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        if (_hpBar == null || Stat.MaxHp <= 0)
            return;

        float ratio = (float)Hp / Stat.MaxHp;
        _hpBar.SetHpBar(ratio);
    }

    protected override void Init()
    {
        base.Init();
        AddHpBar();
    }

    public virtual void OnDamaged()
    {
    }

    public virtual void OnDead()
    {
        State = CreatureState.Dead;

        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect.prefab");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        Destroy(effect, destroyEffectDelay);
    }

    public virtual void UseSkill(S_Skill skillPacket)
    {
    }
}