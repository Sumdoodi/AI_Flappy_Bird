using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float lifeTime;
    public float queueTime = 1.5f;
    private float time = 0;
    public GameObject obstacle;

    public bool spawnedFirstObstacle = false;

    public float height;

    private List<GameObject> obstacles = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        time = queueTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (time > queueTime)
        {
            GameObject go = Instantiate(obstacle, transform);
            go.transform.position = transform.position + new Vector3(0, Random.Range(-height, height), 0);
            obstacles.Add(go);

            time = 0;

            Destroy(go, lifeTime);

            if (!spawnedFirstObstacle) spawnedFirstObstacle = true;
        }

        if (obstacles.Count > 0)
        {
            // the first item in the list should always be a pipe that is 
            // ahead of the player, we aren't interested in the others
            if (obstacles[0].GetComponent<Pipes>().isAhead == false)
            {
                obstacles.RemoveAt(0);
            }
        }

        time += Time.deltaTime;
    }

    public Vector3 GetCurrentObstacle()
    {
        return obstacles[0].transform.position;
    }

    public void Clean()
    {
        obstacles.Clear();
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        time = queueTime;
        spawnedFirstObstacle = false;
    }
}
