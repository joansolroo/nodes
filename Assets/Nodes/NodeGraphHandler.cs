using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class NodeGraphHandler : MonoBehaviour {

    [SerializeField] NodeGraph graph;
    
    void Awake()
    {
        foreach (Node node in graph.nodes)
        {
            node.OnAwake();
        }

    }
    
    // Use this for initialization
    void Start () {
        foreach(Node node in graph.nodes){
            node.OnStart();
        }

    }
	
	// Update is called once per frame
	void Update () {
        foreach (Node node in graph.nodes)
        {
            node.OnUpdate();
        }
    }
}
