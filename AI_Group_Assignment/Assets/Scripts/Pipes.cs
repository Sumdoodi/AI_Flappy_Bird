using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipes : MonoBehaviour
{
    public float speed = 10;
    public bool isAhead = true;
    private float playerXPos = -2.14f; // hardcoded from game

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position -= new Vector3(Time.deltaTime * speed, 0, 0);

        if (isAhead && (transform.position.x + 0.5f) < playerXPos)
        {
            isAhead = false;
        }

        //if (this.gameObject.transform.position.x < -5)
        //{
        //    Destroy(this);
        //}
    }
}
