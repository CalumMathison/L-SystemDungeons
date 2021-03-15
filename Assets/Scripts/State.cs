using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    //Axiom of generator at state
    public string Axiom;
    //Position of generator at state
    public Vector3 position;
    //Forward Vector of generator at state
    public Vector3 forward;
    //Facing angle of generator at state
    public float angle;
    //Current Node
    public Node currNode;
}
