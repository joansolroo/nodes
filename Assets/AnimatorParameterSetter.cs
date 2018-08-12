using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParameterSetter : MonoBehaviour {

    [SerializeField] public Animator animator;
    [SerializeField] public AnimatorControllerParameter parameter;
    [SerializeField] public string parameterName;
    [SerializeField] public OSC.IChannel input;
    [SerializeField] public float value;

	// Use this for initialization
	void Start () {
        FindParameter();

    }
	
	// Update is called once per frame
	void Update () {

        
        Set();
	}

    public void FindParameter()
    {
        if(animator == null)
        {
            return;
        }
        foreach(AnimatorControllerParameter p in animator.parameters)
        {
            if(p.name == parameterName)
            {
                parameter = p;
                return;
            }
        }
    }

    public void Set()
    {
        if(parameter== null)
        {
            return;
        }

        if (input != null)
        {
            value = input.GetValue();
        }
        switch (parameter.type)
        {
                case (AnimatorControllerParameterType.Int):
                    animator.SetInteger(parameter.nameHash, (int)value);
            break;
                case (AnimatorControllerParameterType.Float):
                    animator.SetFloat(parameter.nameHash, value);
            break;
                case (AnimatorControllerParameterType.Bool):
                    animator.SetBool(parameter.nameHash, value > 0);
            break;
                case (AnimatorControllerParameterType.Trigger):
                    if (value > 0)
            {
                animator.SetTrigger(parameter.nameHash);
            }
            break;
        }
    }
}
