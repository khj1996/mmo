using System;
using DG.Tweening;
using EasyButtons;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CircleRange : Poolable
{
    [SerializeField] private DecalProjector range;
    [SerializeField] private Material rangeMat;
    private static readonly int Progress = Shader.PropertyToID("_Progress");

    void Start()
    {
        range.material = new Material(rangeMat);
    }

    public void StartRange(Vector3 position, float diameter, float duration, Action onComplete = null)
    {
        gameObject.transform.position = position;
        range.size = new Vector3(diameter, diameter, 0.5f);

        float progress = 0f;
        DOTween.To(
            () => progress,
            x =>
            {
                progress = x;
                range.material.SetFloat(Progress, progress);
            },
            1f,
            duration
        ).OnComplete(() =>
        {
            onComplete?.Invoke();
            ReturnToPool();
        });
    }

    public override void OnGetFromPool()
    {
    }

    public override void OnReturnToPool()
    {
        gameObject.SetActive(false);
    }
}