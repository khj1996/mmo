using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_Base
{
    public ScrollRect scrollRect;

    private LinkedList<UI_Inventory_Sub> InventorySubs { get; } = new LinkedList<UI_Inventory_Sub>();

    [SerializeField] private TMP_Text _goldTMP;

    private int maxLine;
    private int poolSize;
    private float prefabH;
    private int currentIndex;


    public override void Init()
    {
        InventorySubs.Clear();

        foreach (Transform child in scrollRect.content)
            Destroy(child.gameObject);


        //임시 76칸 고정 = 16줄 8줄만 만들고 오브젝트 풀링
        maxLine = Managers.Inven.SlotLen / 5 + 1;

        //최대로 생성하려는 인벤토리 줄(오브젝트 풀링)
        poolSize = (maxLine >= 10) ? 10 : maxLine;


        //필요한 수량만큼 라인 생성(최대 10)
        for (int i = 0; i < poolSize; i++)
        {
            var sc = Managers.UI.MakeSubItem<UI_Inventory_Sub>(scrollRect.content.transform);
            sc.gameObject.transform.localScale = Vector3.one;
            sc.line = i;
            InventorySubs.AddLast(sc);
        }

        //콘텐츠 오브젝트 길이(오브젝트풀 길이X)
        prefabH = InventorySubs.First.Value.GetComponent<RectTransform>().rect.height;
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, prefabH * maxLine);


        foreach (var item in InventorySubs)
        {
            item.transform.localPosition = new Vector3(0, -prefabH * 0.5f - (item.line * prefabH), 0);
        }


        //초기화
        RefreshUI(Define.InvenRefreshType.All);

        //스크롤 위치 변경시 프리팹 위치이동 및 갱신 위해 필요
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void OnScroll(Vector2 scrollPosition)
    {
        var contentY = scrollRect.content.anchoredPosition.y;

        //현재 contentRect의 y값에 맞춰 최소 인덱스를 구한다.
        int firstIndex = Mathf.Max(0, Mathf.FloorToInt(contentY / prefabH));

        //만약 이전 위치와 현재 위치가 달라졌다면?
        if (currentIndex != firstIndex)
        {
            //위치 인덱스의 차이를 구한다.
            int diffIndex = currentIndex - firstIndex;

            if (diffIndex < 0)
            {
                //그 인덱스 차이 만큼 반복해서 슬롯의 위치를 위에서 아래로 이동시킨다.
                for (int i = 0, cnt = Mathf.Abs(diffIndex); i < cnt; i++)
                {
                    var item = InventorySubs.First.Value;

                    if (maxLine < (firstIndex + poolSize + i))
                    {
                        item.gameObject.SetActive(false);
                    }
                    else
                    {
                        item.gameObject.SetActive(true);
                        item.line = (poolSize - 1) + firstIndex;
                        item.RefreshUI(Define.InvenRefreshType.Slot);
                    }

                    InventorySubs.RemoveFirst();
                    InventorySubs.AddLast(item);
                    item.transform.localPosition = new Vector3(0, (-(currentIndex + poolSize + i) * prefabH) - prefabH * 0.5f, 0);
                }
            }
            else if (diffIndex > 0)
            {
                //그 인덱스 차이 만큼 반복해서 슬롯의 위치를 아래에서 위로 이동시킨다.
                for (int i = 0, cnt = Mathf.Abs(diffIndex); i < cnt; i++)
                {
                    var item = InventorySubs.Last.Value;
                    item.gameObject.SetActive(true);
                    InventorySubs.RemoveLast();
                    InventorySubs.AddFirst(item);
                    item.transform.localPosition = new Vector3(0, (-(firstIndex - i) * prefabH) - prefabH * 0.5f, 0);
                    item.line = firstIndex;
                    item.RefreshUI(Define.InvenRefreshType.Slot);
                }
            }

            //위치 인덱스를 갱신한다.
            currentIndex = firstIndex;
        }
    }

    public void RefreshUI(Define.InvenRefreshType type)
    {
        if (type == Define.InvenRefreshType.Currency || type == Define.InvenRefreshType.All)
        {
            _goldTMP.text = Managers.Inven.Items.Values.First(x => x.TemplateId == 400000).Count.ToString();
        }


        if (Managers.Inven.Items.Count == 0)
            return;

        foreach (var sub in InventorySubs)
        {
            sub.RefreshUI(Define.InvenRefreshType.Slot);
        }
    }
}