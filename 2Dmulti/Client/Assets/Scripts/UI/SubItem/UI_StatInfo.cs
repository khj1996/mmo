using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UI_StatInfo : UI_Base
{
    [SerializeField] public CanvasGroup CanvasGroup;
    [SerializeField] public TMP_Text StatBefore;
    [SerializeField] public TMP_Text StatAfter;
    [SerializeField] public TMP_Text StatGrow;
    [SerializeField] public Image[] Arrows;
    private float animDuration = 0.7f; // 오타 수정

    public override void Init()
    {
    }

    public void SetData(int before, int after)
    {
        StatBefore.text = before.ToString();
        StatAfter.text = after.ToString();
        StatGrow.text = $"+{(after - before)}";


        Arrows[0].DOKill();
        Arrows[1].DOKill();

        Arrows[0].DOFade(0, animDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        Arrows[1].DOFade(1, animDuration * .5f).OnComplete(() =>
            Arrows[1].DOFade(0, animDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo)
        );
    }
}