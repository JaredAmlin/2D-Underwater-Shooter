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

    //variable for sin amplidute: Blowfish
    [SerializeField] private float _amplitude;
    //variable for sin frequency: Blowfish
    [SerializeField] private float _frequency;

    private float _startPositionY;

    private float _redPiranhaFireRate;

    private float _blowFishFireRate;

    [SerializeField] private float _bottomRespawnRange = -5.1f;
    [SerializeField] private float _topRespawnRange = 5.1f;

    //random int variable to roll for if enemy gets a shield
    private int _randomEnemyShield;

    //random int variable for if enemy is smart
    private int _randomSmartEnemy;

    //varibale for enemy ID
    //0 = piranha, 1 = jellyfish, 2 = Red_Piranha, 3 = Blowfish
    [SerializeField] private int _enemyID;

    //bool for if the shield is active
    private bool _isEnemyShieldActive = false;

    //bool for if the enemy is dead
    private bool _isEnemyDead = false;

    //bool to handle if the enemy is ramming the player
    [SerializeField] private bool _isRamming = false;

    //variable to change when the enemy has fired at the target powerup
    private bool _hasFiredAtPowerup = false;

    //variable for shield game object
    [SerializeField] private GameObject _enemyBubbleShield;

    //variable for star projectile
    [SerializeField] private GameObject _starProjectile;

    //variable for spiral projectile
    [SerializeField] private GameObject _spiralProjectile;

    //variable for the piranha chompers projectile
    [SerializeField] private GameObject _piranhaChompers;
    //variable to store the Blowfish spine projectile
    [SerializeField] private GameObject _blowfishSpine;
    //variable for Blowfish Spine array
    [SerializeField] private int[] _blowfishSpines;

    //variable for layermask when raycasting for powerups
    [SerializeField] private LayerMask _powerupLayerMask;

    private Rigidbody2D _enemyRB;
    private PolygonCollider2D _enemyCollider;
    //variable for capsule collider: Blowfish
    private CapsuleCollider2D _blowfishCollider;

    private SpriteRenderer _spriteRenderer;

    private UIManager _uiManager;

    private Player _player;

    //variable for animator: Blowfish
    private Animator _animator;

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

            //range to determine if enemy is smart or not. 
            _randomSmartEnemy = Random.Range(0, 5);

            if (_randomEnemyShield == 0)
            {
                //set the shield game object to active
                _enemyBubbleShield.SetActive(true);

                //set avtive bool to true
                _isEnemyShieldActive = true;
            }

            if (_randomSmartEnemy == 0)
            {
                //make enemy SMART!!!
                //change color to yellow if smart
                _spriteRenderer.color = Color.green;
                //start firing coroutine
                StartCoroutine(SmartEnemyFireRoutine());
            }
        }

        else if (_enemyID == 1)
        {
            StartCoroutine(JellyfishMovementRoutine());
        }

        else if (_enemyID == 2)
        {
            _enemyCollider = GetComponent<PolygonCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _redPiranhaFireRate = Random.Range(2f, 5f);
            //start firing coroutine
            StartCoroutine(RedPiranhaFireRoutine());
            Debug.Log("started firing coroutine for red");
        }

        else if (_enemyID == 3)
        {
            _blowfishCollider = GetComponent<CapsuleCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            //get component the animator
            _animator = GetComponent<Animator>();
            
            //check the animator
            if (_animator == null)
            {
                Debug.LogError("The Animator on the Blowfish is NULL");
            }
            _amplitude = Random.Range(1f, 2.5f);
            _frequency = Random.Range(1f, 2.5f);
            _startPositionY = transform.position.y;
            _blowFishFireRate = Random.Range(2f, 7f);
            StartCoroutine(BlowFishSinRoutine());
            //start firing coroutine for the blowfish
            StartCoroutine(BlowfishFireRoutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();

        if (_enemyID == 1)
        {
            if (_hasFiredAtPowerup == false)
            {
                JellyfishRaycasting();
            }
        }
    }

    private void FixedUpdate()
    {
        RammingJellyfish();
    }

    void EnemyMovement()
    {
        //make movement for piranha only
        if (_enemyID == 0 || _enemyID == 3)
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

        else if (_enemyID == 2)
        {
            PiranhaBoundaries();
        }
    }

    void RammingJellyfish()
    {
        //null check the player to avoid missing reference error if player becomes NULL
        if (_target != null && _isRamming == true)
        {   
            Vector2 direction = (Vector2)_target.position - _enemyRB.position;

            direction.Normalize();

            float rotationAmount = Vector3.Cross(direction, transform.up).z;

            _enemyRB.angularVelocity = -rotationAmount * _rotationSpeed;
        }
    }

    void JellyfishRaycasting()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 20f, _powerupLayerMask);

        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.tag == "Powerup")
            {
                Debug.Log("We detected a powerup!");
                
                //Transform powerupTransform = hitInfo.transform;
                //Debug.Log($"We hit the {powerupTransform.name} Powerup");
                Instantiate(_spiralProjectile, transform.position, transform.rotation);
                StartCoroutine(JellyfishFireCooldownRoutine());
            }
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * 20f, Color.yellow);
    }

    IEnumerator JellyfishFireCooldownRoutine()
    {
        _hasFiredAtPowerup = true;

        yield return new WaitForSeconds(5f);

        _hasFiredAtPowerup = false;
    }

    IEnumerator BlowFishSinRoutine()
    {
        while (_isEnemyDead == false)
        {
            float currentPositionX = transform.position.x;
            float sinMovementY = Mathf.Sin(Time.time * _frequency) * _amplitude;
            transform.position = new Vector3(currentPositionX, _startPositionY + sinMovementY, 0);
            yield return null;
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

    IEnumerator RedPiranhaFireRoutine()
    {
        yield return new WaitForSeconds(_redPiranhaFireRate);

        while (_isEnemyDead == false)
        {
            _redPiranhaFireRate = Random.Range(2f, 5f);

            Vector3 _firePoint = new Vector3(-0.9f, 0, 0);

            Instantiate(_piranhaChompers, this.transform.position + _firePoint, Quaternion.identity);

            yield return new WaitForSeconds(_redPiranhaFireRate);
        }
    }

    //firing routine for the blowfish
    IEnumerator BlowfishFireRoutine()
    {
        yield return new WaitForSeconds(_blowFishFireRate);

        while (_isEnemyDead == false)
        {
            _animator.SetTrigger("IsPuffing");

            yield return new WaitForSeconds(0.1f);

            _blowFishFireRate = Random.Range(2f, 7f);

            Vector3 _firePoint = new Vector3(-0.4f, 0, 0);

            int randomSpines = Random.Range(3, 8);
            _blowfishSpines = new int[randomSpines];

            for (int spineIndex = 0; spineIndex < _blowfishSpines.Length; spineIndex++)
            {
                float _maxDegreesOfRotation = 360;
                float _blowfishSpineRotation = ((float)_maxDegreesOfRotation / _blowfishSpines.Length) * spineIndex;
                Instantiate(_blowfishSpine, this.transform.position + _firePoint, Quaternion.Euler(0, 0, _blowfishSpineRotation));
            }

            yield return new WaitForSeconds(_blowFishFireRate);
        }
    }

    IEnumerator SmartEnemyFireRoutine()
    {
        while (_isEnemyDead == false)
        {
            float _offset = -3f;

            //fire at the player when behind it
            if (_target != null && this.transform.position.x < (_target.transform.position.x + _offset))
            {
                //fire at the player
                Instantiate(_starProjectile, transform.position, Quaternion.identity);

                yield return new WaitForSeconds(5f);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void PiranhaBoundaries()
    {
        if (transform.position.x <= -12f)
        {
            float _randomY = Random.Range(_bottomRespawnRange, _topRespawnRange);
            _startPositionY = _randomY;
            Vector3 _enemyRespawnPosition = new Vector3(11.5f, _randomY, 0);
            transform.position = _enemyRespawnPosition;
        }

        if (transform.position.y <= -8.5f)
        {
            if (_enemyID == 2)
            {
                Destroy(this.transform.parent.gameObject);
            }
            
            Destroy(this.gameObject);
        }

        if (transform.position.x <= -11.5f && _enemyRB.gravityScale == 1)
        {
            if (_enemyID == 2)
            {
                Destroy(this.transform.parent.gameObject);
            }

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
            _isEnemyDead = true;
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

        else if (_enemyID == 2)
        {
            CircleCollider2D _parentCollider = this.GetComponentInParent<CircleCollider2D>();

            Destroy(_parentCollider);

            _isEnemyDead = true;
            _enemyCollider.enabled = false;
            _spriteRenderer.color = Color.blue;
            _spriteRenderer.flipY = true;
            _enemyRB.gravityScale = 1f;
        }

        else if (_enemyID == 3)
        {
            _isEnemyDead = true;
            _blowfishCollider.enabled = false;
            _spriteRenderer.color = Color.blue;
            _spriteRenderer.flipY = true;
            _enemyRB.gravityScale = 1f;
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
