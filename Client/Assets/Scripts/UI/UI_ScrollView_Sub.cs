using UnityEngine;
using UnityEngine.Serialization;

public abstract class UI_ScrollView_Sub : UI_Base
{
    public RectTransform _RectTransform => GetComponent<RectTransform>();

    public int _index;

    public float Height
    {
        get { return _RectTransform.sizeDelta.y; }
        set
        {
            Vector2 sizeDelta = _RectTransform.sizeDelta;
            sizeDelta.y = value;
            _RectTransform.sizeDelta = sizeDelta;
        }
    }

    public abstract void RefreshUI(int _index);

    public Vector2 Top
    {
        get
        {
            Vector3[] corners = new Vector3[4];
            _RectTransform.GetLocalCorners(corners);
            return _RectTransform.anchoredPosition + new Vector2(0.0f, corners[1].y);
        }
        set
        {
            Vector3[] corners = new Vector3[4];
            _RectTransform.GetLocalCorners(corners);
            _RectTransform.anchoredPosition = value - new Vector2(0.0f, corners[1].y);
        }
    }

    public Vector2 Bottom
    {
        get
        {
            Vector3[] corners = new Vector3[4];
            _RectTransform.GetLocalCorners(corners);
            return _RectTransform.anchoredPosition + new Vector2(0.0f, corners[3].y);
        }
        set
        {
            Vector3[] corners = new Vector3[4];
            _RectTransform.GetLocalCorners(corners);
            _RectTransform.anchoredPosition = value - new Vector2(0.0f, corners[3].y);
        }
    }
}