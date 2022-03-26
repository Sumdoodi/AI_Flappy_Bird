using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public float queueTime = 1.5f;
    private float time = 0;
    public GameObject obstacle;

    public List<GameObject> pipeList;
    GameObject activePipe;

    public float height;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Get rid of old pipe
        if (pipeList != null)
        {
            if (pipeList.Count > 0)
            {
                if (pipeList[0].GetComponent<Pipes>().pipePassed)
                {
                    GameObject temp = pipeList[0];
                    pipeList.Remove(temp);
                    Destroy(temp);
                }
            }
        }

        if (time > queueTime)
        {

            GameObject go = Instantiate(obstacle);
            go.transform.position = transform.position + new Vector3(0, Random.Range(-height, height), 0);

            pipeList.Add(go); //add to list

            if (activePipe != null)
            {
                activePipe = pipeList[0];
                Debug.Log(activePipe.GetComponentInParent<Transform>().position);
                activePipe.GetComponent<Pipes>().activePipe = true;
            }
            else
            {
                activePipe = pipeList[0];
                Debug.Log(activePipe.GetComponentInParent<Transform>().position);
                activePipe.GetComponent<Pipes>().activePipe = true;
            }

            time = 0;

            Destroy(go, 10);
        }

        time += Time.deltaTime;
    }
}
