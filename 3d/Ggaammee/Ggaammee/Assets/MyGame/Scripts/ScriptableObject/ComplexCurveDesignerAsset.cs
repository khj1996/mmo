using UnityEngine;

[CreateAssetMenu(fileName = "ComplexCurveDesignerAsset", menuName = "ScriptableObjects/Util/ComplexCurveDesignerAsset")]
public class ComplexCurveDesignerAsset : ScriptableObject
{
    [SerializeField] private ComplexCurveDesigner curveDesigner;

    public ComplexCurveDesigner GetCurveDesigner() => curveDesigner;
}