using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Google.Protobuf.Protocol;
using UnityEngine;

public class UI_LevelUp : UI_Popup
{
    public CanvasGroup CanvasGroup;
    public RectTransform rectTransform;
    public UI_StatInfo[] StatInfos;


    public override void Init()
    {
        base.Init();
        CloseBtn.gameObject.BindEvent((evt) =>
        {
            Managers.Object.MyPlayer.isLevelUp = false;
            Managers.UI.ClosePopupUI(this);
            
        });
    }

    public void LevelUp(StatInfo beforeStat, StatInfo afterStat)
    {
        CanvasGroup.alpha = 1;
        rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 870f), 1f)
            .SetEase(Ease.InOutSine);

        for (int i = 0; i < StatInfos.Length; i++)
        {
            var statInfoRect = StatInfos[i].GetComponent<RectTransform>();

            statInfoRect.anchoredPosition = new Vector2(-100, statInfoRect.anchoredPosition.y);
            StatInfos[i].CanvasGroup.alpha = 0;

            var before = GetStatData(beforeStat, i);
            var after = GetStatData(afterStat, i);

            StatInfos[i].SetData(before, after);

            float delay = 0.1f + i * 0.2f;

            statInfoRect.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutSine).SetDelay(delay);
            StatInfos[i].CanvasGroup.DOFade(1, 0.5f).SetDelay(delay);
        }
    }

    public int GetStatData(StatInfo statInfo, int statId)
    {
        switch (statId)
        {
            case 0:
                return statInfo.Level;
            case 1:
                return statInfo.MaxHp;
            case 2:
                return statInfo.Attack;
            case 3:
                return (int)statInfo.Speed;
        }

        return 0;
    }
}