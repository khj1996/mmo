﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_ScrollView<T> : UI_Base where T : UI_ScrollView_Sub
{
    [Header("---------UI_ScrollView-----------")] [SerializeField]
    protected RectOffset padding;
    public ScrollRect scrollRect;
    public float spacingHeight = 10.0f;
    public RectTransform _RectTransform;
    protected Rect visibleRect;
    protected Vector2 prevScrollPos;

    protected LinkedList<T> items { get; } = new LinkedList<T>();
    protected int maxIndex;
    public float[] ItemPosY;

    public override void Init()
    {
        items.Clear();

        scrollRect.onValueChanged.AddListener(OnScrollPosChanged);
    }


    protected void InitializeView()
    {
        UpdateScrollViewSize();
        UpdateVisibleRect();

        
        if (maxIndex == 0) return;

        if (items.Count < 1)
        {
            ItemPosY = new float[maxIndex];

            Vector2 itemTop = new Vector2(0.0f, -padding.top);

            for (int i = 0; i < maxIndex; i++)
            {
                float itemHeight = GetItemHeight();
                Vector2 itemBottom = itemTop + new Vector2(0.0f, -itemHeight);

                if ((itemTop.y <= visibleRect.y && itemTop.y >= visibleRect.y - visibleRect.height) ||
                    (itemBottom.y <= visibleRect.y && itemBottom.y >= visibleRect.y - visibleRect.height))
                {
                    T item = CreateItem(i);

                    item.Top = itemTop;
                }

                ItemPosY[i] = itemTop.y;

                itemTop = itemBottom + new Vector2(0.0f, -spacingHeight);
            }

            SetFillVisibleRectWithItems();
        }
        else
        {
            Vector2 itemTop = new Vector2(0.0f, -padding.top);
            UpdateVisibleRect();

            ItemPosY = new float[maxIndex];

            for (int i = 0; i < maxIndex; i++)
            {
                float itemHeight = GetItemHeight();
                Vector2 itemBottom = itemTop + new Vector2(0.0f, -itemHeight);

                ItemPosY[i] = itemTop.y;

                itemTop = itemBottom + new Vector2(0.0f, -spacingHeight);
            }

            LinkedListNode<T> node = items.First;

            LinkedList<T> tempList = new LinkedList<T>();
            int newIndex = 0;
            while (node != null)
            {
                node.Value._index = newIndex;

                items.RemoveFirst();
                tempList.AddLast(node);
                node = items.First;
                newIndex++;
            }

            node = tempList.First;
            while (node != null)
            {
                tempList.RemoveFirst();
                items.AddLast(node);

                node = tempList.First;
            }

            node = items.First;

            UpdateItemForIndex(node.Value, node.Value._index);
            node.Value._RectTransform.anchoredPosition = Vector2.up * ItemPosY[node.Value._index];
            node = node.Next;

            while (node != null)
            {
                UpdateItemForIndex(node.Value, node.Previous.Value._index + 1);

                if (node.Value._index < ItemPosY.Length)
                {
                    node.Value.Top = Vector2.up * ItemPosY[node.Value._index];
                    node.Value._RectTransform.anchoredPosition = node.Value.Top;
                }

                node = node.Next;
            }

            UpdateItems(1);
            SetFillVisibleRectWithItems();
        }
    }

    protected void UpdateScrollViewSize()
    {
        float contentHeight = 0.0f;
        for (int i = 0; i < maxIndex; i++)
        {
            contentHeight += GetItemHeight();

            if (i > 0)
            {
                contentHeight += spacingHeight;
            }
        }

        Vector2 sizeDelta = scrollRect.content.sizeDelta;
        sizeDelta.y = padding.top + contentHeight + padding.bottom;
        scrollRect.content.sizeDelta = sizeDelta;
    }

    public void OnScrollPosChanged(Vector2 scrollPos)
    {
        UpdateVisibleRect();
        UpdateItems(scrollPos.y < prevScrollPos.y ? 1 : -1);
        prevScrollPos = scrollPos;
    }

    protected void UpdateVisibleRect()
    {
        visibleRect.x = scrollRect.content.anchoredPosition.x;
        visibleRect.y = -scrollRect.content.anchoredPosition.y;
        visibleRect.width = _RectTransform.rect.width;
        visibleRect.height = _RectTransform.rect.height;
    }

    protected void UpdateItems(int scrollDirection)
    {
        if (items.Count < 1)
        {
            return;
        }

        if (scrollDirection > 0)
        {
            T firstViewSub = items.First.Value;

            while (firstViewSub.Bottom.y > visibleRect.y && items.Last.Value._index < maxIndex - 1)
            {
                T lastViewSub = items.Last.Value;

                firstViewSub.Top = Vector2.up * ItemPosY[lastViewSub._index + 1];

                UpdateItemForIndex(firstViewSub, lastViewSub._index + 1);

                items.AddLast(firstViewSub);

                items.RemoveFirst();

                firstViewSub = items.First.Value;
            }

            SetFillVisibleRectWithItems();
        }
        else if (scrollDirection < 0)
        {
            T lastViewSub = items.Last.Value;
            T firstViewSub = items.First.Value;

            while ((lastViewSub.Top.y < visibleRect.y - visibleRect.height || firstViewSub.Bottom.y < visibleRect.y) && items.First.Value._index > 0)
            {
                firstViewSub = items.First.Value;

                UpdateItemForIndex(lastViewSub, firstViewSub._index - 1);

                lastViewSub.Bottom = Vector2.up * ItemPosY[firstViewSub._index - 1] - new Vector2(0.0f, GetItemHeight());
                items.AddFirst(lastViewSub);
                items.RemoveLast();
                lastViewSub = items.Last.Value;
            }

            SetFillVisibleRectWithItems();
        }
    }

    protected void SetFillVisibleRectWithItems()
    {
        if (items.Count < 1)
        {
            return;
        }

        T lastItem = items.Last.Value;
        int nextItemDataIndex = lastItem._index + 1;
        Vector2 nextItemTop = nextItemDataIndex < maxIndex ? Vector2.up * ItemPosY[nextItemDataIndex] : lastItem.Bottom + new Vector2(0.0f, -spacingHeight);


        while (nextItemDataIndex < maxIndex && nextItemTop.y >= visibleRect.y - visibleRect.height)
        {
            T item = CreateItem(nextItemDataIndex);
            item.Top = nextItemTop;

            lastItem = item;
            nextItemDataIndex = lastItem._index + 1;

            if (nextItemDataIndex >= maxIndex)
                break;

            nextItemTop = Vector2.up * ItemPosY[nextItemDataIndex];
        }
    }

    protected T CreateItem(int index)
    {
        var item = Managers.UI.MakeSubItem<T>(scrollRect.content.transform);

        Vector3 scale = Vector3.one;
        Vector2 sizeDelta = item._RectTransform.sizeDelta;
        Vector2 offsetMin = item._RectTransform.offsetMin;
        Vector2 offsetMax = item._RectTransform.offsetMax;

        item.transform.localPosition = Vector3.zero;

        item.transform.localScale = scale;
        item._RectTransform.sizeDelta = sizeDelta;
        item._RectTransform.offsetMin = offsetMin;
        item._RectTransform.offsetMax = offsetMax;

        UpdateItemForIndex(item, index);

        items.AddLast(item);

        return item;
    }

    protected void UpdateItemForIndex(UI_ScrollView_Sub item, int _index)
    {
        item._index = _index;

        if (item._index >= 0 && item._index <= maxIndex - 1)
        {
            item.gameObject.SetActive(true);
            item.RefreshUI(_index);
            item.Height = GetItemHeight();
        }
        else
        {
            item.gameObject.SetActive(false);
        }
    }

    protected float GetItemHeight()
    {
        if (maxIndex <= 0)
            return 0;

        //나중에 별도로 받아서 쓰도록 수정 필요
        return 108;
    }
}