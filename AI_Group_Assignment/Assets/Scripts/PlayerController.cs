using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Options")]
    public float jumpHeight = 10;

    private void Start()
    {
        //AIData t = new AIData(1);
        //SaveLoadAI.Save(t);
        //Debug.Log("Data Saved");
        //t = SaveLoadAI.Load();
        //Debug.Log("Data Loaded");
        //Debug.Log(t.test);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public void Jump()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpHeight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.gameObject.tag == "Obstacles")
        //{
        SceneManager.LoadScene(0);
        //}
    }
}
