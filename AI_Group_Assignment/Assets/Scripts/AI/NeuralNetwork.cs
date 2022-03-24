using System;
using System.Collections.Generic;

// TODO: back propagation; it needs to be able to learn
// so rather than random mutations, we should adjust weights intelligently

// TODO: ability to interface with SaveLoadAI class in order to save and
// load the neural network to and from the disk

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    private int[] layers; // int array of the size of each layer
    private float[][] neurons; // [layer][neuron]
    private float[][][] weights; // [layer][neuron][weight]
    public float fitness { get; private set; } // the fitness of the network

    private Random random;

    /// <summary>
    /// Initialize an neural network with random weights
    /// </summary>
    /// <param name="layers">layers to the network</param>
    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        random = new Random(System.DateTime.Now.Millisecond); // always different
        //random = new Random(0); // always the same

        // generate the neurons and weights arrays
        InitNeurons();
        InitWeights();
    }

    /// <summary>
    /// Deep copy constructor
    /// </summary>
    /// <param name="parent">The network to create a deep copy of</param>
    public NeuralNetwork(NeuralNetwork parent)
    {
        this.layers = new int[parent.layers.Length];
        for (int i = 0; i < parent.layers.Length; i++)
        {
            this.layers[i] = parent.layers[i];
        }

        // no need to deep copy neuron array, InitNeurons will always give
        // the same result for any given layers array so we can just generate it again
        InitNeurons();

        // we do need to deep copy weights, but we will call InitWeights to generate
        // the correct array sizes in our weights array beforehand
        InitWeights();
        CopyWeights(parent.weights);
    }

    /// <summary>
    /// Deep copy a weights array
    /// </summary>
    /// <param name="parentWeights">The weights array to create a deep copy of</param>
    private void CopyWeights(float[][][] parentWeights)
    {
        // for each layer
        for (int i = 0; i < weights.Length; i++)
        {
            // for each neuron
            for (int j = 0; j < weights[i].Length; j++)
            {
                // for each weight
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = parentWeights[i][j][k];
                }
            }
        }
    }

    /// <summary>
    /// Create neuron matrix
    /// </summary>
    private void InitNeurons()
    {
        List<float[]> neuronList = new List<float[]>();

        // for each layer
        for (int i = 0; i < layers.Length; i++) 
        {
            neuronList.Add(new float[layers[i]]); // add layer with length to list
        }

        neurons = neuronList.ToArray(); // convert to array
    }

    /// <summary>
    /// Create weight matrix
    /// </summary>
    private void InitWeights()
    {
        List<float[][]> weightList = new List<float[][]>();

        // for each layer that isnt the input layer
        for (int i = 1; i < layers.Length; i++) 
        {
            List<float[]> layerWeightList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1];

            // for each neuron in the layer
            for (int j = 0; j < neurons[i].Length; j++) 
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                // for each connection to the previous layer
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    // set random weights
                    neuronWeights[k] = (float)random.NextDouble() - 0.5f;
                }

                layerWeightList.Add(neuronWeights); // add weights for this neuron to list
            }

            weightList.Add(layerWeightList.ToArray()); // add this layer's neuron weights to list
        }

        weights = weightList.ToArray(); // convert to array
    }

    /// <summary>
    /// Feed forward the neural network with the given inputs
    /// </summary>
    /// <param name="inputs">Inputs to the network</param>
    /// <returns>The network's outputs for the given inputs</returns>
    public float[] FeedForward(float[] inputs)
    {
        // set network's input layer to inputs
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        // for each layer that isn't the input layer
        for (int i = 1; i < layers.Length; i++) // layers.Length - 1   ????
        {
            // for each neuron in the layer
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0.25f; // constant bias

                // for each weight in this neuron
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k]; // sum of all the previous layer's neurons multiplied by their respective weights
                }

                neurons[i][j] = (float)Math.Tanh(value); // hyperbolic tangent activation
            }
        }

        return neurons[neurons.Length - 1]; // return the output layer
    }

    public void Mutate()
    {
        // for each layer
        for (int i = 0; i < weights.Length; i++)
        {
            // for each neuron
            for (int j = 0; j < weights[i].Length; j++)
            {
                // for each weight
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    // mutate
                    float r = (float)random.NextDouble() * 1000f;

                    // flip weight sign
                    if (r <= 2.0f)
                    {
                        weight *= -1.0f;
                    }
                    // randomly reassign between -0.5f and 0.5f
                    else if (r <= 4.0f)
                    {
                        weight = (float)random.NextDouble() - 0.5f;
                    }
                    // random increase by a %
                    else if (r <= 6.0f)
                    {
                        weight *= (float)random.NextDouble() + 1.0f;
                    }
                    // random decrase to a %
                    else if (r <= 8.0f)
                    {
                        weight *= (float)random.NextDouble();
                    }

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    /// <summary>
    /// Increase the network's fitness
    /// </summary>
    /// <param name="value">value to increase by</param>
    public void IncreaseFitnessBy(float value)
    {
        fitness += value;
    }

    /// <summary>
    /// Decrease the network's fitness
    /// </summary>
    /// <param name="value">value to decrease by</param>
    public void DecreaseFitnessBy(float value)
    {
        fitness -= value;
    }

    /// <summary>
    /// Compare this network to another based on their fitness
    /// </summary>
    /// <param name="other">the network to compare to</param>
    /// <returns></returns>
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;

        if (fitness > other.fitness) 
            return 1;
        else if (fitness < other.fitness) 
            return -1;
        else 
            return 0;
    }
}