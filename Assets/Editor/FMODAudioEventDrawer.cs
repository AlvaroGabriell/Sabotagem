#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FMODAudioEventAttribute))]
public sealed class FMODAudioEventDrawer : PropertyDrawer
{
    private struct Option
    {
        public string Label;
        public string Value;

        public Option(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }

    private static List<Option> cachedOptions;

    private static List<Option> Options => cachedOptions ??= BuildOptions();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        var options = Options;
        if(options.Count == 0)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        int currentIndex = Mathf.Max(0, options.FindIndex(obj => obj.Value == property.stringValue));
        string[] labels = options.Select(obj => obj.Label).ToArray();

        EditorGUI.BeginProperty(position, label, property);
        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, labels);
        if(newIndex >= 0 && newIndex < options.Count)
        {
            property.stringValue = options[newIndex].Value;
        }
        EditorGUI.EndProperty();
    }

    private static List<Option> BuildOptions()
    {
        var list = new List<Option>
        {
            new Option("<None>", "")
        };

        Type audioEventsType = FindTypeByName("AudioEvents");
        if(audioEventsType == null) return list;

        CollectOptions(audioEventsType, "", list);
        return list;
    }

    private static void CollectOptions(Type type, string prefix, List<Option> list)
    {
        foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (!nested.IsClass)
                continue;

            string nestedPrefix = string.IsNullOrEmpty(prefix) ? nested.Name : $"{prefix}/{nested.Name}";
            CollectOptions(nested, nestedPrefix, list);
        }

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (!field.IsLiteral || field.FieldType != typeof(string))
                continue;

            var value = field.GetRawConstantValue() as string;
            if (string.IsNullOrWhiteSpace(value))
                continue;

            if (!value.StartsWith("event:/", StringComparison.OrdinalIgnoreCase))
                continue;

            string label = string.IsNullOrEmpty(prefix) ? field.Name : $"{prefix}/{field.Name}";
            list.Add(new Option(label, value));
        }
    }

    private static Type FindTypeByName(string typeName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
            }

            foreach (var type in types)
            {
                if (type.Name == typeName)
                    return type;
            }
        }

        return null;
    }
}
#endif