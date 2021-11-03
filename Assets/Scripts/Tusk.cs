using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tusk : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    private float _distance;
    private float _closestDistance = Mathf.Infinity;

    [SerializeField] private float _rotationSpeed = 150f;

    //0 = Tusk, 1 = Piranha_Chompers, 2 = Blowfish_Spine, 3 = Star, 4 = Spiral, 5 = Homing_Tusk
    [SerializeField] private int _projectileID;

    private bool _hasTargetPowerup = false;

    [SerializeField] private bool _hasEnemyTarget = false;

    private Player _player;

    private Transform _target;

    private Transform _targetPowerup;

    private Vector3 _direction;

    private Vector3 _targetPowerupDirection;

    [SerializeField] private LayerMask _powerupLayerMask;

    private GameObject[] _enemies;

    private Transform _enemyTarget = null;

    private Rigidbody2D _homingTuskRB;

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

        if (_projectileID ==5)
        {
            _homingTuskRB = GetComponent<Rigidbody2D>();

            if (_homingTuskRB == null)
            {
                Debug.LogError("The Rigidbody on the homing missile is NULL");
            }

            FindEnemyTargets();
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

    private void FixedUpdate()
    {
        if (_projectileID == 5)
        {
            HomingTuskMovement();
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
            transform.position += _direction * _speed * Time.deltaTime;

            BlowfishSpineBoundaries();
        }

        else if (_projectileID == 4)
        {
            if (_hasTargetPowerup == true)
            {
                if (_targetPowerup != null)
                {
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

    void HomingTuskMovement()
    {
        _homingTuskRB.velocity = transform.right * _speed * Time.deltaTime;

        if (_enemyTarget != null)
        {
            Vector2 direction = (Vector2)_enemyTarget.position - _homingTuskRB.position;

            direction.Normalize();

            float rotationAmount = Vector3.Cross(direction, transform.right).z;

            _homingTuskRB.angularVelocity = -rotationAmount * _rotationSpeed;

            _homingTuskRB.velocity = transform.right * _speed * Time.deltaTime;
        }

        else if (_enemyTarget == null)
        {
            _hasEnemyTarget = false;
            _closestDistance = Mathf.Infinity;
            FindEnemyTargets();
        }

        BlowfishSpineBoundaries();
    }

    void SpiralProjectileRaycast()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 20f, _powerupLayerMask);

        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.tag == "Powerup")
            {
                _targetPowerup = hitInfo.collider.gameObject.GetComponent<Transform>();
            }
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * 20f, Color.yellow);

        _hasTargetPowerup = true;
    }

    void FindEnemyTargets()
    {
        if (_hasEnemyTarget == false)
        {
            _enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (var enemy in _enemies)
            {
                _distance = (enemy.transform.position - this.transform.position).sqrMagnitude;
              
                if (_distance < _closestDistance)
                {
                    _closestDistance = _distance;
                   
                    _enemyTarget = enemy.transform;
                   
                    _hasEnemyTarget = true;                   
                }
            }

            if (_enemyTarget == null)
            {
                GameObject anglerLight = GameObject.FindGameObjectWithTag("AnglerLight");

                if (anglerLight != null)
                {
                    anglerLight.GetComponent<GameObject>();

                    Transform anglerTarget = GameObject.FindGameObjectWithTag("AnglerTarget").GetComponent<Transform>();

                    AnglerfishLight anglerfishLight = anglerLight.GetComponent<AnglerfishLight>();

                    bool isJawOpen = anglerfishLight.IsJawOpen();

                    if (isJawOpen == false)
                    {
                        _hasEnemyTarget = true;
                        _enemyTarget = anglerLight.transform;
                    }

                    else if (isJawOpen == true)
                    {
                        _hasEnemyTarget = true;
                        _enemyTarget = anglerTarget;
                    }
                }
            }
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
            if (other.CompareTag("Player"))
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
            if (other.CompareTag("Powerup"))
            {
                if (other != null)
                {
                    Destroy(other.gameObject);
                    Destroy(this.gameObject);
                }
            }
        }

        else if (_projectileID == 5)
        {
            if (other.CompareTag("AnglerLight"))
            {
                AnglerfishLight anglerLight = other.transform.GetComponent<AnglerfishLight>();

                if (anglerLight != null)
                {
                    anglerLight.AnglerfishLightDamage();
                }

                Destroy(this.gameObject);
            }

            else if (other.CompareTag("AnglerTarget"))
            {
                AnglerfishTarget anglerTarget = other.transform.GetComponent<AnglerfishTarget>();

                if (anglerTarget != null)
                {
                    anglerTarget.AnglerfishTargetDamage(10);
                }
                
                Destroy(this.gameObject);
            }

            else if (other.CompareTag("AnglerFish"))
            {
                Destroy(this.gameObject);
            }
        }

        else if (_projectileID == 0)
        {
            if (other.tag == "AnglerLight")
            { 
                AnglerfishLight anglerLight = other.transform.GetComponent<AnglerfishLight>();

                if (anglerLight != null)
                {                  
                    anglerLight.AnglerfishLightDamage();
                }
                
                Destroy(this.gameObject);
            }

            else if (other.CompareTag("AnglerTarget"))
            {
                AnglerfishTarget anglerTarget = other.transform.GetComponent<AnglerfishTarget>();

                if (anglerTarget != null)
                {
                    anglerTarget.AnglerfishTargetDamage(5);
                }
               
                Destroy(this.gameObject);
            }

            else if (other.CompareTag("AnglerFish"))
            {
                Destroy(this.gameObject);
            }
        }
    }
}
