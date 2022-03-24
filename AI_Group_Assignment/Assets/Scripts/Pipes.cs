using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipes : MonoBehaviour
{
    public float speed = 10; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position -= new Vector3(Time.deltaTime * speed, 0, 0);

        if(this.gameObject.transform.position.x < -5)
        {
            Destroy(this);
        }
    }
}
