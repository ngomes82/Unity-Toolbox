using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(BoundsAttribute))]
public class BoundsDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // First get the attribute since it contains the range for the slider
        BoundsAttribute bounds = attribute as BoundsAttribute;
        
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        float labelWidht = 50f;
        float sliderWidth = 100f;
        float spacer = 5f;
        var minRect    = new Rect(position.x, position.y, labelWidht, position.height);
        var sliderRect = new Rect(position.x+labelWidht+spacer,  position.y, sliderWidth, position.height);
        var maxRect    = new Rect(position.x+labelWidht+sliderWidth+spacer*2, position.y, labelWidht, position.height);


        var minProperty = property.FindPropertyRelative("min");
        var maxProperty = property.FindPropertyRelative("max");

        if(minProperty == null || maxProperty == null)
        {
            EditorGUI.LabelField(new Rect(position.x, position.y, 200f, position.height), "Use [Bounds] with a Range object");
            EditorGUI.EndProperty();
            return;
        }

        if (minProperty.propertyType == SerializedPropertyType.Float)
        {
            float minVal = minProperty.floatValue;
            float maxVal = maxProperty.floatValue;

            minVal = EditorGUI.FloatField(minRect, minVal);
            EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, bounds.lowerLimit, bounds.upperLimit);
            maxVal = EditorGUI.FloatField(maxRect, maxVal);


            minProperty.floatValue = minVal;
            maxProperty.floatValue = maxVal;
        }
        else if (minProperty.propertyType == SerializedPropertyType.Integer)
        {
            float minVal = minProperty.intValue;
            float maxVal = maxProperty.intValue;

            minVal = EditorGUI.IntField(minRect, Mathf.RoundToInt(minVal) );
            EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, Mathf.RoundToInt(bounds.lowerLimit), Mathf.RoundToInt(bounds.upperLimit));
            maxVal = EditorGUI.IntField(maxRect, Mathf.RoundToInt(maxVal));


            minProperty.intValue = Mathf.RoundToInt(minVal);
            maxProperty.intValue = Mathf.RoundToInt(maxVal);
        }

        property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }
}
