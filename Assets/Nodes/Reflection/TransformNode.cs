using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using UnityEditor;
using XNodeEditor;


public class TransformNode : ComponentNode<Transform>{}

[CustomNodeEditor(typeof(TransformNode))] 
public class TNEditor : ComponentNEditor<Transform> {}