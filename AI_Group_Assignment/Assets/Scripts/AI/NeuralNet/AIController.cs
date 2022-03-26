using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIController : MonoBehaviour
{
    [Header("Options")]
    public float jumpHeight = 10;

    public void Jump()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpHeight;
    }

    // TODO: we dont want to reload scene when one AI dies
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Obstacles")
        {
            this.gameObject.SetActive(false);
            //gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            CustomEvents.Instance.BirdDied.Invoke();
        }
    }
}
