using System;
using System.Collections.Generic;


// TODO: ability to interface with SaveLoadAI class in order to save and
// load the neural network to and from the disk

// TODO: take stuff from this old class that isnt yet in the new class such as
// IComparable and fitness, etc.

//public class NeuralNetwork : IComparable<NeuralNetwork>
//{
//    private int[] layers; // int array of the size of each layer
//    private float[][] neurons; // [layer][neuron]
//    private float[][][] weights; // [layer][neuron][weight]
//    public float fitness { get; private set; } // the fitness of the network

//    private Random random;

//    /// <summary>
//    /// Initialize an neural network with random weights
//    /// </summary>
//    /// <param name="layers">layers to the network</param>
//    public NeuralNetwork(int[] layers)
//    {
//        this.layers = new int[layers.Length];
//        for (int i = 0; i < layers.Length; i++)
//        {
//            this.layers[i] = layers[i];
//        }

//        random = new Random(System.DateTime.Now.Millisecond); // always different
//        //random = new Random(0); // always the same

//        // generate the neurons and weights arrays
//        InitNeurons();
//        InitWeights();
//    }

//    /// <summary>
//    /// Deep copy constructor
//    /// </summary>
//    /// <param name="parent">The network to create a deep copy of</param>
//    public NeuralNetwork(NeuralNetwork parent)
//    {
//        this.layers = new int[parent.layers.Length];
//        for (int i = 0; i < parent.layers.Length; i++)
//        {
//            this.layers[i] = parent.layers[i];
//        }

//        // no need to deep copy neuron array, InitNeurons will always give
//        // the same result for any given layers array so we can just generate it again
//        InitNeurons();

//        // we do need to deep copy weights, but we will call InitWeights to generate
//        // the correct array sizes in our weights array beforehand
//        InitWeights();
//        CopyWeights(parent.weights);
//    }

//    /// <summary>
//    /// Deep copy a weights array
//    /// </summary>
//    /// <param name="parentWeights">The weights array to create a deep copy of</param>
//    private void CopyWeights(float[][][] parentWeights)
//    {
//        // for each layer
//        for (int i = 0; i < weights.Length; i++)
//        {
//            // for each neuron
//            for (int j = 0; j < weights[i].Length; j++)
//            {
//                // for each weight
//                for (int k = 0; k < weights[i][j].Length; k++)
//                {
//                    weights[i][j][k] = parentWeights[i][j][k];
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// Create neuron matrix
//    /// </summary>
//    private void InitNeurons()
//    {
//        List<float[]> neuronList = new List<float[]>();

//        // for each layer
//        for (int i = 0; i < layers.Length; i++) 
//        {
//            neuronList.Add(new float[layers[i]]); // add layer with length to list
//        }

//        neurons = neuronList.ToArray(); // convert to array
//    }

//    /// <summary>
//    /// Create weight matrix
//    /// </summary>
//    private void InitWeights()
//    {
//        List<float[][]> weightList = new List<float[][]>();

//        // for each layer that isnt the input layer
//        for (int i = 1; i < layers.Length; i++) 
//        {
//            List<float[]> layerWeightList = new List<float[]>();
//            int neuronsInPreviousLayer = layers[i - 1];

//            // for each neuron in the layer
//            for (int j = 0; j < neurons[i].Length; j++) 
//            {
//                float[] neuronWeights = new float[neuronsInPreviousLayer];

//                // for each connection to the previous layer
//                for (int k = 0; k < neuronsInPreviousLayer; k++)
//                {
//                    // set random weights
//                    neuronWeights[k] = (float)random.NextDouble() - 0.5f;
//                }

//                layerWeightList.Add(neuronWeights); // add weights for this neuron to list
//            }

//            weightList.Add(layerWeightList.ToArray()); // add this layer's neuron weights to list
//        }

//        weights = weightList.ToArray(); // convert to array
//    }

//    /// <summary>
//    /// Feed forward the neural network with the given inputs
//    /// </summary>
//    /// <param name="inputs">Inputs to the network</param>
//    /// <returns>The network's outputs for the given inputs</returns>
//    public float[] FeedForward(float[] inputs)
//    {
//        // set network's input layer to inputs
//        for (int i = 0; i < inputs.Length; i++)
//        {
//            neurons[0][i] = inputs[i];
//        }

//        // for each layer that isn't the input layer
//        for (int i = 1; i < layers.Length; i++) // layers.Length - 1   ????
//        {
//            // for each neuron in the layer
//            for (int j = 0; j < neurons[i].Length; j++)
//            {
//                float value = 0.25f; // constant bias

//                // for each weight in this neuron
//                for (int k = 0; k < neurons[i - 1].Length; k++)
//                {
//                    value += weights[i - 1][j][k] * neurons[i - 1][k]; // sum of all the previous layer's neurons multiplied by their respective weights
//                }

//                neurons[i][j] = (float)Math.Tanh(value); // hyperbolic tangent activation
//            }
//        }

//        return neurons[neurons.Length - 1]; // return the output layer
//    }

//    public void Mutate()
//    {
//        // for each layer
//        for (int i = 0; i < weights.Length; i++)
//        {
//            // for each neuron
//            for (int j = 0; j < weights[i].Length; j++)
//            {
//                // for each weight
//                for (int k = 0; k < weights[i][j].Length; k++)
//                {
//                    float weight = weights[i][j][k];

//                    // mutate
//                    float r = (float)random.NextDouble() * 1000f;

