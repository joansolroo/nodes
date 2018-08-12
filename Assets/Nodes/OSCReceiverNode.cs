using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

using UnityEditor;
using XNodeEditor;


[NodeWidth(300)]
public class OSCReceiverNode : Node
{

    [Node.Input] [SerializeField] public OSC.Receiver Receiver;

    public override void OnAwake()
    {

        initialized = false;
        Init();
    }
    public override void OnStart()
    {
        //Debug.Log("hi");

        initialized = true;
        Link();
    }

    protected override void Init()
    {
        base.Init();
        //Bind(gameObject);
    }
    
    public void Link()
    {
        Receiver = GetInputPort("Receiver").GetInputValue<OSC.Receiver>();
    }
    [HideInInspector] public bool initialized = false;
    

    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        if (to.fieldName == "Receiver")
        {
            Receiver = GetInputPort("Receiver").GetInputValue<OSC.Receiver>();
        }
       
    }
    public override void OnRemoveConnection(NodePort port)
    {
        if (port.fieldName == "Receiver")
        {
            Receiver = null;
        }
    }
}

[CustomNodeEditor(typeof(OSCReceiverNode))]
public class OSCREditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        OSCReceiverNode target = ((OSCReceiverNode)this.target);

        foreach (NodePort port in target.Inputs)
        {

            NodeEditorGUILayout.PortField(port);
        }
        foreach (NodePort port in target.Outputs)
        {

            NodeEditorGUILayout.PortField(port);
        }
        if(target.initialized){
            target.Link();
            
            if (target.Receiver != null)
            {
                foreach(string channel in target.Receiver.values.Keys)
                {
                    EditorGUILayout.TextField(channel, target.Receiver.values[channel].ToString());
                }
            }
        }
    }

    [CustomEditor(typeof(OSCReceiverNode))]
    public class GameObjectNodeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            OSCReceiverNode go = ((OSCReceiverNode)target);
            EditorGUILayout.LabelField(go != null ? go.ToString() : "no obj");

        }
    }
}