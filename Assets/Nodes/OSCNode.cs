using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

using UnityEditor;
using XNodeEditor;


public class OSCNode : Node
{

    [Node.Input][SerializeField] public GameObject gameObject;
    [Node.Output] [SerializeField] public OSC.Connection Connection;
    [Node.Output] [SerializeField] public OSC.Sender Sender;
    [Node.Output] [SerializeField] public OSC.Receiver Receiver;

    public override void OnAwake()
    {
        initialized = false;
        Init();
    }
    public override void OnStart()
    {
        //Debug.Log("hi");
       
    }
    protected override void Init()
    {
        base.Init();
        Bind(gameObject);
    }

    [HideInInspector] public bool initialized = false;
    public void Bind(GameObject go, bool force = false)
    {
        if (force || (go != null && (go != gameObject)))
        {
            //initialized = false;
            gameObject = go;
            name = "OSC ";

            Connection = gameObject.GetComponent<OSC.Connection>();
            Sender = gameObject.GetComponent<OSC.Sender>();
            Receiver = gameObject.GetComponent<OSC.Receiver>();
            initialized = true;
        }
    }
    
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "gameObject") return gameObject;
        if (port.fieldName == "Connection") return Connection;
        if (port.fieldName == "Sender") return Sender;
        if (port.fieldName == "Receiver") return Receiver;
        return null;

       
    }
}

[CustomNodeEditor(typeof(OSCNode))]
public class OSCEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        OSCNode target = ((OSCNode)this.target);
        target.Connection = (OSC.Connection)EditorGUILayout.ObjectField("OSC object", target.Connection, typeof(OSC.Connection), true);
        if (target.Connection != null)
        {
            target.gameObject = target.Connection.gameObject;
        }
        target.Bind(target.gameObject, true);


        foreach (NodePort port in target.Outputs)
        {

            NodeEditorGUILayout.PortField(port);

        }
        /*
        if (target.gameObject == null)
        {

            if (GUILayout.Button("Bind object"))
            {
                target.Bind(Selection.activeGameObject);
            };
        }
        else if (target.initialized)
        {           
            if (GUILayout.Button("Rebind"))
            {
                ((OSCNode)target).Bind(Selection.activeGameObject,true);
            };

        }*/
    }

    [CustomEditor(typeof(GameObjectNode))]
    public class GameObjectNodeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            OSCNode go = ((OSCNode)target);
            EditorGUILayout.LabelField(go != null ? go.ToString() : "no obj");

        }
    }
}