//                    // flip weight sign
//                    if (r <= 2.0f)
//                    {
//                        weight *= -1.0f;
//                    }
//                    // randomly reassign between -0.5f and 0.5f
//                    else if (r <= 4.0f)
//                    {
//                        weight = (float)random.NextDouble() - 0.5f;
//                    }
//                    // random increase by a %
//                    else if (r <= 6.0f)
//                    {
//                        weight *= (float)random.NextDouble() + 1.0f;
//                    }
//                    // random decrase to a %
//                    else if (r <= 8.0f)
//                    {
//                        weight *= (float)random.NextDouble();
//                    }

//                    weights[i][j][k] = weight;
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// Increase the network's fitness
//    /// </summary>
//    /// <param name="value">value to increase by</param>
//    public void IncreaseFitnessBy(float value)
//    {
//        fitness += value;
//    }

//    /// <summary>
//    /// Decrease the network's fitness
//    /// </summary>
//    /// <param name="value">value to decrease by</param>
//    public void DecreaseFitnessBy(float value)
//    {
//        fitness -= value;
//    }

//    /// <summary>
//    /// Compare this network to another based on their fitness
//    /// </summary>
//    /// <param name="other">the network to compare to</param>
//    /// <returns></returns>
//    public int CompareTo(NeuralNetwork other)
//    {
//        if (other == null) return 1;

//        if (fitness > other.fitness) 
//            return 1;
//        else if (fitness < other.fitness) 
//            return -1;
//        else 
//            return 0;
//    }
//}

namespace AI
{
    public class NeuralNetwork
    {
        int[] layer;
        Layer[] layers;
        float learningRate;

        public NeuralNetwork(int[] layer, float learningRate)
        {
            this.learningRate = learningRate;
            this.layer = new int[layer.Length];
            for (int i = 0; i < layer.Length; i++)
                this.layer[i] = layer[i];

            layers = new Layer[layer.Length - 1]; // ignore input layer

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(layer[i], layer[i + 1]);
            }
        }

        public float[] FeedForward(float[] inputs)
        {
            layers[0].FeedForward(inputs); // pass inputs to the first layer

            // for each layer, skipping the input layer
            for (int i = 1; i < layers.Length; i++)
            {
                layers[i].FeedForward(layers[i - 1].outputs); // feed output from previous layer
            }

            return layers[layers.Length - 1].outputs; // return output from last layer
        }

        public void BackPropagation(float[] expected)
        {
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                if (i == layers.Length - 1)
                {
                    layers[i].BackPropagationOutput(expected);
                }
                else
                {
                    layers[i].BackPropagationHidden(layers[i + 1].gamma, layers[i + 1].weights);
                }
            }

            // for each layer, skipping the input layer
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].UpdateWeights(learningRate);
            }
        }
    }

    public class Layer
    {
        int numberOfInputs; // number of neurons in previous layer
        int numberOfOutputs; // number of neurons in this layer

        public float[] outputs;
        public float[] inputs;
        public float[,] weights;
        public float[,] deltaWeights;
        public float[] gamma;
        public float[] error;

        public static Random random = new Random(System.DateTime.Now.Millisecond); // always different
        
        public Layer(int numInputs, int numOutputs)
        {
            this.numberOfInputs = numInputs;
            this.numberOfOutputs = numOutputs;

            outputs = new float[numberOfOutputs];
            inputs = new float[numberOfInputs];
            weights = new float[numberOfOutputs, numberOfInputs];
            deltaWeights = new float[numberOfOutputs, numberOfInputs];
            gamma = new float[numberOfOutputs];
            error = new float[numberOfOutputs];

            InitializeWeights();
        }

        public void InitializeWeights()
        {
            // for each neuron in this layer
            for (int i = 0; i < numberOfOutputs; i++)
            {
                // for each neuron in the previous layer
                for (int j = 0; j < numberOfInputs; j++)
                {
                    // set the weight randomly between -0.5f and 0.5f
                    weights[i, j] = (float)random.NextDouble() - 0.5f;
                }
            }
        }

        public float[] FeedForward(float[] input)
        {
            this.inputs = input;

            // for each neuron in this layer
            for (int i = 0; i < numberOfOutputs; i++)
            {
                outputs[i] = 0;

                // for each neuron in the previous layer
                for (int j = 0; j < numberOfInputs; j++)
                {
                    outputs[i] += inputs[j] * weights[i, j];
                }

                outputs[i] = (float)Math.Tanh(outputs[i]);
            }

            return outputs;
        }

        /// <summary>
        /// Calculate the derivative of a TanH value
        /// </summary>
        /// <param name="value">the value to calculate a derrivative for</param>
        /// <returns></returns>
        public float TanHPrime(float value)
        {
            return 1 - (value * value);
        }

        // TODO: can these for loops be collapsed?
        public void BackPropagationOutput(float[] expected)
        {
            for (int i = 0; i < numberOfOutputs; i++)
                error[i] = outputs[i] - expected[i];

            for (int i = 0; i < numberOfOutputs; i++)
                gamma[i] = error[i] * TanHPrime(outputs[i]);

            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    deltaWeights[i, j] = gamma[i] * inputs[j];
                }
            }

        }

        public void BackPropagationHidden(float[] gammaForward, float[,] forwardWeights)
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                gamma[i] = 0;

                for (int j = 0; j < gammaForward.Length; j++)
                {
                    gamma[i] += gammaForward[j] * forwardWeights[j, i];
                }

                gamma[i] *= TanHPrime(outputs[i]);
            }

            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    deltaWeights[i, j] = gamma[i] * inputs[j];
                }
            }
        }

        public void UpdateWeights(float learningRate)
        {
            // for each neuron in this layer
            for (int i = 0; i < numberOfOutputs; i++)
            {
                // for each neuron in the previous layer
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weights[i, j] -= deltaWeights[i, j] * learningRate;
                }
            }
        }
    }
}
