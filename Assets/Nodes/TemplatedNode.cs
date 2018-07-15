using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using UnityEditor;
using XNodeEditor;
using System;
// SimpleNode.cs

public static class SerializedPropertyExtensions
{
    public static object GetValue(this SerializedProperty property)
    {
        System.Type parentType = property.serializedObject.targetObject.GetType();
        System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);
        return fi.GetValue(property.serializedObject.targetObject);
    }
    public static void SetValue(this SerializedProperty property, object value)
    {
        System.Type parentType = property.serializedObject.targetObject.GetType();
        System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);//this FieldInfo contains the type.
        fi.SetValue(property.serializedObject.targetObject, value);
    }
}

[AttributeUsage(AttributeTargets.Field)]
public abstract class NodeAttribute : Attribute
{
    [AttributeUsage(AttributeTargets.Field)]
    public class Input : NodeAttribute
    {

    }
    [AttributeUsage(AttributeTargets.Field)]
    public class Output : NodeAttribute
    {

    }
}

public abstract class WrapperNode : Node
{
    [HideInInspector] [SerializeField] public Component _component;
    [HideInInspector] [SerializeField] public SerializedObject serializedObject;
    [HideInInspector] [SerializeField] public SerializedProperty[] properties;

    [HideInInspector] [SerializeField] public NodePort objectInput;
    [HideInInspector] [SerializeField] public NodePort[] __input;
    [HideInInspector] [SerializeField] public NodePort[] __output;
    [HideInInspector] [SerializeField] public bool[] __isInput;
    [HideInInspector] [SerializeField] public bool[] __isShown;
    [HideInInspector] [SerializeField] public bool[] __isOutput;
}

public abstract class ComponentNode<T> : WrapperNode where T : Component
{
    [Input(ShowBackingValue.Unconnected,ConnectionType.Override)]
    [HideInInspector] [SerializeField] public T component;

    protected override void Init()
    {
        base.Init();
        objectInput = GetInputPort("component");
        name = typeof(T).Name;
    }

    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        if (to.fieldName == objectInput.fieldName)
        {
            component = (T)to.GetInputValue();
            _component = component;
            Bind();
        }
    }
    public override void OnRemoveConnection(NodePort port)
    {
        if (port.fieldName == objectInput.fieldName)
        {
            component = default(T);
            ClearInstancePorts();
        }
    }
    public override object GetValue(NodePort port)
    {
        for (int idx = 0; idx < __output.Length; ++idx)
        {
            if (port == __output[idx])
            {

                return GetValue(properties[idx]);
            }

        }
        return null;
    }

    public static System.Type GetType(SerializedPropertyType serializedType)
    {
        switch (serializedType)
        {
            case SerializedPropertyType.Vector3: return typeof(Vector3);
            case SerializedPropertyType.Vector2: return typeof(Vector2);
            case SerializedPropertyType.Vector4: return typeof(Vector4);
            case SerializedPropertyType.Quaternion: return typeof(Quaternion);
            default: return typeof(object);
        }

    }
    public static object GetValue(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Vector3: return property.vector3Value;
            case SerializedPropertyType.Vector2: return property.vector2Value;
            case SerializedPropertyType.Vector4: return property.vector4Value;
            case SerializedPropertyType.Quaternion: return property.quaternionValue;
            default: return property.GetValue();
        }

    }

    public void Bind()
    {
        if (component == null)
        {
            return;
        }
        this.name = component.GetType().Name;
        serializedObject = new SerializedObject(component);
        SerializedProperty prop = serializedObject.GetIterator();

        int count = 0;
        if (prop.NextVisible(true))
        {
            do
            {
                //if (prop.GetType() == typeof(Vector3))
                {
                    if (prop.name == "m_Script")
                    {
                        continue;//TODO: Fix HACK
                    }
                    count++;
                }
            }
            while (prop.NextVisible(false));
        }
        __input = new NodePort[count];
        __output = new NodePort[count];
        __isInput = new bool[count];
        __isOutput = new bool[count];
        __isShown = new bool[count];
        properties = new SerializedProperty[count];
        int idx = 0;
        prop = serializedObject.GetIterator();
        if (prop.NextVisible(true))
        {
            do
            {
                if (prop.name == "m_Script")
                {
                    continue;//TODO: Fix HACK
                }

                __input[idx] = null;
                __output[idx] = null;

                __isInput[idx] = false;
                __isOutput[idx] = false;
                __isShown[idx] = true;
                System.Reflection.PropertyInfo info = component.GetType().GetProperty(prop.name);
                if (info != null)
                {
                    object[] attributes = info.GetCustomAttributes(true);
                    foreach (object a in attributes)
                    {
                        NodeAttribute attribute = a as NodeAttribute;
                        if (attribute != null)
                        {
                            if (attribute as NodeAttribute.Input != null)
                            {
                                __isInput[idx] = true;
                            }
                            if (attribute as NodeAttribute.Output != null)
                            {
                                __isOutput[idx] = true;
                            }
                        }
                    }
                }

                properties[idx] = serializedObject.FindProperty(prop.propertyPath);
                __input[idx] = AddInstanceInput(prop.GetType(), ConnectionType.Override, "-> " + prop.name);
                __output[idx] = AddInstanceOutput(prop.GetType(), ConnectionType.Multiple, prop.name + " ->");
                ++idx;
            }
            while (prop.NextVisible(false));
        }
    }
}

