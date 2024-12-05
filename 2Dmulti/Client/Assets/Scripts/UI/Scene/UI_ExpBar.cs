using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_ExpBar : UI_Base
{
    [SerializeField] public Image expBar;

    public override void Init()
    {
    }

    public void SetExpBar(int exp)
    {
        var currentLevelData = Managers.Data.GetCurrentLevelData(exp);

        float ratio = (currentLevelData.expToNextLevel > 0) ? ((float)currentLevelData.currentExpInLevel / currentLevelData.expToNextLevel) : 1.0f;

        expBar.DOFillAmount(ratio, 0.5f);
    }
}