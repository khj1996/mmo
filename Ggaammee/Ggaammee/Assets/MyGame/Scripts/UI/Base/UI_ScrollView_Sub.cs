using UnityEngine;
using UnityEngine.Serialization;

public abstract class UI_ScrollView_Sub : UI_Base
{
    [SerializeField] public RectTransform RectTransform => GetComponent<RectTransform>();

    public int _index;

    public float Height
    {
        get { return RectTransform.sizeDelta.y; }
        set
        {
            Vector2 sizeDelta = RectTransform.sizeDelta;
            sizeDelta.y = value;
            RectTransform.sizeDelta = sizeDelta;
        }
    }

    public abstract void RefreshUI(int _index);

    public Vector2 Top
    {
        get
        {
            Vector3[] corners = new Vector3[4];
            RectTransform.GetLocalCorners(corners);
            return RectTransform.anchoredPosition + new Vector2(0.0f, corners[1].y);
        }
        set
        {
            Vector3[] corners = new Vector3[4];
            RectTransform.GetLocalCorners(corners);
            RectTransform.anchoredPosition = value - new Vector2(0.0f, corners[1].y);
        }
    }

    public Vector2 Bottom
    {
        get
        {
            Vector3[] corners = new Vector3[4];
            RectTransform.GetLocalCorners(corners);
            return RectTransform.anchoredPosition + new Vector2(0.0f, corners[3].y);
        }
        set
        {
            Vector3[] corners = new Vector3[4];
            RectTransform.GetLocalCorners(corners);
            RectTransform.anchoredPosition = value - new Vector2(0.0f, corners[3].y);
        }
    }
}