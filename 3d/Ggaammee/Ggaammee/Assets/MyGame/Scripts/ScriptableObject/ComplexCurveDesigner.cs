using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ComplexCurveDesigner
{
    [SerializeField] private CurveSelectionPolicy progressionCurveSelectionPolicy;
    [SerializeField] private CurveSelectionPolicy parallelPlaneDecorationCurveSelectionPolicy;
    [SerializeField] private CurveSelectionPolicy verticalPlaneDecorateCurveSelectionPolicy;

    [SerializeField] private List<AnimationCurve> progressionCurves;
    [SerializeField] private List<AnimationCurve> parallelPlaneDecorationCurves;
    [SerializeField] private List<AnimationCurve> verticalPlaneDecorationCurves;

    private int _lastSelectedProgressionCurveIdx = -1;
    private int _lastSelectedParallelDecorationCurveIdx = -1;
    private int _lastSelectedVerticalDecorationCurveIdx = -1;

    private List<int> _progressionCurveBag = new();
    private List<int> _parallelDecorationCurveBag = new();
    private List<int> _verticalDecorationCurveBag = new();

    public void Reset()
    {
        _lastSelectedProgressionCurveIdx = -1;
        _lastSelectedParallelDecorationCurveIdx = -1;
        _lastSelectedVerticalDecorationCurveIdx = -1;

        _progressionCurveBag.Clear();
        _parallelDecorationCurveBag.Clear();
        _verticalDecorationCurveBag.Clear();
    }

    public CurveSet GetNextCurveSet()
    {
        return new CurveSet
        {
            CurrentProgressionCurve = progressionCurves.Count > 0
                ? progressionCurveSelectionPolicy switch
                {
                    CurveSelectionPolicy.Random => ExtractCurveRandom(progressionCurves),
                    CurveSelectionPolicy.Sequential => ExtractCurveSequential(progressionCurves, false, ref _lastSelectedProgressionCurveIdx),
                    CurveSelectionPolicy.PingPong => ExtractCurveSequential(progressionCurves, true, ref _lastSelectedProgressionCurveIdx),
                    CurveSelectionPolicy.RandomBag => ExtractCurveRandomBag(progressionCurves, _progressionCurveBag),
                    _ => throw new ArgumentOutOfRangeException()
                }
                : null,
            CurrentParallelDecorationCurve = parallelPlaneDecorationCurves.Count > 0
                ? parallelPlaneDecorationCurveSelectionPolicy switch
                {
                    CurveSelectionPolicy.Random => ExtractCurveRandom(parallelPlaneDecorationCurves),
                    CurveSelectionPolicy.Sequential => ExtractCurveSequential(parallelPlaneDecorationCurves, false, ref _lastSelectedParallelDecorationCurveIdx),
                    CurveSelectionPolicy.PingPong => ExtractCurveSequential(parallelPlaneDecorationCurves, true, ref _lastSelectedParallelDecorationCurveIdx),
                    CurveSelectionPolicy.RandomBag => ExtractCurveRandomBag(parallelPlaneDecorationCurves, _parallelDecorationCurveBag),
                    _ => throw new ArgumentOutOfRangeException()
                }
                : null,
            CurrentVerticalDecorationCurve = verticalPlaneDecorationCurves.Count > 0
                ? verticalPlaneDecorateCurveSelectionPolicy switch
                {
                    CurveSelectionPolicy.Random => ExtractCurveRandom(verticalPlaneDecorationCurves),
                    CurveSelectionPolicy.Sequential => ExtractCurveSequential(verticalPlaneDecorationCurves, false, ref _lastSelectedVerticalDecorationCurveIdx),
                    CurveSelectionPolicy.PingPong => ExtractCurveSequential(verticalPlaneDecorationCurves, true, ref _lastSelectedVerticalDecorationCurveIdx),
                    CurveSelectionPolicy.RandomBag => ExtractCurveRandomBag(verticalPlaneDecorationCurves, _verticalDecorationCurveBag),
                    _ => throw new ArgumentOutOfRangeException()
                }
                : null
        };
    }

    public Vector3 EvaluateBasePosition(Vector3 initPosition, Vector3 destPosition, float tVal, CurveSet curveSetToSimulate)
    {
        var yAxisVector = Vector3.up;
        var shootVector = (destPosition - initPosition).normalized;

        var simulatePosition = new Vector3(
            curveSetToSimulate.CurrentParallelDecorationCurve?.Evaluate(tVal) ?? 0,
            curveSetToSimulate.CurrentProgressionCurve?.Evaluate(tVal) ?? 0,
            curveSetToSimulate.CurrentVerticalDecorationCurve?.Evaluate(tVal) ?? 0);

        var lerpPosition = RangeFreeLerp(initPosition, destPosition, simulatePosition.y);

        simulatePosition.y = 0;
        var quaternion = Quaternion.FromToRotation(yAxisVector, shootVector);
        var decorationOffset = quaternion * simulatePosition;

        var applyPosition = lerpPosition + decorationOffset;
        return applyPosition;
    }

    private static AnimationCurve ExtractCurveRandom(IReadOnlyList<AnimationCurve> extractionSource)
        => extractionSource[Random.Range(0, extractionSource.Count)];

    private static AnimationCurve ExtractCurveSequential(IReadOnlyList<AnimationCurve> extractionSource, bool isPingPong, ref int lastSelected)
    {
        int selected;
        if (isPingPong)
        {
            selected = (int)Mathf.PingPong(++lastSelected, extractionSource.Count - 1);
        }
        else
        {
            lastSelected++;
            if (lastSelected >= extractionSource.Count)
                lastSelected = 0;
            selected = lastSelected;
        }

        return extractionSource[selected];
    }

    private static AnimationCurve ExtractCurveRandomBag(List<AnimationCurve> extractionSource, ICollection<int> duplicateBag)
    {
        if (duplicateBag.Count == extractionSource.Count)
        {
            duplicateBag.Clear();
        }

        var selectedIdx = Random.Range(0, extractionSource.Count);
        duplicateBag.Add(selectedIdx);
        return extractionSource[selectedIdx];
    }

    private static float RangeFreeLerp(float a, float b, float t)
    {
        return (1 - t) * a + t * b;
    }

    private static float RangeFreeInverseLerp(float a, float b, float val)
    {
        return (val - a) / (b - a);
    }

    private static Vector3 RangeFreeLerp(Vector3 a, Vector3 b, float t)
    {
        return a + (b - a) * t;
    }

    public enum CurveSelectionPolicy
    {
        Random,
        Sequential,
        PingPong,
        RandomBag
    }
}

public class CurveSet
{
    public AnimationCurve CurrentProgressionCurve;
    public AnimationCurve CurrentParallelDecorationCurve;
    public AnimationCurve CurrentVerticalDecorationCurve;
}