using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject generationText;
    public GameObject controlledBy;
    public GameObject completion;

    public void SetTrainingType(TrainingType type)
    {
        switch (type)
        {
            case TrainingType.GENERATIONS:
                generationText.gameObject.SetActive(true);
                controlledBy.gameObject.SetActive(false);
                completion.gameObject.SetActive(false);
                break;
            case TrainingType.BACK_PROPAGATION:
                generationText.gameObject.SetActive(false);
                controlledBy.gameObject.SetActive(true);
                completion.gameObject.SetActive(true);
                break;
        }
    }

    public void UpdateGeneration(int current, int total)
    {
        generationText.transform.GetChild(1).GetComponent<Text>().text = $"{current}/{total}";
    }

    public void UpdateBackPropagation(bool controlledByPlayer, double percentComplete)
    {
        if (controlledByPlayer)
        {
            controlledBy.transform.GetChild(1).GetComponent<Text>().text = "Player";
        }
        else
        {
            controlledBy.transform.GetChild(1).GetComponent<Text>().text = "AI";
        }

        completion.transform.GetChild(1).GetComponent<Text>().text = $"{percentComplete}%";
    }
}
