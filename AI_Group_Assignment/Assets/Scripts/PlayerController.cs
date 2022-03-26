using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float jumpHeight = 10;
    public TextMeshProUGUI scoreText;
    int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpHeight;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Obstacles")
        {
            SceneManager.LoadScene(0);
        }
        else if(collision.gameObject.tag == "Goal")
        {
            if (collision.gameObject.GetComponentInParent<Pipes>().activePipe)
            {
                score++;
                scoreText.text = score.ToString();
                collision.gameObject.GetComponentInParent<Pipes>().activePipe = false;
                collision.gameObject.GetComponentInParent<Pipes>().pipePassed = true;
            }
        }
    }
}
