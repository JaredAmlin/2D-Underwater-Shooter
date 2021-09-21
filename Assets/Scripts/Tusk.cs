using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tusk : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    //0 = Tusk, 1 = Piranha_Chompers
    [SerializeField] private int _projectileID;

    private Player _player;

    private Transform _target;

    private Vector3 _direction;

    private void Start()
    {
        if (_projectileID == 3)
        {
            _player = GameObject.Find("Player").GetComponent<Player>();

            if (_player == null)
            {
                Debug.Log("The player target is NULL for the Star projectile");
            }

            _target = _player.GetComponent<Transform>();

            if (_target == null)
            {
                Debug.Log("The Transform on the Player is NULL for the Star Projectile");
            }

            if (_target != null)
            {
                _direction = (_target.position - this.transform.position).normalized;
            }
        }
    }

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

        else if (_projectileID == 3)
        {
            //move towards the location of the player when fired. Not updated or homing. 
            transform.position += _direction * _speed * Time.deltaTime;

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
        if (_projectileID == 1 || _projectileID == 2 || _projectileID == 3)
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
