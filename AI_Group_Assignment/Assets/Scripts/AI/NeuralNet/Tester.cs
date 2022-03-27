using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class Tester : MonoBehaviour
{
    [Header("Network Parameters")]
    public float learningRate;

    [Header("Testing Parameters")]
    public int itterations;

    private DateTime startTime;
    private DateTime endTime;

    // Start is called before the first frame update
    void Start()
    {
        NeuralNetwork network = new NeuralNetwork(new int[] { 3, 25, 25, 1 }, learningRate);

        startTime = DateTime.Now;
        Debug.Log($"Started training at {startTime.ToString("G")}");

        for (int i = 0; i < itterations; i++)
        {
            network.FeedForward(new float[] { 0, 0, 0 });
            network.BackPropagation(new float[] { 0 }, 1.0f);

            network.FeedForward(new float[] { 0, 0, 1 });
            network.BackPropagation(new float[] { 1 }, 1.0f);

            network.FeedForward(new float[] { 0, 1, 0 });
            network.BackPropagation(new float[] { 1 }, 1.0f);

            network.FeedForward(new float[] { 0, 1, 1 });
            network.BackPropagation(new float[] { 0 }, 1.0f);

            network.FeedForward(new float[] { 1, 0, 0 });
            network.BackPropagation(new float[] { 1 }, 1.0f);

            network.FeedForward(new float[] { 1, 0, 1 });
            network.BackPropagation(new float[] { 0 }, 1.0f);

            network.FeedForward(new float[] { 1, 1, 0 });
            network.BackPropagation(new float[] { 0 }, 1.0f);

            network.FeedForward(new float[] { 1, 1, 1 });
            network.BackPropagation(new float[] { 1 }, 1.0f);
        }

        endTime = DateTime.Now;
        Debug.Log($"Ended training at {endTime.ToString("G")}");
        TimeSpan totalTime = endTime - startTime;
        Debug.Log($"Training took: {totalTime.ToString("G")}");

        Debug.Log("Training results:");
        Debug.Log($"0,0,0: {network.FeedForward(new float[] { 0, 0, 0 })[0]} [0]");
        Debug.Log($"0,0,1: {network.FeedForward(new float[] { 0, 0, 1 })[0]} [1]");
        Debug.Log($"0,1,0: {network.FeedForward(new float[] { 0, 1, 0 })[0]} [1]");
        Debug.Log($"0,1,1: {network.FeedForward(new float[] { 0, 1, 1 })[0]} [0]");
        Debug.Log($"1,0,0: {network.FeedForward(new float[] { 1, 0, 0 })[0]} [1]");
        Debug.Log($"1,0,1: {network.FeedForward(new float[] { 1, 0, 1 })[0]} [0]");
        Debug.Log($"1,1,0: {network.FeedForward(new float[] { 1, 1, 0 })[0]} [0]");
        Debug.Log($"1,1,1: {network.FeedForward(new float[] { 1, 1, 1 })[0]} [1]");
    }
}
