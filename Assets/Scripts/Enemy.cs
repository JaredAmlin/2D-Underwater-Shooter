using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 2.5f;
    [SerializeField] private float _jellyfishThrust = 150f;
    [SerializeField] private float _rammingThrust = 300f;
    private float _currentThrust;
    [SerializeField] private float _rotationSpeed = 150f;
    [SerializeField] private float _rammingDistance = 5f;

    [SerializeField] private float _amplitude;
    [SerializeField] private float _frequency;

    private float _startPositionY;

    private float _redPiranhaFireRate;
    private float _blowFishFireRate;

    [SerializeField] private float _bottomRespawnRange = -5.1f;
    [SerializeField] private float _topRespawnRange = 5.1f;

    private int _randomEnemyShield;

    private int _randomSmartEnemy;

    //0 = piranha, 1 = jellyfish, 2 = Red_Piranha, 3 = Blowfish
    [SerializeField] private int _enemyID;

    private bool _isEnemyShieldActive = false;

    private bool _isEnemyDead = false;

    [SerializeField] private bool _isRamming = false;

    private bool _hasFiredAtPowerup = false;

    [SerializeField] private GameObject _enemyBubbleShield;
    [SerializeField] private GameObject _starProjectile;
    [SerializeField] private GameObject _spiralProjectile;
    [SerializeField] private GameObject _piranhaChompers;
    [SerializeField] private GameObject _blowfishSpine;
 
    [SerializeField] private int[] _blowfishSpines;

    [SerializeField] private GameObject _deadEnemy;

    [SerializeField] private LayerMask _powerupLayerMask;

    private Rigidbody2D _enemyRB;
    private PolygonCollider2D _enemyCollider;
    private CapsuleCollider2D _blowfishCollider;

    private SpriteRenderer _spriteRenderer;

    private UIManager _uiManager;

    private Player _player;

    private Animator _animator;

    private Transform _target;

    // Start is called before the first frame update
    void Start()
    {
        _enemyRB = GetComponent<Rigidbody2D>();

        if (_enemyRB == null)
        {
            Debug.LogError("The enemy Rigidbody is NULL");
        }

        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.Log("The Player is NULL");
        }

        _target = _player.GetComponent<Transform>();

        if (_target == null)
        {
            Debug.Log("The Player Transform is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        if (_enemyID == 0)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            _randomEnemyShield = Random.Range(0, 5);

            _randomSmartEnemy = Random.Range(0, 5);

            if (_randomEnemyShield == 0)
            {
                _enemyBubbleShield.SetActive(true);
                _isEnemyShieldActive = true;
            }

            if (_randomSmartEnemy == 0)
            {
                _spriteRenderer.color = Color.green;

                StartCoroutine(SmartEnemyFireRoutine());
            }
        }

        else if (_enemyID == 1)
        {
            StartCoroutine(JellyfishMovementRoutine());
        }

        else if (_enemyID == 2)
        {
            _redPiranhaFireRate = Random.Range(2f, 5f);
            StartCoroutine(RedPiranhaFireRoutine());
        }

        else if (_enemyID == 3)
        {
            _animator = GetComponent<Animator>();
            
            if (_animator == null)
            {
                Debug.LogError("The Animator on the Blowfish is NULL");
            }

            _amplitude = Random.Range(1f, 2.5f);
            _frequency = Random.Range(1f, 2.5f);
            _startPositionY = transform.position.y;
            _blowFishFireRate = Random.Range(2f, 7f);
            StartCoroutine(BlowFishSinRoutine());
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
        if (_enemyID == 0 || _enemyID == 3)
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
            PiranhaBoundaries();
        }

        else if (_enemyID == 1)
        {
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
                Instantiate(_spiralProjectile, transform.position, transform.rotation);
                StartCoroutine(JellyfishFireCooldownRoutine());
            }
        }

        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * 20f, Color.yellow);
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

            if (_target != null && this.transform.position.x < (_target.transform.position.x + _offset))
            {
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

    void EnemyShieldCheck()
    {
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
        Instantiate(_deadEnemy, transform.position, Quaternion.identity);

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {  
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
            }

            EnemyShieldCheck();
        }

        else if (other.CompareTag("Tusk"))
        {
            if (other != null)
            {
                Destroy(other.gameObject);
            }

            if (_uiManager != null)
            {
                _uiManager.UpdateScore(10);
            }

            EnemyShieldCheck();
        }

        else if (other.CompareTag("Bubble_Blaster"))
        {
            if (other != null)
            {
                Destroy(other.gameObject);
            }

            if (_uiManager != null)
            {
                _uiManager.UpdateScore(10);
            }

            EnemyShieldCheck();
        }
    }
}
