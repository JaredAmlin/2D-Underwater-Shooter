using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speed;
    [SerializeField] private float _defaultSpeed = 4f;
    [SerializeField] private float _flipperBoostSpeed = 7f;

    [SerializeField] private float _startPosX = -6f;

    [SerializeField] private float _rightBoundary = 0f;
    [SerializeField] private float _leftBoundary = 0f;
    [SerializeField] private float _ceilingBoundary = 0f;
    [SerializeField] private float _floorBoundary = 0f;

    [SerializeField] private float _tuskFireRate = 0.5f;
    private float _canFireTusk = 0f;

    [SerializeField] private int _currentLives;
    [SerializeField] private int _maxLives = 3;
    private int _minLives = 0;

    private bool _isTripleTuskActive = false;
    private bool _isFlipperBoostActive = false;
    private bool _isShieldActive = false;

    [SerializeField] private GameObject _tuskPrefab;
    [SerializeField] private GameObject _tripleTuskPrefab;
    [SerializeField] private GameObject _bubbleShield;

    //game object variables for player damage visualization
    [SerializeField] private GameObject _damageScarSingle;
    [SerializeField] private GameObject _damageScarDouble;

    //variable to hold my CameraShake class
    private CameraShake _cameraHolder;

    private PolygonCollider2D _playerCollider;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private AudioClip _pewPewSoundClip;
    [SerializeField] private AudioClip _bubblePopSoundClip;
    private AudioSource _audioSource;
  
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

        _speed = _defaultSpeed;

        //set the lives to be the maximum value
        _currentLives = _maxLives;

        //find the camera holder and get the CameraShake class component
        _cameraHolder = GameObject.Find("Camera_Holder").GetComponent<CameraShake>();

        //debug the GetComponent
        if (_cameraHolder == null)
        {
            Debug.LogError("The Camera Holder is NULL");
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
    }

    void PlayerMovement()
    {
        float _horizontalInput = Input.GetAxisRaw("Horizontal");
        float _verticalInput = Input.GetAxisRaw("Vertical");

        PlayerBoundaries();

        FlipperBoost();

        Vector3 _direction = new Vector3(_horizontalInput, _verticalInput, 0).normalized;
        transform.Translate(_direction * _speed * Time.deltaTime);
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
        _canFireTusk = Time.time + _tuskFireRate;
        Vector3 _firePoint = new Vector3(2f, 0.2f, 0);

        if (_isTripleTuskActive == true)
        {
            Instantiate(_tripleTuskPrefab, transform.position + _firePoint, Quaternion.identity);
        }

        else
        {
            Instantiate(_tuskPrefab, transform.position + _firePoint, Quaternion.identity);
        }

        //play fire sound
        _audioSource.PlayOneShot(_pewPewSoundClip);
    }

    void FlipperBoost()
    {
        if (_isFlipperBoostActive == false)
        {
            _speed = _defaultSpeed;
        }

        else
        {
            _speed = _flipperBoostSpeed;
        }
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _bubbleShield.SetActive(false);
            _audioSource.PlayOneShot(_bubblePopSoundClip);
            return;
        }

        else
        {
            //start coroutine on the camera shake script.
            StartCoroutine(_cameraHolder.ShakeTheCamera(0.5f, 0.5f));

            _currentLives--;
            //_lives = _lives -1;
            //_lives -= 1;

            //safeguard to keep lives from going below zero
            int _currentLivesDamageClamp = Mathf.Clamp(_currentLives, _minLives, _maxLives);

            //assign the current life to the clamped value
            _currentLives = _currentLivesDamageClamp;

            _uiManager.UpdateLives(_currentLives);

            PlayerHurtAnimation();

            //trigger Player Damage animation
            _animator.SetTrigger("PlayerHurt");
        }

        //check if current lives is les than max, if so display damage scar
        if (_currentLives < _maxLives)
        {
            _damageScarSingle.SetActive(true);
        }

        //if current lives is less than 2 activate the second damage scar
        if (_currentLives < 2)
        {
            _damageScarDouble.SetActive(true);
        }

        //if lives is less than one, destroy the player
        if (_currentLives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _uiManager.GameOverText();
            Destroy(this.gameObject);
        }
    }

    public void Heal()
    {
        //send a message to the console to let me know I got this far
        Debug.Log("the health powerup hit the player");
        
        //take the current value of lives and add one
        _currentLives++;

        //use Mathf to clamp the lives value between 0 and 3
        int _currentLivesHealClamp = Mathf.Clamp(_currentLives, _minLives, _maxLives);

        //assign current life to be the clamped value
        _currentLives = _currentLivesHealClamp;

        //tell the uiManager to update the lives sprites
        _uiManager.UpdateLives(_currentLives);

        //deactivate the second scar if the player has two or more lives
        if (_currentLives > 1)
        {
            _damageScarDouble.SetActive(false);
        }

        //deactivate the first scar if player has full life
        if (_currentLives == _maxLives)
        {
            _damageScarSingle.SetActive(false);
        }
    }

    public void TripleTuskActive()
    {
        StartCoroutine(TripleTuskPowerDownRoutine());
    }

    IEnumerator TripleTuskPowerDownRoutine()
    {
        _isTripleTuskActive = true;

        yield return new WaitForSeconds(5f);

        _isTripleTuskActive = false;
    }

    public void FlipperBoostActive()
    {
        StartCoroutine(FlipperBoostPowerDownRoutine());
    }

    IEnumerator FlipperBoostPowerDownRoutine()
    {
        _isFlipperBoostActive = true;

        yield return new WaitForSeconds(5f);

        _isFlipperBoostActive = false;
    }

     public void ShieldActive()
    {
        _isShieldActive = true;
        _bubbleShield.SetActive(true);
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

        /*set both damage scars to inactive when 
        the sprite render is set to clear*/
        _damageScarSingle.SetActive(false);
        _damageScarDouble.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.white;

        /*check current lives to set 
        damage scars active if needed*/
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

        /*set both damage scars to inactive when 
        the sprite render is set to clear*/
        _damageScarSingle.SetActive(false);
        _damageScarDouble.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.white;

        /*check current lives to set 
        damage scars active if needed*/
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

        /*set both damage scars to inactive when 
        the sprite render is set to clear*/
        _damageScarSingle.SetActive(false);
        _damageScarDouble.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.white;

        /*check current lives to set 
        damage scars active if needed*/
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
