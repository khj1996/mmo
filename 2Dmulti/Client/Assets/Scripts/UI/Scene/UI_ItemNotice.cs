using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UI_ItemNotice : UI_Base
{
    private Queue<UI_NoticeItemSub> _uiNoticeItemSubs = new Queue<UI_NoticeItemSub>();
    private Queue<Item> _waitingItems = new Queue<Item>();
    private Stack<UI_NoticeItemSub> _itemPool = new Stack<UI_NoticeItemSub>();

    private const int MaxActiveItems = 4;

    public override void Init()
    {
        Managers.Inven.AddNewItemAction -= AddNotice;
        Managers.Inven.AddNewItemAction += AddNotice;
    }

    public void AddNotice(Item item)
    {
        if (_uiNoticeItemSubs.Count >= MaxActiveItems)
        {
            _waitingItems.Enqueue(item);
        }
        else
        {
            ShowNotice(item);
        }
    }

    private void ShowNotice(Item item)
    {
        UI_NoticeItemSub noticeItem = GetItem();
        noticeItem.SetItemInfo(item);

        noticeItem.transform.localPosition = new Vector3(0, -170, 0);
        noticeItem.canvasGroup.alpha = 1;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.2f)
            .Append(noticeItem.transform.DOLocalMoveY(0, 1.5f).SetEase(Ease.OutQuad))
            .Join(noticeItem.canvasGroup.DOFade(0, 1.5f))
            .OnComplete(() =>
            {
                ReturnItem(noticeItem);

                if (_waitingItems.Count > 0)
                {
                    Item nextItem = _waitingItems.Dequeue();
                    ShowNotice(nextItem);
                }
            });

        _uiNoticeItemSubs.Enqueue(noticeItem);
    }

    private UI_NoticeItemSub GetItem()
    {
        UI_NoticeItemSub item;
        if (_itemPool.Count > 0)
        {
            item = _itemPool.Pop();
            item.gameObject.SetActive(true);
        }
        else
        {
            item = Managers.UI.MakeSubItem<UI_NoticeItemSub>(gameObject.transform);
        }

        return item;
    }

    private void ReturnItem(UI_NoticeItemSub item)
    {
        _uiNoticeItemSubs.Dequeue();
        item.gameObject.SetActive(false);
        _itemPool.Push(item);
    }
}