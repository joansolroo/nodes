using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using UnityEditor;
using XNodeEditor;

[System.Serializable]
public class GameObjectNode : Node
{

    [Node.Output]
    [SerializeField] public GameObject gameObject;
    [Node.Output]
    [SerializeField] public Transform Transform;

    /*
    [HideInInspector] [SerializeField] public Component[] components;
    [HideInInspector] [SerializeField] public NodePort[] output;
    [SerializeField] public bool[] isOutput;
    */
    public override void OnAwake()
    {
        initialized = false;
    }
    public override void OnStart()
    {
        //Debug.Log("hi");
        Init();
    }
    protected override void Init()
    {
        base.Init();
        Bind(gameObject);
    }

    public bool initialized = false;
    public void Bind(GameObject go, bool force = false)
    {

        if (force || (go != null && (go != gameObject)))
        {
            //initialized = false;
            gameObject = go;
            name = "GameObject: " + go.name;
            Transform = go.transform;
            /*ClearInstancePorts();
            components = gameObject.GetComponents<Component>();
            output = new NodePort[components.Length];
            isOutput = new bool[components.Length];
            int idx = 0;
            foreach (Component c in components)
            {
                NodePort p = AddInstanceOutput(c.GetType(), ConnectionType.Multiple, c.GetType().Name);
                isOutput[idx] = true;
                output[idx++] = p;
            }*/
            initialized = true;
        }
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "gameObject") return gameObject;
        if (port.fieldName == "Transform") return Transform;
        /*
        for (int idx = 0; idx < output.Length; ++idx)
        {
            if (port == output[idx])
            {
                return components[idx];
            }
        }*/
        return null;
    }
}

[CustomNodeEditor(typeof(GameObjectNode))]
public class GONEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        GameObjectNode target = ((GameObjectNode)this.target);
        if (target.gameObject == null)
        {

            if (GUILayout.Button("Bind object"))
            {
                ((GameObjectNode)target).Bind(Selection.activeGameObject);
            };
        }
        else if (target.initialized)
        {

            foreach (NodePort port in target.Outputs)
            {

                NodeEditorGUILayout.PortField(port);

            }/*
            for (int idx = 0; idx< ((GameObjectNode)target).output.Length; ++idx)
            {
                if (((GameObjectNode)target).isOutput[idx])
                {
                    NodeEditorGUILayout.PortField(target.output[idx]);
                }
            }*/
            if (GUILayout.Button("Rebind"))
            {
                ((GameObjectNode)target).Bind(Selection.activeGameObject);
            };

        }
        else
        {
            ((GameObjectNode)target).Bind(target.gameObject, true);
        }
    }

    [CustomEditor(typeof(GameObjectNode))]
    public class GameObjectNodeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GameObjectNode go = ((GameObjectNode)target);
            EditorGUILayout.LabelField(go != null ? go.ToString() : "no obj");

            /*
            if (go != null)
            {
                EditorGUILayout.LabelField("Object:", go.name);
                if (go.components != null) for (int idx = 0; idx < go.components.Length; ++idx)
                    {
                        go.isOutput[idx] = EditorGUILayout.Toggle(go.components[idx].GetType().Name + " as output", go.isOutput[idx]);
                    }
            }*/

        }
    }
}