public class TemplatedNode : ComponentNode<Component> { }

[CustomNodeEditor(typeof(WrapperNode))]
public class CustomNhjkEditor : ComponentNEditor<Component>
{
}

public class ComponentNEditor<T> : NodeEditor where T : Component
{
    public override void OnBodyGUI()
    {
        if (target == null)
        {
            return;
        }
        NodeEditorGUILayout.PortField(((ComponentNode<T>)target).objectInput);
        ComponentNode<T> node = ((ComponentNode<T>)target);
        if (Event.current.type != EventType.MouseUp)
        {
            if (node.component != null && node.serializedObject != null)
            {
                for (int idx = 0; idx < node.properties.Length; idx++)
                {
                    if (node.__isInput[idx])
                    {
                        NodeEditorGUILayout.PortField(node.__input[idx]);
                    }
                    if (node.__isShown[idx] && ((node.__isInput[idx] || !node.__input[idx].IsConnected) || (node.__isOutput[idx] || !node.__output[idx].IsConnected)))
                    {
                        EditorGUILayout.PropertyField(node.properties[idx], true);
                    }
                    if (node.__isOutput[idx])
                    {
                       
                        NodeEditorGUILayout.PortField(node.__output[idx]);
                    }
                }
                node.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
[CustomEditor(typeof(WrapperNode), true)]
public class ComponentNodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        WrapperNode component = ((WrapperNode)target);
        if (component != null)
        {
            EditorGUILayout.LabelField("Object:", component.name);
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Property","\tShow\t Input\t Output");
                EditorGUILayout.EndHorizontal();
            }

            if (component.properties != null) for (int idx = 0; idx < component.properties.Length; ++idx)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(component.properties[idx].name);
                    component.__isShown[idx] = EditorGUILayout.Toggle(component.__isShown[idx]);
                    component.__isInput[idx] = EditorGUILayout.Toggle(component.__isInput[idx]);
                    component.__isOutput[idx] = EditorGUILayout.Toggle(component.__isOutput[idx]);
                    EditorGUILayout.EndHorizontal();
                }
        }

    }
}
/*
public class SimpleNode : Node
{
    public int a;
    public int b;
    [Output] public int sum;

    public override object GetValue(NodePort port)
    {
        return a + b;
    }
}
[CustomNodeEditor(typeof(SimpleNode))]
public class SimpleNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        SerializedObject serializedObject = new SerializedObject(target);

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("a"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("b"));
        UnityEditor.EditorGUILayout.LabelField("The value is " + target.GetValue(null));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("sum"));
    }
}*/
