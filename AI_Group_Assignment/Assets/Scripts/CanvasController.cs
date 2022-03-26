using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public Text generationText;

    public void UpdateGeneration(int current, int total)
    {
        generationText.text = $"{current}/{total}";
    }
}
