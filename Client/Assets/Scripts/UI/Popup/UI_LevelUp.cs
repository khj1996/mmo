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

    public void LevelUp(StatInfo beforeStat, StatInfo afterStat)
    {
        CanvasGroup.alpha = 1;
        rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 870f), 0.2f)
            .SetEase(Ease.InOutSine);

        // UI_StatInfo 요소들을 순서대로 나타나게 설정
        for (int i = 0; i < StatInfos.Length; i++)
        {
            var statInfoRect = StatInfos[i].GetComponent<RectTransform>();

            // 시작 위치와 투명도 초기화
            statInfoRect.anchoredPosition = new Vector2(-100, statInfoRect.anchoredPosition.y);
            StatInfos[i].CanvasGroup.alpha = 0;

            var before = GetStatData(beforeStat, i);
            var after = GetStatData(afterStat, i);

            StatInfos[i].SetData(before, after);

            float delay = 0.1f + i * 0.2f;

            // 슬라이드와 페이드 인 효과 설정
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