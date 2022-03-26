using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEvents : MonoBehaviour
{
    public UnityEvent BirdDied;

    private static CustomEvents _instance;

    public static CustomEvents Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        if (BirdDied == null)
            BirdDied = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
    }
}
