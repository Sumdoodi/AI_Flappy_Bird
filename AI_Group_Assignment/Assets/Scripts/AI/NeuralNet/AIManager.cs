using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

// TODO: Game states, wait for player decision on each frame,
// ask network for its output based on inputs, back propagate
// based on player decision

public enum TrainingType { GENERATIONS, BACK_PROPAGATION }

/// <summary>
/// This is the class used to handle the AI's training.
/// It will handle:
///  - Generations
///  - Fitness scores
///  - Interfacing with the game
/// </summary>
public class AIManager : MonoBehaviour
{
    public TrainingType trainingType;

    [Header("Generation Training Parameters")]
    public int generations;
    public int networksPerGeneration;
    public float mutationStrength;

    [Header("Back Propagation Training Parameters")]
    public float learningRate;
    public uint targetDataPoints;

    [Header("Network Parameters")]
    public float activation;
    public int[] hiddenLayers;

    [Header("Bird Prefabs")]
    public GameObject generationPrefab;
    public GameObject backPropagationPrefab;

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

    private bool isPlayerControlled = true;
    private uint dataPointsCollected = 0;
    private NeuralNetwork bpNetwork;
    private BirdController birdController;
    private bool died = false;
    private bool netDidJump = false;
    private float[] netRawOut;

    // Start is called before the first frame update
    void Start()
    {
        List<int> netLayers = new List<int>(hiddenLayers);
        netLayers.Insert(0, 5);
        netLayers.Add(1);

        // populate the list of networks if training by generation
        // spawn the playable bird prefab if training by back propagation
        switch (trainingType)
        {
            case TrainingType.GENERATIONS:
                for (int i = 0; i < networksPerGeneration; i++)
                {
                    NeuralNetwork current = new NeuralNetwork(netLayers.ToArray(), mutationStrength);
                    generation.Add(current);
                    birds[current] = NewBird();
                    canvasController.UpdateGeneration(currentGeneration, generations);
                    birdsAlive = networksPerGeneration;
                }
                break;
            case TrainingType.BACK_PROPAGATION:
                // spawn bird
                birdController = Instantiate(backPropagationPrefab, new Vector3(-2.14f, 1.55f, -0.265f), Quaternion.identity).GetComponent<BirdController>();
                bpNetwork = new NeuralNetwork(netLayers.ToArray(), learningRate);
                canvasController.UpdateBackPropagation(isPlayerControlled, 0.0d, false, 0.0d);
                break;
        }

        canvasController.SetTrainingType(trainingType);

        CustomEvents.Instance.BirdDied.AddListener(OnBirdDied);
    }

    // Update is called once per frame
    void Update()
    {
        switch (trainingType)
        {
            case TrainingType.GENERATIONS:
                {
                    if (!spawner.spawnedFirstObstacle)
                    {
                        //Debug.Log("waiting");
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
                                try
                                {
                                    AIController temp;
                                    birds.TryGetValue(generation[i], out temp);
                                }
                                catch
                                {
                                    Debug.Log("generation[i] not found failed");
                                }

                                // skip if the AI died
                                if (birds[generation[i]].gameObject.activeSelf == false) continue;

                                // ask network for its decision
                                float[] output = generation[i].FeedForward(new float[] {
                        birds[generation[i]].transform.position.y,                      // the bird's height
                        birds[generation[i]].GetComponent<Rigidbody2D>().velocity.y,    // the bird's vertical velocity
                        pipeXPos - birds[generation[i]].transform.position.x,           // distance to the next pipe
                        pipeUpperHeight,                                                // next pipe's upper bound
                        pipeLowerHeight                                                 // next pipe's lower bound
                    });

                                Debug.Log(output[0]);
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
                            NeuralNetwork temp = generation[0];
                            generation.Clear();
                            generation.Add(temp);

                            birds[generation[0]] = NewBird();

                            // fill the rest of the list with mutated versions of the top performer
                            for (int i = 1; i < networksPerGeneration; i++)
                            {
                                generation.Add(new NeuralNetwork(generation[0], 0.25f));
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
                    break;
                }
            case TrainingType.BACK_PROPAGATION:
                {
                    if (!spawner.spawnedFirstObstacle) return;

                    // reset the game if bird died
                    if (died)
                    {
                        spawner.Clean();
                        birdController.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                        birdController.transform.position = new Vector3(-2.14f, 1.55f, -0.265f);
                        died = false;
                        return; // skip this loop
                    }

                    // check for tab key, toggle isPlayerControlled
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        isPlayerControlled = !isPlayerControlled;
                        UpdateUI();
                    }

                    UpdateInputs();

                    if (isPlayerControlled) // teach the network based on the player's decisions
                    {
                        bool playerDidJump = false;
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            birdController.Jump();
                            playerDidJump = true;
                        }

                        // teach the network
                        netRawOut = bpNetwork.FeedForward(new float[]
                        {
                            birdController.transform.position.y,
                            birdController.gameObject.GetComponent<Rigidbody2D>().velocity.y,
                            pipeXPos - birdController.transform.position.x,
                            pipeUpperHeight,
                            pipeLowerHeight
                        });
                        Debug.Log(netRawOut[0]);
                        if (netRawOut[0] >= activation)
                            netDidJump = true;

                        if (playerDidJump)
                        {
                            bpNetwork.BackPropagation(new float[] { 0.5f });
                        }
                        else
                        {
                            bpNetwork.BackPropagation(new float[] { -0.5f });
                        }

                        dataPointsCollected += 1;

                        UpdateUI();
                    }
                    else // allow the network to play
                    {
                        float decision = bpNetwork.FeedForward(new float[]
                        {
                            birdController.transform.position.y,
                            birdController.gameObject.GetComponent<Rigidbody2D>().velocity.y,
                            pipeXPos - birdController.transform.position.x,
                            pipeUpperHeight,
                            pipeLowerHeight
                        })[0];

                        if (decision > activation)
                        {
                            birdController.Jump();
                        }
                    }
                    break;
                }
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
        return Instantiate(generationPrefab, new Vector3(-2.14f, 1.55f, -0.265f), Quaternion.identity, transform).GetComponent<AIController>();
    }

    private void UpdateUI()
    {
        double percentComplete = (double)dataPointsCollected / targetDataPoints * 100.0d;
        percentComplete = System.Math.Round(percentComplete, 1); // round to one decimal places
        double netRawOutRound = System.Math.Round(netRawOut[0], 2); // round to two decimal places
        canvasController.UpdateBackPropagation(isPlayerControlled, percentComplete, netDidJump, netRawOutRound);
        netDidJump = false;
    }

    public void OnBirdDied()
    {
        switch (trainingType)
        {
            case TrainingType.GENERATIONS:
                birdsAlive--;
                break;
            case TrainingType.BACK_PROPAGATION:
                died = true;
                break;
        }
    }
}
