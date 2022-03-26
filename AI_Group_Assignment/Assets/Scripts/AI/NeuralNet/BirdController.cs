using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [Header("Options")]
    public float jumpHeight = 5;

    public void Jump()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpHeight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacles")
        {
            CustomEvents.Instance.BirdDied.Invoke();
        }
    }
}
