using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tusk : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    // Update is called once per frame
    void Update()
    {
        TuskMovement();
    }

    void TuskMovement()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        TuskBoundaries();
    }

    void TuskBoundaries()
    {
        if (transform.position.x >= 12f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }
}
