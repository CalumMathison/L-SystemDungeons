using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public const int MAX_DIM = 10;
    public const int MIN_DIM = 3;
    private Node currNode;
    public int MaxSize;
    public int max_iterations = 5;


    private void Awake()
    {
        //Create initial state
        State newState = new State();
        newState.Axiom = StartingAxiom;
        newState.position = new Vector3(0, 0, 0);
        newState.forward = Vector3.up;
        newState.angle = 0;
        states.Push(newState);
        //Set current state
        currentState = new State();
        currentState = states.Pop();
        Debug.Log("State Setup Complete");

        // Set rules for algorithm
        SetupRules();
        currentAxiom = StartingAxiom;
        // Pass over algorithm
        currentAxiom = GenerateAxiom(currentState.Axiom);
        // Render Algorithm
        CalculatePositions();
        Debug.Log("L-system setup complete");
    }

    void SetupRules()
    {
        State newState = new State();
        newState.Axiom = StartingAxiom;
        newState.position = new Vector3(0, 0, 0);
        newState.forward = Vector3.up;
        newState.angle = 0;
        states.Push(newState);
        //Set current state
        currentState = new State();
        currentState = states.Pop();
        rules.Clear();
        // Create rules
        Rule S = new Rule('S');
        S.IsDrawable = true;
        Rule B = new Rule('B');
        B.IsDrawable = true;
        Rule C = new Rule('C');
        C.IsDrawable = true;
        Rule L = new Rule('L');
        L.IsDrawable = true;
        Rule P = new Rule('P');
        P.IsDrawable = true;
        Rule plus = new Rule('+');
        plus.angle = (float)Random.Range(0, 180);
        plus.IsDrawable = false;
        Rule minus = new Rule('-');
        minus.angle = (float)Random.Range(-180, 0);
        minus.IsDrawable = false;
        Rule sBranch = new Rule('[');
        sBranch.IsDrawable = false;
        Rule eBranch = new Rule(']');
        eBranch.IsDrawable = false;

        //Add rules to dictionary
        rules.Add('S', S);
        rules.Add('B', B);
        rules.Add('C', C);
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
        SetupRules();
        // Start with current axiom
        string result = current;

        // Iterate over axiom and make new axiom with rules
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

        // Check branching rules and ensure axiom branches match up
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

        // Ensure axiom contains a starting room
        if (!result.Contains("S"))
        {
            result = 'S' + result;
        }

        if (!result.Contains("B"))
        {
            result += 'B';
        }
        
        Debug.Log("Axiom Generate. Axiom: " + result);
        GameObject.FindGameObjectsWithTag("AxiomText")[0].GetComponent<Text>().text = "Current Axiom: " + result + "\nIterations: " + Iterations;
        return result;
    }

    void CalculatePositions()
    {
        tPosition = new Vector3(0, 0, 0);
        //Vector3 forward = Vector3.up;
        //Vector3 forward = new Vector3(Random.Range(0, 360), Random.Range(0, 360));
        Vector3[] vs = new Vector3[4];
        vs[0] = Vector3.up;
        vs[1] = Vector3.down;
        vs[2] = Vector3.right;
        vs[3] = Vector3.left;
        Vector3 forward = vs[Random.Range(0, 3)];
        Debug.Log(forward);
        Vector3[] positions = new Vector3[currentAxiom.Length + 1];
        int index = 1;

        // Iterate over current axiom
        foreach (char c in currentAxiom)
        {
            // If axiom character is a rule
            if (rules.ContainsKey(c))
            {
                Rule rule = rules[c];
                if (rule.character == c)
                {
                    // Create new state for open branch
                    if (c == '[')
                    {
                        State newState = new State();
                        newState.Axiom = currentState.Axiom;
                        newState.position = tPosition;
                        newState.forward = forward;
                        if (currNode != null)
                        {
                            newState.currNode = currNode;
                        }
                        else
                        {
                            newState.currNode = null;
                        }
                        states.Push(newState);
                        Debug.Log("New State pushed");
                        positions[index] = tPosition;
                        index++;
                    }
                    else if (c == ']') // Close branch and return to previous state
                    {
                        currentState = states.Pop();
                        currNode = currentState.currNode;
                        tPosition = currentState.position;
                        forward = currentState.forward;
                        positions[index] = tPosition;
                        index++;
                        Debug.Log("Old state loaded");
                    }
                    else if (rule.angle != 0) // Rotate direction for next room
                    {
                        forward += Quaternion.AngleAxis(rule.angle, Vector3.up) * forward * CorridorLength;
                        Debug.LogError(forward);
                    }

                    // Check if rule is drawable and create node to represent it
                    if (rules[c].IsDrawable)
                    {
                        Node newNode = new Node();
                        newNode.positon = new Vector3(tPosition.x + 0.5f, tPosition.y + 0.5f, 0);
                        Debug.Log("Node pos: " + newNode.positon);
                        newNode.width = (int)Random.Range(MIN_DIM, MAX_DIM);
                        newNode.height = (int)Random.Range(MIN_DIM, MAX_DIM);
                        newNode.type = c;
                        newNode.Parent = currNode;
                        currNode = newNode;
                        nodes.Add(newNode);
                        Debug.Log(tPosition);
                        tPosition += forward;
                        positions[index] = tPosition;
                        index++;
                    }
                }
            }
        }

        CheckNodeOverlap();

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
            try
            {
                Reset();
                Debug.ClearDeveloperConsole();
                if (Iterations < max_iterations)
                {
                    Iterations++;
                }
                currentAxiom = GenerateAxiom(currentAxiom);
                CalculatePositions();
                Debug.Log("Iteration Increased");
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && Iterations > 0)
        {
            Reset();
            Debug.ClearDeveloperConsole();
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
        states = new Stack<State>();
        currentState = new State();
        nodes = new List<Node>();
        SetupRules();
    }

    static bool IsOverlapping(Node n, Node j)
    {
        return (Vector2.Distance(n.positon, j.positon) < n.width + j.width + 1
                        || Vector2.Distance(n.positon, j.positon) < n.height + j.height + 1);
    }

    static bool IsOffTopGrid(Node n)
    {
        if (n.positon.x + n.width + 1 > 68 || n.positon.y + n.height + 1 > 68)
            return true;

        return false;
    }

    static bool IsOffBotGrid(Node n)
    {
        if (n.positon.x - 1 < -70 || n.positon.y - 1 < -70)
            return true;

        return false;
    }

    private void CheckNodeOverlap()
    {
        bool redo = false;
        float counter = 0;

        do
        {
            counter += 0.1f;
            redo = false;
            for (int i = 0; i < nodes.Count; ++i)
                for (int j = i + 1; j < nodes.Count; ++j)
                {
                    var node_i = nodes[i];
                    var node_j = nodes[j];

                    if (IsOverlapping(node_i, node_j))
                    {
                        node_i.positon.x = Mathf.Max(0, node_i.positon.x - node_i.width);
                        node_i.positon.y = Mathf.Max(0, node_i.positon.y - node_i.height);
                        node_j.positon.x = Mathf.Min(140 - node_j.width - 1, node_j.positon.x + 1);
                        node_j.positon.y = Mathf.Min(140 - node_j.height - 1, node_j.positon.y + 1);
                        i = nodes.Count;
                        j = nodes.Count;
                        redo = true;
                }
            }
        } while (redo && (counter < 10000));

        if (counter >= 10000)
        {
            Debug.LogError("Operation Failed.");
        }

        //counter = 0;
        //for (int i = 0; i < nodes.Count; i++)
        //{
        //    var node_i = nodes[i];

        //    if (IsOffTopGrid(node_i))
        //    {
        //        node_i.
        //    }

        //    if (IsOffBotGrid(node_i)
        //    {

        //    }
        //}

    }
}
