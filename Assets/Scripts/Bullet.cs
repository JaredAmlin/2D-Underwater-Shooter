using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BulletMovement();

        BulletBoundaries();
    }

    void BulletMovement()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    void BulletBoundaries()
    {
        if (transform.position.x >= 12f)
        {
            Destroy(this.gameObject);
        }
    }
}
