using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

using UnityEditor;
using XNodeEditor;


[NodeWidth(300)]
public class AnimatorParameterNode : Node
{

    [Node.Input] [SerializeField] public OSC.IChannel channel;
    [Node.Input] [SerializeField] public Animator animator;
    [SerializeField] public AnimatorParameterSetter parameter;
    [SerializeField] public string parameterName;
    [SerializeField] public int selected =0;
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
    public void Bind(Animator anim, string parameterName)
    {
        animator = anim;
        this.parameterName = parameterName;
        AnimatorParameterSetter[] setters = animator.GetComponents<AnimatorParameterSetter>();
        bool found = false;
        foreach (AnimatorParameterSetter parameter2 in setters)
        {
           
            if (parameter2.parameterName == parameterName)
            {
                parameter = parameter2;
                found = true;
                break;
            }
        }
        if (!found)
        {
            foreach (AnimatorControllerParameter p in animator.parameters)
            {
                if (p.name == parameterName)
                {
                    parameter = animator.gameObject.AddComponent<AnimatorParameterSetter>();
                    parameter.parameterName = parameterName;
                    break;
                }
            }
        }
        parameter.input = channel;
    }
    public void Link()
    {
        channel = GetInputPort("channel").GetInputValue<OSC.IChannel>();
        animator = GetInputPort("animator").GetInputValue<Animator>();
        parameter.input = channel;
    }
    [HideInInspector] public bool initialized = true;


    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        if (to.fieldName == "animator")
        {
            animator = GetInputPort("animator").GetInputValue<Animator>();
        }
        if (to.fieldName == "channel")
        {
            channel = GetInputPort("channel").GetInputValue<OSC.IChannel>();
            parameter.input = channel;
        }
    }
    public override void OnRemoveConnection(NodePort port)
    {
        if (port.fieldName == "animator")
        {
            animator = null;
        }
        if (port.fieldName == "channel")
        {
            channel = null;
        }


    }
}

[CustomNodeEditor(typeof(AnimatorParameterNode))]
public class AnimatorParameterEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        AnimatorParameterNode target = ((AnimatorParameterNode)this.target);
        target.parameter = (AnimatorParameterSetter)EditorGUILayout.ObjectField("Parameter", target.parameter, typeof(AnimatorParameterSetter), true);
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
            if (target.animator != null)
            {
                string[] options = new string[target.animator.parameterCount];
                for (int p = 0; p < target.animator.parameterCount; ++p) options[p] = target.animator.parameters[p].name;

                target.selected = EditorGUILayout.Popup("Label", target.selected, options);
                //target.parameterName = EditorGUILayout.TextField("Parameter name:", options[target.selected]);
                target.Bind(target.animator, target.parameterName);
                if (target.parameter != null)
                {

                    EditorGUILayout.LabelField("Value:", target.parameter.value.ToString());
                }
            }
        }
    }
}