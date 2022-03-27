using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject generationText;
    public GameObject controlledBy;
    public GameObject completion;
    public GameObject networkOutput;
    public GameObject networkOutputRaw;
    public GameObject networkAvgOutput;

    private Color netOutInitColour;
    private TrainingType trainingType;

    public Color positiveColour;
    public Color negativeColour;

    private void Start()
    {
        netOutInitColour = networkOutput.transform.GetChild(1).GetComponent<Text>().color;
        networkOutput.transform.GetChild(1).GetComponent<Text>().color = new Color(netOutInitColour.r, netOutInitColour.g, netOutInitColour.b, 0.0f);
    }

    public void SetTrainingType(TrainingType type)
    {
        trainingType = type;
        switch (type)
        {
            case TrainingType.GENERATIONS:
                generationText.gameObject.SetActive(true);
                controlledBy.gameObject.SetActive(false);
                completion.gameObject.SetActive(false);
                networkOutput.gameObject.SetActive(false);
                networkOutputRaw.gameObject.SetActive(false);
                networkAvgOutput.gameObject.SetActive(true);
                break;
            case TrainingType.BACK_PROPAGATION:
                generationText.gameObject.SetActive(false);
                controlledBy.gameObject.SetActive(true);
                completion.gameObject.SetActive(true);
                networkOutput.gameObject.SetActive(true);
                networkOutputRaw.gameObject.SetActive(true);
                networkAvgOutput.gameObject.SetActive(false);
                break;
        }
    }

    public void UpdateGeneration(int current, int total, double netAvg)
    {
        generationText.transform.GetChild(1).GetComponent<Text>().text = $"{current}/{total}";
        networkAvgOutput.transform.GetChild(1).GetComponent<Text>().text = $"{netAvg}";

        if (netAvg > 0)
            networkAvgOutput.transform.GetChild(1).GetComponent<Text>().color = positiveColour;
        else if (netAvg < 0)
            networkAvgOutput.transform.GetChild(1).GetComponent<Text>().color = negativeColour;
        else
            networkAvgOutput.transform.GetChild(1).GetComponent<Text>().color = Color.white;
    }

    public void UpdateBackPropagation(bool controlledByPlayer, double percentComplete, bool netDidJump, double netRawOut)
    {
        if (controlledByPlayer)
        {
            controlledBy.transform.GetChild(1).GetComponent<Text>().text = "Player";
        }
        else
        {
            controlledBy.transform.GetChild(1).GetComponent<Text>().text = "AI";
        }

        if (percentComplete % 1 == 0)
        {
            completion.transform.GetChild(1).GetComponent<Text>().text = $"{percentComplete}.0%";
        }
        else
        {
            completion.transform.GetChild(1).GetComponent<Text>().text = $"{percentComplete}%";
        }

        if (netDidJump)
        {
            networkOutput.transform.GetChild(1).GetComponent<Text>().color = netOutInitColour;
        }

        networkOutputRaw.transform.GetChild(1).GetComponent<Text>().text = $"{netRawOut}";
    }

    private void Update()
    {
        if (trainingType == TrainingType.GENERATIONS) return;

        if (networkOutput.transform.GetChild(1).GetComponent<Text>().color.a > 0)
        {
            networkOutput.transform.GetChild(1).GetComponent<Text>().color = new Color(
                netOutInitColour.r,
                netOutInitColour.g,
                netOutInitColour.b,
                networkOutput.transform.GetChild(1).GetComponent<Text>().color.a - Time.deltaTime
                );
        }
    }
}
