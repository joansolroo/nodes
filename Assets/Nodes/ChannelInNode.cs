using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

using UnityEditor;
using XNodeEditor;


[NodeWidth(300)]
public class ChannelInNode : Node
{

    [Node.Input] [SerializeField] public OSC.Receiver Receiver;
    [Node.Output] [SerializeField] public OSC.ChannelIn channel;
    [SerializeField] public string address;

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
    public void Bind(OSC.Receiver receiver, string address)
    {
        Receiver = receiver;
        this.address = address;
        OSC.ChannelIn[] channels = receiver.GetComponents<OSC.ChannelIn>();
        bool found = false;
        foreach (OSC.ChannelIn channel2 in channels)
        {
            if (channel2.address == address)
            {
                channel = channel2;
                found = true;
                break;
            }
        }
        if (!found)
        {
            if(channel == null || channel.gameObject!= receiver.gameObject) 
            {
                channel = receiver.gameObject.AddComponent<OSC.ChannelIn>();
            }
            channel.address = this.address;
        }
        channel.receiver = receiver;

    }
    public void Link()
    {
        Receiver = GetInputPort("Receiver").GetInputValue<OSC.Receiver>();
    }

    [HideInInspector] public bool initialized = true;


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
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "channel") return channel;
        
        return null;


    }
}

[CustomNodeEditor(typeof(ChannelInNode))]
public class ChannelInNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        ChannelInNode target = ((ChannelInNode)this.target);

        foreach (NodePort port in target.Inputs)
        {
            NodeEditorGUILayout.PortField(port);
        }
        foreach (NodePort port in target.Outputs)
        {
            NodeEditorGUILayout.PortField(port);
        }
        if (target.initialized)
        {
            target.Link();
            if (target.Receiver != null)
            {
                target.address = EditorGUILayout.TextField("Address:", target.address);

                target.Bind(target.Receiver, target.address);
                if (target.channel != null)
                {

                    EditorGUILayout.LabelField("Value:", target.channel.value.ToString());
                }
            }
        }
    }
}