using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

// TODO: update inputs, 

/// <summary>
/// This is the class used to handle the AI's training.
/// It will handle:
///  - Generations
///  - Fitness scores
///  - Interfacing with the game
/// </summary>
public class AIManager : MonoBehaviour
{

    [Header("Training Parameters")]
    public int generations;
    public int networksPerGeneration;

    [Header("Network Parameters")]
    public int[] hiddenLayers;
    public float learningRate;
    public float activation;

    [Header("Bird Prefab")]
    public GameObject prefab;

    [Header("References")]
    public Spawner spawner;
    public CanvasController canvasController;

    // Inputs
    private float pipeXPos = 0.0f;
    private float pipeUpperHeight = 0.0f;
    private float pipeLowerHeight = 0.0f;

    private List<NeuralNetwork> generation = new List<NeuralNetwork>();
    private Dictionary<NeuralNetwork, AIController> birds = new Dictionary<NeuralNetwork, AIController>();

    private int currentGeneration = 1;
    private int birdsAlive;
    private float currentScore = 0.0f;
    private float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        List<int> netLayers = new List<int>(hiddenLayers);
        netLayers.Insert(0, 5);
        netLayers.Add(1);

        for (int i = 0; i < networksPerGeneration; i++)
        {
            NeuralNetwork current = new NeuralNetwork(netLayers.ToArray(), learningRate);
            generation.Add(current);
            birds[current] = NewBird();
        }

        canvasController.UpdateGeneration(currentGeneration, generations);
        birdsAlive = networksPerGeneration;

        CustomEvents.BirdDied.AddListener(OnBirdDied);
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawner.spawnedFirstObstacle)
        {
            Debug.Log("waiting");
            return;
        }

        if (currentGeneration <= generations)
        {
            // if there is still at least one bird alive
            if (birdsAlive > 0)
            {
                UpdateInputs();

                for (int i = 0; i < generation.Count; i++)
                {
                    // skip if the AI died
                    if (birds[generation[i]].dead) continue;

                    // ask network for its decision
                    float[] output = generation[i].FeedForward(new float[] {
                        birds[generation[i]].transform.position.y,                      // the bird's height
                        birds[generation[i]].GetComponent<Rigidbody2D>().velocity.y,    // the bird's vertical velocity
                        pipeXPos - birds[generation[i]].transform.position.x,           // distance to the next pipe
                        pipeUpperHeight,                                                // next pipe's upper bound
                        pipeLowerHeight                                                 // next pipe's lower bound
                    });

                    // make the bird just if the network's output is greater than the activation
                    if (output[0] >= activation)
                    {
                        birds[generation[i]].Jump();
                        Debug.Log($"Bird {i} jumped");
                    }

                    // increate the networks fitness by the amount of distance it covered without dying this frame
                    generation[i].IncreaseFitnessBy(Time.deltaTime * speed);
                }
            }
            else // end of generation
            {
                // reset dictionary
                birds = new Dictionary<NeuralNetwork, AIController>();

                // sort generation by fitness
                generation.Sort();

                // reset generation list while keeping top performer
                List<NeuralNetwork> nextGen = new List<NeuralNetwork>() { generation[0] };
                generation = nextGen;

                // fill the rest of the list with mutated versions of the top performer
                for (int i = 1; i < networksPerGeneration; i++)
                {
                    generation[i] = new NeuralNetwork(generation[0], 0.25f);
                    birds[generation[i]] = NewBird();
                }

                // increment current generation
                currentGeneration++;

                // update generation UI text
                canvasController.UpdateGeneration(currentGeneration, generations);

                // reset current score
                currentScore = 0;

                // reset birds alive
                birdsAlive = networksPerGeneration;

                // clear all pipes
                spawner.Clean();
            }
        }
        else // we've gone through all generations
        {
            // TODO: stop the game, show highest score
            // maybe sort generation by fitness and save top performer to disk
            Debug.Log("DONE");
        }
    }

    public void UpdateInputs()
    {
        Vector3 nextObstaclePosition = spawner.GetCurrentObstacle();
        pipeXPos = nextObstaclePosition.x;
        pipeUpperHeight = nextObstaclePosition.y + 1.3f;
        pipeLowerHeight = nextObstaclePosition.y - 1.3f;
    }

    private AIController NewBird()
    {
        // hard coded to the same starting position as the game
        return Instantiate(prefab, new Vector3(-2.14f, 1.55f, -0.265f), Quaternion.identity, transform).GetComponent<AIController>();
    }

    public void OnBirdDied()
    {
        birdsAlive--;
    }
}
