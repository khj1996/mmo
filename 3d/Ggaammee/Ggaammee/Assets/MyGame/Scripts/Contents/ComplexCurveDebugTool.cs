using System;
using Cysharp.Threading.Tasks;
using EasyButtons;
using UnityEngine;

public class ComplexCurveDebugTool : MonoBehaviour
{
    [SerializeField] private ComplexCurveDesignerAsset curveAsset;
    [SerializeField] private Transform initTf;
    [SerializeField] private Transform destTf;
    [Range(5, 200), SerializeField] private int sampleCnt;

    [Header("Display Setting")] [SerializeField]
    private Transform displayObj;

    [SerializeField] private float playTime = 2.0f;

    private CurveSet _currentCurveSet;
    private bool _isPlayingMovement;
    private bool _cancellationToken;

    [Button("Set Next Curve")]
    public void SetNextCurve()
    {
        var designer = curveAsset.GetCurveDesigner();
        _currentCurveSet = designer.GetNextCurveSet();
    }

    [Button("Show Movement")]
    public async void DisplayCurveMovement()
    {
        if (!initTf || !destTf) return;
        if (_currentCurveSet == null) return;

        var designer = curveAsset.GetCurveDesigner();

        if (_isPlayingMovement)
        {
            _cancellationToken = true;
            await UniTask.WaitUntil(() => !_cancellationToken);
        }

        _isPlayingMovement = true;

        var timer = 0.0f;
        while (!_cancellationToken && timer <= playTime)
        {
            var tVal = timer / playTime;
            var currentPosition = designer.EvaluateBasePosition(initTf.position, destTf.position, tVal, _currentCurveSet);
            displayObj.position = currentPosition;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
            timer += Time.deltaTime;
        }

        displayObj.position = destTf.position;
        _cancellationToken = false;
        _isPlayingMovement = false;
    }

    private void OnDrawGizmos()
    {
        if (!initTf || !destTf) return;
        if (_currentCurveSet == null) return;

        var designer = curveAsset.GetCurveDesigner();
        var currentDrawPoint = initTf.position;

        for (var i = 0; i <= sampleCnt; i++)
        {
            var tVal = (float)i / sampleCnt;
            Gizmos.color = new Color(tVal, 1F - tVal, 1F - tVal);
            var destDrawPoint = designer.EvaluateBasePosition(initTf.position, destTf.position, tVal, _currentCurveSet);

            Gizmos.DrawLine(currentDrawPoint, destDrawPoint);
            currentDrawPoint = destDrawPoint;
        }
    }
}