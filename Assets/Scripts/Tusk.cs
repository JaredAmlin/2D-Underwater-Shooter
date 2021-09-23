using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tusk : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    //0 = Tusk, 1 = Piranha_Chompers
    [SerializeField] private int _projectileID;

    private bool _hasTargetPowerup = false;

    private Player _player;

    private Transform _target;

    private Transform _targetPowerup;

    private Vector3 _direction;

    private Vector3 _targetPowerupDirection;

    [SerializeField] private LayerMask _powerupLayerMask;

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

        if (_projectileID == 4)
        {
            if (_hasTargetPowerup == false)
            {
                SpiralProjectileRaycast();
            }
        }

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

        else if (_projectileID == 4)
        {
            if (_hasTargetPowerup == true)
            {
                if (_targetPowerup != null)
                {
                    //move towards the powerup hit by raycast from the jellyfish
                    _targetPowerupDirection = (_targetPowerup.position - this.transform.position).normalized;
                    transform.position += _targetPowerupDirection * _speed * Time.deltaTime;
                }

                else
                {
                    transform.position += _targetPowerupDirection * _speed * Time.deltaTime;
                }
            }

            BlowfishSpineBoundaries();
        }
    }

    void SpiralProjectileRaycast()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 20f, _powerupLayerMask);

        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.tag == "Powerup")
            {
                Debug.Log("The Spiral Shot detected a powerup!");

                _targetPowerup = hitInfo.collider.gameObject.GetComponent<Transform>();
                Debug.Log($"We hit the {_targetPowerup.name} Powerup");
            }
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * 20f, Color.yellow);

        _hasTargetPowerup = true;
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

        else if (_projectileID == 4)
        {
            if (other.tag == "Powerup")
            {
                if (other != null)
                {
                    Destroy(other.gameObject);
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
