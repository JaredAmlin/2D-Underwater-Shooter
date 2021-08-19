using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 2.5f;
    //varible for thrust for jellyfish
    [SerializeField] private float _jellyfishThrust = 150f;
    //variable for higher thrust amount for ramming the player
    [SerializeField] private float _rammingThrust = 300f;
    //variable for reassignable thrust amount
    private float _currentThrust;
    //speed for rotation in radians per second
    [SerializeField] private float _rotationSpeed = 150f;
    //variable for distance value, to ram the player or not
    [SerializeField] private float _rammingDistance = 5f;

    [SerializeField] private float _bottomRespawnRange = -5.1f;
    [SerializeField] private float _topRespawnRange = 5.1f;

    //random int variable to roll for if enemy gets a shield
    private int _randomEnemyShield;

    //varibale for enemy ID
    //0 = piranha, 1 = jellyfish
    [SerializeField] private int _enemyID;

    //bool for if the shield is active
    private bool _isEnemyShieldActive = false;

    //bool for if the enemy is dead
    private bool _isEnemyDead = false;

    //bool to handle if the enemy is ramming the player
    [SerializeField] private bool _isRamming = false;

    //variable for shield game object
    [SerializeField] private GameObject _enemyBubbleShield;

    private Rigidbody2D _enemyRB;
    private PolygonCollider2D _enemyCollider;
    private SpriteRenderer _spriteRenderer;

    private UIManager _uiManager;

    private Player _player;

    //target varible for the player transform
    private Transform _target;

    // Start is called before the first frame update
    void Start()
    {
        _enemyRB = GetComponent<Rigidbody2D>();

        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.Log("The Player is NULL");
        }

        //get handle to player transform
        _target = _player.GetComponent<Transform>();

        //null check the player transform
        if (_target == null)
        {
            Debug.Log("The Player Transform is NULL");
        }

        else
        {
            //check to see if the get component worked
            Debug.Log("The player transform is not null");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        if (_enemyID == 0)
        {
            _enemyCollider = GetComponent<PolygonCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            //give range of 5 for random shield value
            _randomEnemyShield = Random.Range(0, 5);

            if (_randomEnemyShield == 0)
            {
                //set the shield game object to active
                _enemyBubbleShield.SetActive(true);

                //set avtive bool to true
                _isEnemyShieldActive = true;
            }
        }

        else if (_enemyID == 1)
        {
            StartCoroutine(JellyfishMovementRoutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
    }

    private void FixedUpdate()
    {
        RammingJellyfish();
    }

    void EnemyMovement()
    {
        //make movement for piranha only
        if (_enemyID == 0)
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
            PiranhaBoundaries();
        }

        else if (_enemyID == 1)
        {
            
            //check to see if the player target is closer than or equal to the ramming distance. 
            if (_target != null && Vector2.Distance(this.transform.position, _target.transform.position) <= _rammingDistance)
            {
                _isRamming = true;
            }
            
            else
            {
                _isRamming = false;
            }

            JellyfishBoundaries();
        }
    }

    void RammingJellyfish()
    {
        //null check the player to avoid missing reference error if player becomes NULL
        if (_target != null)
        {
            if (_isRamming == true)
            {
                Vector2 direction = (Vector2)_target.position - _enemyRB.position;

                direction.Normalize();

                float rotationAmount = Vector3.Cross(direction, transform.up).z;

                _enemyRB.angularVelocity = -rotationAmount * _rotationSpeed;
            }
        }
    }

    IEnumerator JellyfishMovementRoutine()
    {
        while (_isEnemyDead == false)
        {
            //check the isRamming bool to assign thrust value
            if (_isRamming == true)
            {
                _currentThrust = _rammingThrust;
            }

            else
            {
                _currentThrust = _jellyfishThrust;
            }

            _enemyRB.AddForce(transform.up * _currentThrust);
            yield return new WaitForSeconds(1f);
        }
    }

    void PiranhaBoundaries()
    {
        if (transform.position.x <= -12f)
        {
            float _randomY = Random.Range(_bottomRespawnRange, _topRespawnRange);
            Vector3 _enemyRespawnPosition = new Vector3(11.5f, _randomY, 0);
            transform.position = _enemyRespawnPosition;
        }

        if (transform.position.y <= -6.5f)
        {
            Destroy(this.gameObject);
        }

        if (transform.position.x <= -11.5f && _enemyRB.gravityScale == 1)
        {
            Destroy(this.gameObject);
        }
    }

    void JellyfishBoundaries()
    {
        //screen-wrap enemy jellyfish boundaries
        if (transform.position.y > 6.5f)
        {
            transform.position = new Vector3(transform.position.x, -6.5f, 0);
        }
        else if (transform.position.y < -6.5f)
        {
            transform.position = new Vector3(transform.position.x, 6.5f, 0);
        }
        else if (transform.position.x > 11.2f)
        {
            transform.position = new Vector3(-11.2f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.2f)
        {
            transform.position = new Vector3(11.2f, transform.position.y, 0);
        }
    }

    //method for checking enemy shields
    void EnemyShieldCheck()
    {
        //check if shield is active
        if (_isEnemyShieldActive == true)
        {
            _isEnemyShieldActive = false;

            _enemyBubbleShield.SetActive(false);

            return;
        }

        else
        {
            EnemyOnDeathBehavior();
        }
    }

    void EnemyOnDeathBehavior()
    {
        if (_enemyID == 0)
        {
            _enemyCollider.enabled = false;
            _spriteRenderer.color = Color.blue;
            _spriteRenderer.flipY = true;
            _enemyRB.gravityScale = 1f;
        }
        
        else if (_enemyID == 1)
        {
            _isEnemyDead = true;
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enemy collided with " + other.transform.name + " at the location of " + other.transform.position);
        
        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.Damage();
            }

            EnemyShieldCheck();
        }

        else if (other.tag == "Tusk")
        {
            if (other != null)
            {
                Destroy(other.gameObject);
            }

            //communicate with UI Manager
            //add 10 points to score
            if (_uiManager != null)
            {
                _uiManager.UpdateScore(10);
            }

            EnemyShieldCheck();
        }

        else if (other.tag == "Bubble_Blaster")
        {
            if (other != null)
            {
                Destroy(other.gameObject);
            }

            //debug message to see if tag is working
            Debug.Log("the enemy hit the bubble blaster");

            //add 10 points to the score
            if (_uiManager != null)
            {
                _uiManager.UpdateScore(10);
            }

            EnemyShieldCheck();
        }
    }
}
