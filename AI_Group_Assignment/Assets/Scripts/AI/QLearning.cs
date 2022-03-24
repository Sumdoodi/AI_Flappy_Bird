using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Move
{
    public Vector4 previousState;
    public int previousAction;
    public Vector4 state;
}

public class QLearning
{
    public bool train;

    [Header("Parameters")]
    public float discount = 0.95f;
    public float alpha = 0.7f;
    public Dictionary<int, int> reward = new Dictionary<int, int>() { 
        { 0, 0 },       // no reward for flapping
        { 1, -1000 }    // negative reward for dying
    };
    public float alphaDecay = 0.00003f;

    [Header("States")]
    public int episode = 0;
    public int prevAction = 0;
    public Vector4 prevState = Vector4.zero;
    public List<Move> moves = new List<Move>();
    
}
