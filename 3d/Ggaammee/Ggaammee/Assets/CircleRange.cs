using System;
using DG.Tweening;
using EasyButtons;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CircleRange : MonoBehaviour
{
    [SerializeField] private DecalProjector range;
    [SerializeField] private Material rangeMat;
    private static readonly int Progress = Shader.PropertyToID("_Progress");

    void Start()
    {
        range.material = new Material(rangeMat);
    }

    [Button]
    public void TestProgress()
    {
        range.material.SetFloat(Progress, 0);
        DOTween.To(
            () => range.material.GetFloat(Progress),
            x => range.material.SetFloat(Progress, x),
            1f,
            3.5f
        );
    }

    public void StartRange(float diameter, float duration, Action onComplete = null)
    {
        range.size = new Vector3(diameter, diameter);

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
        ).OnComplete(() => { onComplete?.Invoke(); });
    }
}