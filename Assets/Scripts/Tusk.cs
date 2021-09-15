using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tusk : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    //0 = Tusk, 1 = Piranha_Chompers
    [SerializeField] private int _projectileID;

    private Player _player;

    // Update is called once per frame
    void Update()
    {
        TuskMovement();
    }

    void TuskMovement()
    {
        if (_projectileID == 0)
        {
            transform.Translate(Vector3.right * _speed * Time.deltaTime);

            TuskBoundaries();
        }

        else if (_projectileID == 1)
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);

            PiranhaChompersBoundaries();
        }

        else if (_projectileID == 2)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            BlowfishSpineBoundaries();
        }
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

    void PiranhaChompersBoundaries()
    {
        if (transform.position.x <= -12f)
        {
            Destroy(this.gameObject);
        }
    }

    void BlowfishSpineBoundaries()
    {
        if (transform.position.x <= -12f || transform.position.x >= 12f || transform.position.y >= 7f || transform.position.y <= -7f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_projectileID == 1 || _projectileID == 2)
        {
            if (other.tag == "Player")
            {
                _player = other.transform.GetComponent<Player>();

                if (_player != null)
                {
                    _player.Damage();

                    Destroy(this.gameObject);
                }
            }
        }
    }
}
