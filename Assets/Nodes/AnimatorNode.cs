using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XNode;

using UnityEditor;
using XNodeEditor;

[NodeWidth(300)]
public class AnimatorNode : Node
{

    [Node.Input] [SerializeField] public GameObject gameObject;
    [Node.Output] [SerializeField] public Animator Animator;

    public override void OnAwake()
    {
        initialized = false;
        Init();
    }
    public override void OnStart()
    {
        //Debug.Log("hi");
    }
    public override void OnUpdate()
    {
        initialized = true;
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
            name = "Animator: "+go.name;
            
            Animator = gameObject.GetComponent<Animator>();
            initialized = true;
        }
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "gameObject") return gameObject;
        if (port.fieldName == "Animator") return Animator;

        return null;


    }
}

[CustomNodeEditor(typeof(AnimatorNode))]
public class AnimatorEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        AnimatorNode target = ((AnimatorNode)this.target);

        //EditorGUILayout.LabelField("GameObject:", target.gameObject!=null?target.gameObject.name:"null");
        target.Animator = (UnityEngine.Animator)EditorGUILayout.ObjectField("Animator", target.Animator, typeof(UnityEngine.Animator), true);
        
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
        }*/
        if (target.gameObject != null && target.initialized)
        {

            if (target.Animator != null)
            {
                target.Animator.runtimeAnimatorController = (UnityEngine.RuntimeAnimatorController)EditorGUILayout.ObjectField("Controller", target.Animator.runtimeAnimatorController, typeof(UnityEngine.RuntimeAnimatorController), true);

                for (int p = 0; p < target.Animator.parameters.Length; ++p)
                {
                    AnimatorControllerParameter parameter = target.Animator.parameters[p];
                    if (parameter.name != null && parameter.name.Length > 0)
                    {
                        if (parameter.type == AnimatorControllerParameterType.Bool || parameter.type == AnimatorControllerParameterType.Trigger)
                        {
                            target.Animator.SetBool(parameter.nameHash, EditorGUILayout.ToggleLeft(parameter.name, target.Animator.GetBool(parameter.nameHash)));
                        }
                        else if (parameter.type == AnimatorControllerParameterType.Int)
                        {
                            target.Animator.SetInteger(parameter.nameHash, EditorGUILayout.IntField(parameter.name, target.Animator.GetInteger(parameter.nameHash)));
                        }
                        else if (parameter.type == AnimatorControllerParameterType.Float)
                        {
                            target.Animator.SetFloat(parameter.nameHash, EditorGUILayout.FloatField(parameter.name, target.Animator.GetFloat(parameter.nameHash)));
                        }
                    }
                }
            }

            /*
            if (GUILayout.Button("Rebind"))
            {
                target.Bind(Selection.activeGameObject,true);
            };*/

        }
        else
        {

            target.Bind(target.gameObject,true);
        }

    }

    [CustomEditor(typeof(GameObjectNode))]
    public class GameObjectNodeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            AnimatorNode go = ((AnimatorNode)target);
            EditorGUILayout.LabelField(go != null ? go.ToString() : "no obj");

        }
    }
}