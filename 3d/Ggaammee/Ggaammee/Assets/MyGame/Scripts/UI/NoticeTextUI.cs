using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EasyButtons;
using TMPro;
using UnityEngine;

public enum ShowType
{
    Persistent,
    Timed,
    Normal
}

public struct TextData
{
    public readonly ShowType type;
    public readonly string content;
    public readonly float duration;

    public TextData(ShowType type, string content, float duration)
    {
        this.type = type;
        this.content = content;
        this.duration = duration;
    }
}

public class NoticeTextUI : MonoBehaviour
{
    public static NoticeTextUI Instance { get; private set; }

    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup canvasGroup;

    private bool isShowPersistent = false;
    private Coroutine persistentCoroutine;
    private TextData persistentTextData;

    private Queue<TextData> textQueue = new Queue<TextData>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowText(ShowType type, string _text, float duration = 1.0f)
    {
        TextData newTextData = new TextData(type, _text, duration);

        if (type == ShowType.Persistent)
        {
            HandlePersistentText(newTextData);
            return;
        }

        textQueue.Enqueue(newTextData);

        if (!isShowPersistent && textQueue.Count == 1)
        {
            ShowNextText();
        }
    }

    private void HandlePersistentText(TextData newTextData)
    {
        if (isShowPersistent)
        {
            StopCoroutine(persistentCoroutine);
        }

        persistentTextData = newTextData;
        isShowPersistent = true;
        persistentCoroutine = StartCoroutine(ShowPersistentText());
    }

    private void ShowNextText()
    {
        if (textQueue.Count == 0)
        {
            if (isShowPersistent)
            {
                RestorePersistentText();
            }

            return;
        }

        TextData nextTextData = textQueue.Dequeue();

        if (isShowPersistent && nextTextData.type != ShowType.Persistent)
        {
            textQueue.Enqueue(nextTextData);
            return;
        }

        DisplayText(nextTextData.content, nextTextData.duration, ShowNextText);
    }

    private IEnumerator ShowPersistentText()
    {
        SetText(persistentTextData.content);

        while (isShowPersistent)
        {
            while (textQueue.Count > 0)
            {
                TextData nextTextData = textQueue.Dequeue();
                yield return DisplayTextCoroutine(nextTextData.content, nextTextData.duration);
            }

            yield return null;
        }

        FadeOutText();
    }

    private void RestorePersistentText()
    {
        if (isShowPersistent)
        {
            SetText(persistentTextData.content);
        }
    }

    private void DisplayText(string message, float duration, TweenCallback onComplete = null)
    {
        SetText(message);

        DOTween.Sequence()
            .AppendInterval(duration)
            .Append(canvasGroup.DOFade(0, 0.5f))
            .OnComplete(() =>
            {
                text.text = "";
                onComplete?.Invoke();
            });
    }

    private IEnumerator DisplayTextCoroutine(string message, float duration)
    {
        SetText(message);

        yield return DOTween.Sequence()
            .AppendInterval(duration)
            .Append(canvasGroup.DOFade(0, 0.5f))
            .OnComplete(() => SetText(persistentTextData.content))
            .WaitForCompletion();
    }

    private void SetText(string message)
    {
        text.text = message;
        canvasGroup.alpha = 1;
    }

    private void FadeOutText()
    {
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => text.text = "");
    }

    public void StopPersistentText()
    {
        if (isShowPersistent)
        {
            isShowPersistent = false;
            if (persistentCoroutine != null)
            {
                StopCoroutine(persistentCoroutine);
                persistentCoroutine = null;
            }

            FadeOutText();
        }
    }

    [Button]
    public void TextTest()
    {
        ShowText(ShowType.Timed, "일반 메시지", 2.0f);
    }

    [Button]
    public void PersistentTest()
    {
        ShowText(ShowType.Persistent, "지속 메시지", 0);
    }

    [Button]
    public void StopPersistentTest()
    {
        StopPersistentText();
    }
}