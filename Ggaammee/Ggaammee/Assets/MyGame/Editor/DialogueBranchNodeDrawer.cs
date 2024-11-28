using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueBranchNode), true)]
public class DialogueBranchNodeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 현재 타입이 표시될 드롭다운
        if (property.managedReferenceValue == null)
        {
            // 타입 선택 드롭다운
            if (GUI.Button(position, "Select Node Type"))
            {
                GenericMenu menu = new GenericMenu();
                foreach (var type in TypeCache.GetTypesDerivedFrom<DialogueBranchNode>())
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        property.serializedObject.Update();
                        var newNode = Activator.CreateInstance(type);
                        property.managedReferenceValue = newNode;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }
        }
        else
        {
            // 선택된 타입에 따른 필드 표시
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

}