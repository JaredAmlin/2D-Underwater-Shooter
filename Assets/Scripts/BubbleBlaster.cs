using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBlaster : MonoBehaviour
{
    private float _bubbleThrust;
    private float _bubbleThrustMin = 5f;
    private float _bubbleThrustMax = 20f;

    private float _gravityMin = -0.2f;
    private float _gravityMax = -1f;

    private Rigidbody2D _bubbleRB;

    

    // Start is called before the first frame update
    void Start()
    {
        _bubbleRB = GetComponent<Rigidbody2D>();

        if (_bubbleRB == null)
        {
            Debug.LogError("The Bubble Blaster Rigidbody is NULL");
        }

        _bubbleThrust = Random.Range(_bubbleThrustMin, _bubbleThrustMax);
        _bubbleRB.gravityScale = Random.Range(_gravityMin, _gravityMax);
    }

    private void FixedUpdate()
    {
        _bubbleRB.AddForce(transform.right * _bubbleThrust);
    }

    private void Update()
    {
        if (transform.position.y > 7f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("The Bubble collider hit the enemy");
        }
    }
}
