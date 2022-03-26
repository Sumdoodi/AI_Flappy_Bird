using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEvents : MonoBehaviour
{
    public static UnityEvent BirdDied;

    // Start is called before the first frame update
    void Start()
    {
        if (BirdDied == null)
            BirdDied = new UnityEvent();
    }
}
