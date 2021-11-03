using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _defaultSpeed;
    [SerializeField] private float _minSpeed = 4f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _flipperBoostSpeed = 7f;
    [SerializeField] private float _accelFactor = 0.1f;
    [SerializeField] private float _decelFactor = 0.2f;
    [SerializeField] private float _currentThrust;
    [SerializeField] private float _maxThrust = 100f;
    [SerializeField] private float _minThrust = 0f;
    [SerializeField] private float _thrustDecelFactor = 0.2f;
    [SerializeField] private float _thrustAccelFactor = 0.1f;
    [SerializeField] private float _thrusterCoolDownTime = 2f;

    [SerializeField] private float _startPosX = -6f;

    [SerializeField] private float _rightBoundary = 0f;
    [SerializeField] private float _leftBoundary = 0f;
    [SerializeField] private float _ceilingBoundary = 0f;
    [SerializeField] private float _floorBoundary = 0f;

    [SerializeField] private float _tuskFireRate = 0.5f;
    private float _canFireTusk = 0f;

    private float _bubbleBlasterFireRate = 0.1f;
    private float _canFireBubbleBlaster = 0f;

    [SerializeField] private int _currentTuskAmmo;
    [SerializeField] private int _maxTuskAmmo = 15;
    private int _minTuskAmmo = 0;

    [SerializeField] private int _currentLives;
    [SerializeField] private int _maxLives = 3;
    private int _minLives = 0;
    [SerializeField] private int _shieldHealth;
    private int _maxShieldHealth = 3;
    private int _minShieldHealth = 0;


    [SerializeField] private bool _isTripleTuskActive = false;
    private bool _isFlipperBoostActive = false;
    private bool _isShieldActive = false;
    [SerializeField] private bool _isBubbleBlasterActive = false;
    [SerializeField] private bool _isHomingTuskActive = false;
 
    private bool _hasSpawnedAmmoReload = false;

    [SerializeField] private bool _isPlayerPenaltyActive = false;

    [SerializeField] private bool _canThrust = false;
    [SerializeField] private bool _isThrusting = false;
    [SerializeField] private bool _isThrustingSwitch = false;

    private bool _isPullingPowerups = false;

    [SerializeField] private GameObject _tuskPrefab;
    [SerializeField] private GameObject _tripleTuskPrefab;
    [SerializeField] private GameObject _bubbleShield;

    [SerializeField] private GameObject _bubbleBlasterPrefab;
    [SerializeField] private GameObject _homingTusk;

    [SerializeField] private GameObject _damageScarSingle;
    [SerializeField] private GameObject _damageScarDouble;

    private CameraShake _cameraHolder;

    private PolygonCollider2D _playerCollider;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private Animator _animator;

    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _shieldSpriteRenderer;

    [SerializeField] private AudioClip _pewPewSoundClip;
    [SerializeField] private AudioClip _bubblePopSoundClip;
    [SerializeField] private AudioClip _outOfAmmoSoundClip;
    private AudioSource _audioSource;

    [SerializeField] private Color _shieldDamageColor1;
    [SerializeField] private Color _shieldDamageColor2;
  
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("The Animator is NULL");
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer == null)
        {
            Debug.LogError("The Sprite Renderer is NULL");
        }

        _playerCollider = GetComponent<PolygonCollider2D>();

        if (_playerCollider == null)
        {
            Debug.LogError("The Player Collider is NULL");
        }

        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on the Player is NULL");
        }

        transform.position = new Vector3(_startPosX, 0, 0);

        _currentSpeed = _minSpeed;
        _defaultSpeed = _minSpeed;

        _shieldHealth = 0;

        _currentLives = _maxLives;

        _currentThrust = _maxThrust;

        _canThrust = true;

        _currentTuskAmmo = _maxTuskAmmo;

        _uiManager.UpdateTuskAmmo(_currentTuskAmmo);

        _cameraHolder = GameObject.Find("Camera_Holder").GetComponent<CameraShake>();

        if (_cameraHolder == null)
        {
            Debug.LogError("The Camera Holder is NULL");
        }

        _shieldSpriteRenderer = _bubbleShield.GetComponent<SpriteRenderer>();
        if  (_shieldSpriteRenderer == null)
        {
            Debug.LogError("The Sprite Renderer on the Player Shield is NULL");
        }

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();

        if (Input.GetKey(KeyCode.Space) && Time.time > _canFireTusk)
        {
            FireWeapon();
        }

        PullPowerupsToPlayer();
    }

    void PlayerMovement()
    {
        float _horizontalInput = Input.GetAxisRaw("Horizontal");
        float _verticalInput = Input.GetAxisRaw("Vertical");

        PlayerBoundaries();

        FlipperThrusters();

        Vector3 _direction = new Vector3(_horizontalInput, _verticalInput, 0).normalized;

        if (_isPlayerPenaltyActive == true)
        {
            transform.Translate(-_direction * _currentSpeed * Time.deltaTime);
        }

        else
        {
            transform.Translate(_direction * _currentSpeed * Time.deltaTime);
        }
    }

    void PlayerBoundaries()
    {
        float _xMovementClamp = Mathf.Clamp(transform.position.x, _leftBoundary, _rightBoundary);
        float _yMovementClamp = Mathf.Clamp(transform.position.y, _floorBoundary, _ceilingBoundary);
        Vector3 _limitPlayerMovement = new Vector3(_xMovementClamp, _yMovementClamp, 0);
        transform.position = _limitPlayerMovement;
    }

    void FireWeapon()
    {
        if (_isBubbleBlasterActive == true && Time.time > _canFireBubbleBlaster)
        {
            _canFireBubbleBlaster = Time.time + _bubbleBlasterFireRate;
            Vector3 _firePoint = new Vector3(2f, 0.2f, 0);

            Instantiate(_bubbleBlasterPrefab, transform.position + _firePoint, Quaternion.identity);
        }

        else if (_isBubbleBlasterActive == false)
        {
            _canFireTusk = Time.time + _tuskFireRate;
            Vector3 _firePoint = new Vector3(2f, 0.2f, 0);

            if (_isTripleTuskActive == true && _currentTuskAmmo > _minTuskAmmo)
            {
                if (_isHomingTuskActive == false)
                {
                    Instantiate(_tripleTuskPrefab, transform.position + _firePoint, Quaternion.identity);

                    _audioSource.PlayOneShot(_pewPewSoundClip);
                }
            }

            else if (_isHomingTuskActive == true && _currentTuskAmmo > _minTuskAmmo)
            {
                if (_isTripleTuskActive == false)
                {
                    Instantiate(_homingTusk, transform.position + _firePoint, Quaternion.identity);

                    _audioSource.PlayOneShot(_pewPewSoundClip);
                }
            }

            else if (_currentTuskAmmo > _minTuskAmmo)
            {
                Instantiate(_tuskPrefab, transform.position + _firePoint, Quaternion.identity);

                _audioSource.PlayOneShot(_pewPewSoundClip);
            }

            else
            {
                _audioSource.PlayOneShot(_outOfAmmoSoundClip);
            }

            _currentTuskAmmo--;

            int _tuskAmmoClamp = Mathf.Clamp(_currentTuskAmmo, _minTuskAmmo, _maxTuskAmmo);

            _currentTuskAmmo = _tuskAmmoClamp;

            _uiManager.UpdateTuskAmmo(_currentTuskAmmo);

            if (_currentTuskAmmo == _minTuskAmmo && _hasSpawnedAmmoReload == false)
            {
                _hasSpawnedAmmoReload = true;
                _spawnManager.PlayerOutOfAmmo();
            }
        }
    }

    void FlipperThrusters()
    {
        if (_isFlipperBoostActive == true)
        {
            _minSpeed = _flipperBoostSpeed;
        }

        else if (_isFlipperBoostActive == false)
        {
            _minSpeed = _defaultSpeed;
        }

        float _speedClamp = Mathf.Clamp(_currentSpeed, _minSpeed, _maxSpeed);

        float _thrustClamp = Mathf.Clamp(_currentThrust, _minThrust, _maxThrust);

        if (_currentThrust <= _minThrust)
        {
            ThrusterCoolDown();

            _uiManager.ThrusterCoolDown(_thrusterCoolDownTime);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        { 
            if (_isThrustingSwitch == false && _canThrust == true)
            {
                _isThrusting = true;
                _isThrustingSwitch = true;
            }

            if (_canThrust == true)
            {
                _currentSpeed = _speedClamp += _accelFactor * Time.deltaTime;

                _currentThrust = _thrustClamp -= _thrustDecelFactor * Time.deltaTime;
            }
        }
 
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isThrusting = false;
            _isThrustingSwitch = false;
        }

        if (_currentSpeed > _minSpeed && _isThrusting == false)
        {
            _currentSpeed = _speedClamp -= _decelFactor * Time.deltaTime;
        }

        else if (_currentSpeed <= _minSpeed)
        {
            _currentSpeed = _minSpeed;
        }

        if (_currentThrust < _maxThrust && _isThrusting == false)
        {
            _currentThrust = _thrustClamp += _thrustAccelFactor * Time.deltaTime;
        }

        else if (_currentThrust >= _maxThrust)
        {
            _currentThrust = _maxThrust;
        }

        _uiManager.UpdateThruster(_currentThrust);
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _shieldHealth--;

            int _shieldHealthClamp = Mathf.Clamp(_shieldHealth, _minShieldHealth, _maxShieldHealth);

            _shieldHealth = _shieldHealthClamp;

            if (_shieldHealth == 2)
            {
                _shieldSpriteRenderer.color = _shieldDamageColor1;
            }

            else if (_shieldHealth == 1)
            {
                _shieldSpriteRenderer.color = _shieldDamageColor2;
            }

            if (_shieldHealth <= _minShieldHealth)
            {
                _isShieldActive = false;
                _bubbleShield.SetActive(false);
                _audioSource.PlayOneShot(_bubblePopSoundClip);
                _shieldSpriteRenderer.color = Color.white;
            }
            
            return;
        }

        else
        {
            StartCoroutine(_cameraHolder.ShakeTheCamera(0.5f, 0.5f));

            _currentLives--;
         
            int _currentLivesDamageClamp = Mathf.Clamp(_currentLives, _minLives, _maxLives);

            _currentLives = _currentLivesDamageClamp;

            _uiManager.UpdateLives(_currentLives);

            PlayerHurtAnimation();

            _animator.SetTrigger("PlayerHurt");
        }

        if (_currentLives < _maxLives)
        {
            _damageScarSingle.SetActive(true);
        }

        if (_currentLives < 2)
        {
            _damageScarDouble.SetActive(true);
        }

        if (_currentLives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _uiManager.GameOverText();
            Destroy(this.gameObject);
        }
    }

    public void Heal()
    {
        _currentLives++;

        int _currentLivesHealClamp = Mathf.Clamp(_currentLives, _minLives, _maxLives);

        _currentLives = _currentLivesHealClamp;

        _uiManager.UpdateLives(_currentLives);

        if (_currentLives > 1)
        {
            _damageScarDouble.SetActive(false);
        }

        if (_currentLives == _maxLives)
        {
            _damageScarSingle.SetActive(false);
        }
    }

    public void PlayerReload()
    {
        _currentTuskAmmo = _maxTuskAmmo;
        _uiManager.UpdateTuskAmmo(_currentTuskAmmo);
        _hasSpawnedAmmoReload = false;
    }

    public void TripleTuskActive()
    {
        StartCoroutine(TripleTuskPowerDownRoutine());
    }

    IEnumerator TripleTuskPowerDownRoutine()
    {
        if (_isHomingTuskActive == true)
        {
            _isHomingTuskActive = false;
        }

        _isTripleTuskActive = true;

        yield return new WaitForSeconds(5f);

        _isTripleTuskActive = false;
    }

    public void HomingTuskActive()
    {
        StartCoroutine(HomingTuskPowerDownRoutine());
    }

    IEnumerator HomingTuskPowerDownRoutine()
    {
        if (_isTripleTuskActive == true)
        {
            _isTripleTuskActive = false;
        }

        _isHomingTuskActive = true;

        yield return new WaitForSeconds(5f);

        _isHomingTuskActive = false;
    }

    public void FlipperBoostActive()
    {
        _currentSpeed = _flipperBoostSpeed;
        StartCoroutine(FlipperBoostPowerDownRoutine());
    }

    IEnumerator FlipperBoostPowerDownRoutine()
    {
        _isFlipperBoostActive = true;

        yield return new WaitForSeconds(5f);

        _isFlipperBoostActive = false;

        _currentSpeed = _defaultSpeed;
    }

    public void BubbleBlasterActive()
    {
        StartCoroutine(BubbleBlasterPowerDownRoutine());
    }

    IEnumerator BubbleBlasterPowerDownRoutine()
    {
        _isBubbleBlasterActive = true;

        yield return new WaitForSeconds(5f);

        _isBubbleBlasterActive = false;
    }

     public void ShieldActive()
    {
        _shieldHealth = _maxShieldHealth;
        _isShieldActive = true;
        _shieldSpriteRenderer.color = Color.white;
        _bubbleShield.SetActive(true);
    }

    void ThrusterCoolDown()
    {
        StartCoroutine(ThrusterCoolDownRoutine());
    }

    IEnumerator ThrusterCoolDownRoutine()
    {
        _canThrust = false;
        _isThrusting = false;
        _isThrustingSwitch = false;
        yield return new WaitForSeconds(_thrusterCoolDownTime);
        _canThrust = true;
    }

    public void PlayerPenaltyActive()
    {
        StartCoroutine(PlayerPenaltyPowerDownRoutine());
    }

    IEnumerator PlayerPenaltyPowerDownRoutine()
    {
        _isPlayerPenaltyActive = true;

        yield return new WaitForSeconds(5f);

        _isPlayerPenaltyActive = false;
    }

    void PullPowerupsToPlayer()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _isPullingPowerups = true;
        }

        else if (Input.GetKeyUp(KeyCode.C))
        {
            _isPullingPowerups = false;
        }
    }

    public bool PowerupPullCheck()
    {
        return _isPullingPowerups;
    }

    public int PlayerLives()
    {
        return _currentLives;
    }

    public int PlayerShield()
    {
        return _shieldHealth;
    }

    void PlayerHurtAnimation()
    {
        StartCoroutine(PlayerHurtAnimationPowerDownRoutine());
    }

    IEnumerator PlayerHurtAnimationPowerDownRoutine()
    {
        _playerCollider.enabled = false;

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.clear;

        _damageScarSingle.SetActive(false);
        _damageScarDouble.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.white;

        if (_currentLives < _maxLives)
        {
            _damageScarSingle.SetActive(true);
        }

        if (_currentLives < 2)
        {
            _damageScarDouble.SetActive(true);
        }

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.clear;

        _damageScarSingle.SetActive(false);
        _damageScarDouble.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.white;

        if (_currentLives < _maxLives)
        {
            _damageScarSingle.SetActive(true);
        }

        if (_currentLives < 2)
        {
            _damageScarDouble.SetActive(true);
        }

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.clear;

        _damageScarSingle.SetActive(false);
        _damageScarDouble.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.white;

        if (_currentLives < _maxLives)
        {
            _damageScarSingle.SetActive(true);
        }

        if (_currentLives < 2)
        {
            _damageScarDouble.SetActive(true);
        }

        if (_playerCollider != null)
        {
            _playerCollider.enabled = true;
        }
    }
}
