﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule
{
    //List of usable characters in randomisation. Insures that only on start and end room occurs.
    public string[] nonTerminalChars = { "C", "P", "L", "+", "-", "[" };
    public string[] nonBranchingChars = { "C", "P", "L", "+", "-" };
    //Character representation of rule
    public char character;
    //Replacement character of rule
    public string replacementCharacter;
    //Forward angle of rule (default = 0)
    public float angle = 0;
    public bool IsDrawable
    {
        get
        {
            if (character == '[' ||
                character == '+' ||
                character == '-' ||
                character == ']')
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public Rule(char c)
    {
        character = c;
    }

    //Append character with new string
    public string Replace()
    {
        string rep = "";

        rep += character + nonTerminalChars[(int)Random.Range(0, nonTerminalChars.Length)];

        //Make sure branching completes, required for stack
        if (rep.Contains("["))
        {
            int count = (int)Random.Range(1, 3);
            for (int i = 0; i < count; i++)
            {
                rep += nonBranchingChars[(int)Random.Range(0, nonBranchingChars.Length)];
            }

            rep += "]";
        }
        else if (character == 'B')
        {
            rep = "B";
        }

        return rep;
    }

}
