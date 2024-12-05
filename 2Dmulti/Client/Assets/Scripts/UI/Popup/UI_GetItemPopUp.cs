using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_GetItemPopUp : UI_Popup
{
    public ScrollRect _scrollRect;
    public AnimCurve _animCurve;
    public UI_Item itemPrefab;
    public List<UI_Item> items = new List<UI_Item>();

    private float tweenInterval = 0.1f;

    public override void Init()
    {
        base.Init();
        CloseBtn.gameObject.BindEvent(ClosePopupUI);
    }


    public void OpenPopUpMultiItem(List<ItemInfo> itemDatas)
    {
        if (Managers.Object.MyPlayer.isLevelUp)
        {
            Managers.UI.ClosePopupUI(this);
        }
        
        int itemDataCount = itemDatas.Count;

        if (items.Count < itemDataCount)
        {
            foreach (Transform child in _scrollRect.content.transform)
                items.Add(child.GetComponent<UI_Item>());

            while (items.Count < itemDataCount)
            {
                var uiItem = Instantiate(this.itemPrefab, _scrollRect.content);
                uiItem.gameObject.SetActive(false);
                items.Add(uiItem);
            }
        }


        float startTime = _animCurve.curve.keys[0].time;
        float endTime = _animCurve.curve.keys[_animCurve.curve.length - 1].time;
        float duration = endTime - startTime;

        for (int i = 0; i < items.Count; i++)
        {
            if (i >= itemDataCount)
            {
                items[i].gameObject.SetActive(false);
                continue;
            }

            Item item = Item.MakeItem(itemDatas[i]);
            items[i].SetItem(item);

            int currentIndex = i;
            float delay = tweenInterval * i;

            items[i].gameObject.SetActive(false);

            DOTween.To(() => startTime, time => UpdateScale(currentIndex, time), endTime, duration)
                .SetEase(Ease.Linear)
                .SetDelay(delay)
                .OnStart(() => { items[currentIndex].gameObject.SetActive(true); });
        }
    }

    void UpdateScale(int index, float time)
    {
        float curveValue = _animCurve.curve.Evaluate(time);

        items[index].transform.localScale = Vector3.one * curveValue;
    }
}