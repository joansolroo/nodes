using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using UnityEditor;
using XNodeEditor;

public class GameObjectNode : WrapperNode
{

    [SerializeField] public GameObject gameObject;
    [HideInInspector] [SerializeField] public Component[] components;
    [HideInInspector] public NodePort[] output;
    [SerializeField] public bool[] isOutput;

    protected override void Init()
    {
        base.Init();
        Bind(gameObject);
    }

    public void Bind(GameObject go)
    {
        gameObject = go;
        if (gameObject != null)
        {
            name = "GameObject: "+go.name;
            ClearInstancePorts();
            components = gameObject.GetComponents<Component>();
            output = new NodePort[components.Length];
            isOutput = new bool[components.Length];
            int idx = 0;
            foreach (Component c in components)
            {
                NodePort p = AddInstanceOutput(c.GetType(), ConnectionType.Multiple, c.GetType().Name);
                isOutput[idx] = true;
                output[idx++] = p;
            }
        }
    }

    public override object GetValue(NodePort port)
    {
        for (int idx = 0; idx < output.Length; ++idx)
        {
            if (port == output[idx])
            {
                return components[idx];
            }
        }
        return null;
    }
}

[CustomNodeEditor(typeof(GameObjectNode))]
public class GONEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        if (((GameObjectNode)target).gameObject == null)
        {

            if (GUILayout.Button("Build Object"))
            {
                ((GameObjectNode)target).Bind(Selection.activeGameObject);
            };
        }
        else
        {
            //SerializedObject serializedObject = new SerializedObject(target);
            //NodeEditorGUILayout.PortField(((GameObjectNode)target).input);
            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("a"));
            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("b"));
            //UnityEditor.EditorGUILayout.LabelField("The value is " + target.GetValue(null));
            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("sum"));
            
            for (int idx = 0; idx< ((GameObjectNode)target).output.Length; ++idx)
            {
                if (((GameObjectNode)target).isOutput[idx])
                {
                    NodeEditorGUILayout.PortField(((GameObjectNode)target).output[idx]);
                }
            }

        }
    }
    [CustomEditor(typeof(GameObjectNode))]
    public class GameObjectNodeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GameObjectNode go = ((GameObjectNode)target);
            if (go != null)
            {
                EditorGUILayout.LabelField("Object:", go.name);
                if(go.components!=null)for (int idx = 0; idx < go.components.Length; ++idx)
                { 
                    go.isOutput[idx] = EditorGUILayout.Toggle(go.components[idx].GetType().Name + " as output", go.isOutput[idx]);
                }
            }

        }
    }
}