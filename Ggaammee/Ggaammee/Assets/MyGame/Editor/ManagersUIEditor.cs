#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Managers))]
public class ManagersUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 렌더링
        base.OnInspectorGUI();

        // ShopUI 컴포넌트를 가져옵니다.
        Managers shopUI = (Managers)target;

        // 버튼을 생성합니다.
        if (GUILayout.Button("Set Test Data"))
        {
            // RefreshShop 메서드를 호출합니다.
            shopUI.SetTestData();
        }
    }
}
#endif