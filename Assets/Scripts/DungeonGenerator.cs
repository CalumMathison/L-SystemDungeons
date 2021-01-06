using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    //Holds data and references for all rules. Allows for multiple use of rules
    public Dictionary<char, Rule> rules = new Dictionary<char, Rule>();
    //Holds all data for each stack. Allows for backtracking
    private Stack<State> states = new Stack<State>();
    //Holds data for current stack
    private State currentState;
    //Holds data and references for all nodes;
    public List<Node> nodes = new List<Node>();

    //Data for axiom at 0 iterations
    public string StartingAxiom;
    //Data for axiom at 0> iterations
    private string currentAxiom;

    //Count of iterations
    public int Iterations;
    //Data for length of connections between nodes ||| Move to node class? |||
    public float CorridorLength;
    private Vector3 tPosition = new Vector3(0,0,0);

    public int MaxSize;

    private void Awake()
    {
        State newState = new State();
        newState.Axiom = StartingAxiom;
        newState.position = new Vector3(0, 0, 0);
        newState.forward = Vector3.up;
        newState.angle = 0;
        states.Push(newState);
        currentState = new State();
        currentState = states.Peek();
        Debug.Log("State Setup Complete");

        SetupRules();
        currentAxiom = StartingAxiom;
        currentAxiom = GenerateAxiom(currentState.Axiom);
        CalculatePositions();
        Debug.Log("L-system setup complete");
    }

    void SetupRules()
    {
        Rule S = new Rule('S');
        Rule B = new Rule('B');
        Rule C = new Rule('C');
        Rule L = new Rule('L');
        Rule P = new Rule('P');
        Rule plus = new Rule('+');
        plus.angle = 45;
        Rule minus = new Rule('-');
        minus.angle = -45;
        Rule sBranch = new Rule('[');
        Rule eBranch = new Rule(']');

        rules.Add('S', S);
        rules.Add('B', B);
        rules.Add('C', L);
        rules.Add('L', L);
        rules.Add('P', P);
        rules.Add('+', plus);
        rules.Add('-', minus);
        rules.Add('[', sBranch);
        rules.Add(']', eBranch);
        Debug.Log("Rule Setup Complete");
    }

    string GenerateAxiom(string current)
    {
        string result = current;

        for (int i = 0; i < Iterations; i++)
        {
            string s = "";

            foreach (char c in result)
            {
                if (!rules.ContainsKey(c))
                {
                    s += c;
                    continue;
                }

                s += rules[c].Replace();

                if (s.Length > MaxSize)
                {
                    s += 'B';
                    break;
                }                
            }

            result = s;
        }

        int a = 0, b = 0;
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] == '[')
            {
                a++;
            }
            if (result[i] == ']')
            {
                b++;
            }
        }

        if (a > b)
        {
            int i = a - b;
            for (int j = 0; j < i; j++)
            {
                result += ']';
            }
        }
        else if (b > a)
        {
            int i = b - a;
            for (int j = 0; j < i; j++)
            {
                result = '[' + result;
            }
        }

        if (!result.Contains("S"))
        {
            result = 'S' + result;
        }
        
        Debug.Log("Axiom Generate. Axiom: " + result);
        return result;
    }

    void CalculatePositions()
    {
        tPosition = Vector3.zero;
        Vector3 forward = Vector3.up;
        Vector3[] positions = new Vector3[currentAxiom.Length + 1];
        int index = 1;
        Vector3 pos = Vector3.zero;

        foreach (char c in currentAxiom)
        {
            if (rules.ContainsKey(c))
            {
                Rule rule = rules[c];
                if (rule.character == c)
                {
                    if (c == '[')
                    {
                        State newState = new State();
                        newState.Axiom = currentState.Axiom;
                        newState.position = tPosition;
                        newState.forward = forward;
                        states.Push(newState);
                        positions[index] = tPosition;
                        index++;
                    }
                    else if (c == ']')
                    {
                        currentState = states.Pop();
                        tPosition = currentState.position;
                        forward = currentState.forward;
                        positions[index] = tPosition;
                        index++;
                    }
                    else if (rule.angle != 0)
                    {
                        forward = Quaternion.AngleAxis(rule.angle, Vector3.forward) * forward;
                    }

                    if (rules[c].IsDrawable)
                    {
                        Node newNode = new Node();
                        newNode.positon = new Vector3(tPosition.x + 0.5f, tPosition.y + 0.5f, 0);
                        nodes.Add(newNode);
                        tPosition += forward * 5;
                        positions[index] = tPosition;
                        index++;
                    }
                }
            }
        }

        Debug.Log("Node Generation Complete. Node Count: " + nodes.Count);

        foreach (Node n in nodes)
        {
            Debug.Log(n.positon);
        }

        GameObject.FindGameObjectWithTag("Level").GetComponent<Level>().Setup(nodes);

        Debug.Log("Node Visualisation Complete");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Reset();
            Iterations++;
            currentAxiom = GenerateAxiom(currentAxiom);
            CalculatePositions();
            Debug.Log("Iteration Increased");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && Iterations > 0)
        {
            Reset();
            Iterations--;
            currentAxiom = GenerateAxiom(currentAxiom);
            CalculatePositions();
            Debug.Log("Iteration Decreased");
        }
    }

    private void Reset()
    {
        nodes.Clear();
        states.Clear();
    }
}
