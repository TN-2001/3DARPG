using System;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
# endif

public class SubclassSelectorAttribute : PropertyAttribute
{
    public Type Type { get; private set; }

    public SubclassSelectorAttribute(Type type)
    {
        Type = type;
    }
}

# if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SubclassSelectorAttribute selectorAttribute = (SubclassSelectorAttribute)attribute;

        // 現在の値を入手
        MonoBehaviour currentValue = property.objectReferenceValue as MonoBehaviour;
        // 描画し、新規の値を入手
        MonoBehaviour newValue = (MonoBehaviour)EditorGUI.ObjectField(position, label, currentValue, typeof(MonoBehaviour), true);
        // 値を反映
        if (newValue == null) {
            property.objectReferenceValue = null;
        } else {
            var component = newValue.GetComponent(selectorAttribute.Type);
            if (component != null) {
                property.objectReferenceValue = component as MonoBehaviour;
            }
        }
    }
}
# endif